using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
//using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/* Notes;
 * I'm working on another project to create a neural network from scratch in c#. 
 * 
 * This is ment to be shared or used as desired as long as the understanding that
 * my origianl content is respected.
 * 
 * This project is finding methods to read images to use for pixel extraction in 
 * my nn.
 * 
 * I am sure there will be better ways of doing everything I attempt to do. Please 
 * comment but be aware that I'm not a professional coder. I'm extracting ideas from 
 * other works and attempting to build it in c# because I can read c# and I feel that
 * with all of the videos available on YouTube and all of the software that allows for
 * almost instant building of NNs and CNNs there are still pieces I'm missing. I don't
 * expect this work to be anything new.
 * 
 * thanks to Microsoft and the following
 * https://github.com/
 * https://stackoverflow.com/questions/10442269/scaling-a-system-drawing-bitmap-to-a-given-size-while-maintaining-aspect-ratio
 * https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
 * LW
 * 
 * found items for review
 * https://imageprocessor.org/
 * https://social.msdn.microsoft.com/Forums/vstudio/en-US/bc35d445-10a1-473c-9348-eefba2197e68/device-independent-bitmap-dib?forum=netfxbcl
 * https://stackoverflow.com/questions/11452246/add-a-bitmap-header-to-a-byte-array-then-create-a-bitmap-file
 * https://en.wikipedia.org/wiki/BMP_file_format#Example_1
 * https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap?view=netframework-4.8
 * https://docs.microsoft.com/en-us/dotnet/api/system.drawing.imaging.bitmapdata?view=netframework-4.8
 */

namespace MacGetImage
{
    public class GetImageClass : IDisposable 
    {
        OpenFileDialog ofd;
        public Bitmap MacBM { get; set; }
        public string FullFileName { get; set; } = "";
        public GetImageClass()
        {
        }

        public string FindFile()
        {
            ofd = new OpenFileDialog
            {
                //InitialDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)); // didn't work???
                InitialDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal)) + @"\MyPictures",
                Filter = "Image Files (*.bmp or *.jpg or *.png)|*.bmp;*.jpg;*.png"
            };
            ofd.ShowDialog();
            FullFileName = ofd.FileName;
            return FullFileName;
        }

        public Bitmap LoadBitmapFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                MacBM = new Bitmap(filename);
                return MacBM;
            }
            else { return null; }
        }

        public Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            SolidBrush brush = new SolidBrush(Color.Black);
            Rectangle r = new Rectangle(0, 0, width, height);

            // I found that the values needed to be converted to floating point type or the devision resulted in 0.
            double scale = Math.Min(((double)width / (double)image.Width), ((double)height / (double)image.Height));
            int scalewidth = (int)(Math.Floor((double)image.Width * scale));
            int scaleheight = (int)(Math.Floor((double)image.Height * scale));

            Bitmap bmp = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.FillRectangle(brush, r);
                graphics.DrawImage(image, (width - scalewidth) / 2, (height - scaleheight) / 2, scalewidth, scaleheight);
                //Debug.Print(width.ToString() + ", " + height.ToString() + ", " + scale.ToString() );
            }

            return new Bitmap(bmp);
        }

        public void GetPixelArray(Bitmap bitmap)
        {
            PixelFormat bits = bitmap.PixelFormat;

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
            ofd.Dispose();
            MacBM.Dispose();
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~GetImageClass()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }



    // https://stackoverflow.com/questions/24701703/c-sharp-faster-alternatives-to-setpixel-and-getpixel-for-bitmaps-for-windows-f
    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color colour)
        {
            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }


    // https://stackoverflow.com/questions/28310592/viewing-a-large-bitmap-image-using-memory-mapped-view
    // https://msdn.microsoft.com/en-us/library/dd997372%28v=vs.110%29.aspx
    // https://msdn.microsoft.com/en-us/library/dd988186(v=vs.110).aspx
    // http://visualstudiomagazine.com/articles/2010/06/23/memory-mapped-files.aspx?m=1
    // https://stackoverflow.com/questions/11065689/processing-on-large-bitmaps-up-to-3gb?lq=1
    // https://stackoverflow.com/questions/569889/how-do-i-use-large-bitmaps-in-net

    class BMPMMF
    {
        /// <summary>
        /// It opens the image using memory mapped view and read the needed 
        /// parts, then call CreateBM to create a partially bitmap
        /// </summary>
        /// <param name="bmpFilename">Path to the physical bitmap</param>
        /// <returns></returns>
        public Bitmap readPartOfImage(string bmpFilename)
        {
            var headers = ReadHeaders(bmpFilename);
            var mmf = MemoryMappedFile.CreateFromFile(bmpFilename, FileMode.Open);
            int rowSize = headers.Item2.RowSize;    // number of byes in a row

            // Dictionary<ColorObject, int> rowColors = new Dictionary<ColorObject, int>();

            int colorSize = Marshal.SizeOf(typeof(MyColor));
            int width = rowSize / colorSize;//(headers.Item1.DataOffset+ rowSize) / colorSize;
            int height = 200;
            ColorObject cObj;
            MyColor outObj;
            ColorObject[][] rowColors = new ColorObject[height][];
            // Read the view image and save row by row pixel
            for (int j = 0; j < height; j++)
            {
                rowColors[j] = new ColorObject[width];
                using (var view = mmf.CreateViewAccessor(headers.Item1.DataOffset + rowSize * j, rowSize, MemoryMappedFileAccess.Read))
                {
                    for (long i = 0; i < rowSize; i += colorSize)
                    {

                        view.Read(i, out outObj);
                        cObj = new ColorObject(outObj);
                        rowColors[j][i / colorSize] = cObj;


                    }
                }
            }
            return CreateBM(rowColors);

        }
        /// <summary>
        /// Used to create a bitmap from provieded bytes 
        /// </summary>
        /// <param name="rowColors">Contains bytes of bitmap</param>
        /// <returns></returns>
        private Bitmap CreateBM(ColorObject[][] rowColors)
        {
            int width = rowColors[0].Count();
            int height = rowColors.Count();
            //int width = rowColors.Values.Where(o => o == 0).Count();
            Bitmap bitm = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            // new Bitmap(imgdat.GetUpperBound(1) + 1, imgdat.GetUpperBound(0) + 1, PixelFormat.Format24bppRgb);
            BitmapData bitmapdat = bitm.LockBits(new Rectangle(0, 0, bitm.Width, bitm.Height), ImageLockMode.ReadWrite, bitm.PixelFormat);
            int stride = bitmapdat.Stride;
            byte[] bytes = new byte[stride * bitm.Height];

            for (int r = 0; r < bitm.Height; r++)
            {

                for (int c = 0; c < bitm.Width; c++)
                {
                    ColorObject color = rowColors[r][c];
                    bytes[(r * stride) + c * 3] = color.Blue;
                    bytes[(r * stride) + c * 3 + 1] = color.Green;
                    bytes[(r * stride) + c * 3 + 2] = color.Red;

                }
            }


            System.IntPtr scan0 = bitmapdat.Scan0;
            Marshal.Copy(bytes, 0, scan0, stride * bitm.Height);
            bitm.UnlockBits(bitmapdat);

            return bitm;
        }

        /// <summary>
        /// Returns a tuple that contains necessary information about bitmap header 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private Tuple<BmpHeader, DibHeader> ReadHeaders(string filename)
        {
            var bmpHeader = new BmpHeader();
            var dibHeader = new DibHeader();
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    bmpHeader.MagicNumber = br.ReadInt16();
                    bmpHeader.Filesize = br.ReadInt32();
                    bmpHeader.Reserved1 = br.ReadInt16();
                    bmpHeader.Reserved2 = br.ReadInt16();
                    bmpHeader.DataOffset = br.ReadInt32();

                    dibHeader.HeaderSize = br.ReadInt32();
                    if (dibHeader.HeaderSize != 40)
                    {
                        throw new ApplicationException("Only Windows V3 format supported.");
                    }
                    dibHeader.Width = br.ReadInt32();
                    dibHeader.Height = br.ReadInt32();
                    dibHeader.ColorPlanes = br.ReadInt16();
                    dibHeader.Bpp = br.ReadInt16();
                    dibHeader.CompressionMethod = br.ReadInt32();
                    dibHeader.ImageDataSize = br.ReadInt32();
                    dibHeader.HorizontalResolution = br.ReadInt32();
                    dibHeader.VerticalResolution = br.ReadInt32();
                    dibHeader.NumberOfColors = br.ReadInt32();
                    dibHeader.NumberImportantColors = br.ReadInt32();
                }
            }

            return Tuple.Create(bmpHeader, dibHeader);
        }
    }

    public struct MyColor
    {
        public byte Alpha;
        public byte Red;
        public byte Green;
        public byte Blue;
    }
    public class ColorObject
    {
        public ColorObject(MyColor c)
        {
            this.Alpha = c.Alpha;
            this.Red = c.Red;
            this.Green = c.Green;
            this.Blue = c.Blue;
        }
        public byte Alpha;
        public byte Red;
        public byte Green;
        public byte Blue;
    }

    public class BmpHeader
    {
        public short MagicNumber { get; set; }
        public int Filesize { get; set; }
        public short Reserved1 { get; set; }
        public short Reserved2 { get; set; }
        public int DataOffset { get; set; }
    }

    public class DibHeader
    {
        public int HeaderSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public short ColorPlanes { get; set; }
        public short Bpp { get; set; }
        public int CompressionMethod { get; set; }
        public int ImageDataSize { get; set; }
        public int HorizontalResolution { get; set; }
        public int VerticalResolution { get; set; }
        public int NumberOfColors { get; set; }
        public int NumberImportantColors { get; set; }
        public int RowSize
        {
            get
            {
                return (Bpp / 8 * Width) + Width % 4; // (bytes * pixel width) = ((24/8) *99) or 3*99 or 297 mod 4 = 1 
            }
        }
    }


}
