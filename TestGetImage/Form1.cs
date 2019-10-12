using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestGetImage
{
    public partial class Form1 : Form
    {
        MacGetImage.GetImageClass gi;
        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
            this.FormClosing += Form1_FormClosing;
            //this.Paint += Form1_Paint;
        }

        //private void Form1_Paint(object sender, PaintEventArgs e)
        //{
        //    string ss = "gi is null";
        //    if (gi != null)
        //    {
        //        ss = "gi.mbm is null";
        //        if (gi.mbm != null)
        //        {
        //            ss = "gi.mbm is not null";
        //            //e.Graphics.DrawImage(gi.mbm, 130, 60);
        //        }
        //    }

        //    Label1.Text = ss;
        //}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //gi.Dispose();
            //if (gi != null) { gi = null; }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.button1.Click += Button1_Click;
            this.button2.Click += Button2_Click;
            this.button3.Click += Button3_Click;
            this.Resize += Form1_Resize;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Bitmap bm = gi.ResizeImage(gi.MacBM, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bm;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            //gi.Dispose();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (gi != null)
            {
                if (gi.FullFileName != "")
                {
                    if (gi.LoadBitmapFromFile(gi.FullFileName) != null)
                    {
                        Bitmap bm = gi.ResizeImage(gi.MacBM, pictureBox1.Width, pictureBox1.Height);
                        pictureBox1.Image = bm;
                        //pictureBox1.Invalidate();
                        //pictureBox1.Refresh();
                    }
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (gi == null) gi = new MacGetImage.GetImageClass();
            gi.FindFile();
            textBox1.Text = gi.FullFileName;
        }

    }
}
