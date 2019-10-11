using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
//using System.Drawing;
using System.IO;
using System.Linq;
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
                Filter = "Image Files (*.bmp or *.bmp)|*.bmp;*.bmp"
            };
            ofd.ShowDialog();
            FullFileName = ofd.FileName;
            return FullFileName;
        }

        public Bitmap GetImage(string filename)
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
            System.Drawing.Imaging.PixelFormat bits = bitmap.PixelFormat;

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
}
