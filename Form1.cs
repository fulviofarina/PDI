using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;

namespace PDI
{

    public partial class Form1 : Form
    {
        Img imagen = new Img();

        public Form1()
        {
            InitializeComponent();

            GetFiles();

        }

        private void GetFiles()
        {
            imagen.path = browseBox.Text; // "D:\\USBGDrive\\MAESTRIA\\LTER - Moorea";
            imagen.GetFiles();

            filesBS.DataSource = imagen.dir.table;

           
        }

        private void getRGB_Click(object sender, EventArgs e)
        {

            //  img = img.SmoothBlur(1,1);
            // img= img.Sobel(1, 1, 1).Convert<Rgba,Byte>();
            //  Image<Rgba, byte> redImg = imagen.GetRed();

            imagen.GetChannel(0);
            imagen.GetChannel(1);
            imagen.GetChannel(2);
            imagen.GetChannel(3);
            int val = Convert.ToInt32(numericUpDown1.Value);
            rgbBox.Image = imagen.RGB[val].Bitmap;
          

        }



        private void threshold_Click(object sender, EventArgs e)
        {
            //  Bitmap bm = new Bitmap(pictureBox2.Image);
            //= new Image<Rgba, Byte>(bm);

            double d = Convert.ToDouble(textBox1.Text);
            double max = Convert.ToDouble(textBox2.Text);

            imagen.Threshold(d, max);
            int val = Convert.ToInt32(numericUpDown1.Value);
            rgbBox.Image = imagen.Thres[val].Bitmap;


            // redImg2 = imagen.Thres[0].Convert<Rgb, byte>();
            // redImg2 = redImg2.Sobel(1, 1, 13).Convert<Rgb, byte>();
            lastBitMap = imagen.Thres[val].Bitmap;

            segmentBox.Image = lastBitMap;
         


            //   pictureBox1.Image = redImg.ToBitmap();
        }



        private void divide_Click(object sender, EventArgs e)
        {

            Bitmap bm = new Bitmap(segmentBox.Image);
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(bm);

            imagen.Divide();
            int val = Convert.ToInt32(numericUpDown1.Value);
            redImg2 = imagen.Divs[val];

            lastBitMap = redImg2.Bitmap;

            segmentBox.Image = lastBitMap;
          
        }
        Bitmap lastBitMap = null;
        private void segment_Click(object sender, EventArgs e)
        {

           

            //line
            double cth = Convert.ToDouble(cannyThreshold.Text);
            double cthlink = Convert.ToDouble(cannyThreshLinking.Text);
            //circle
            double accum = Convert.ToDouble(cannyCircleAccum.Text);
            double dp = Convert.ToDouble(dpbox.Text);
            double minDist = Convert.ToDouble(minDistbox.Text);
            double minRadio = Convert.ToDouble(minRadiobox.Text);
            //lines
            double rho = Convert.ToDouble(rhoBox.Text);
            double threshold = Convert.ToDouble(thresholdbox.Text);
            double minWithLine = Convert.ToDouble(minWidthbox.Text);
            double gap = Convert.ToDouble(gapbox.Text);
            //trianRect
            double epsilonFact = Convert.ToDouble(epsilonBox.Text);
            double minArea = Convert.ToDouble(minAreabox.Text);

            imagen.args = new double[] { cth, cthlink,
                accum, dp,
                minDist, minRadio, rho,
                threshold, minWithLine,gap ,
                epsilonFact , minArea };

           


        }



        private static void WrongHough(Image<Bgr, byte> redImg2)
        {
            //DenseHistogram h = new DenseHistogram(1, new RangeF(0, 255));
            LineSegment2D[][] f = redImg2.HoughLines(255, 1, redImg2.Width, 1, 1, 2, 1);

            HashSet<LineSegment2D> hs = new HashSet<LineSegment2D>();

            for (int j = 0; j < f.Length; j++)
            {
                for (int i = 0; i < f[j].Length; i++)
                {
                    LineSegment2D seg = f[j][i];
                    if (seg.Length >= 10)
                    {
                        hs.Add(seg);

                        DrawLines(ref redImg2, seg);
                        // redImg2.Bitmap.SetPixel(seg.P2.X, seg.P2.Y, Color.Black);
                        // redImg2.Bitmap.SetPixel(seg.P1.X, seg.P1.Y, Color.Black);
                    }

                }
            }
        }

        private static void DrawLines(ref Image<Bgr, byte> redImg2, LineSegment2D seg)
        {
            if (seg.P2.X - seg.P1.X != 0)
            {
                double slope = (seg.P2.Y - seg.P1.Y);
                slope /= (seg.P2.X - seg.P1.X);
                double cutoff = seg.P1.Y - (slope * seg.P1.X);
                double cutoff2 = seg.P2.Y - (slope * seg.P2.X);

                int x = seg.P1.X;


                while (x <= seg.P2.X)
                {
                    int y = Convert.ToInt32(slope * x + cutoff);
                    redImg2.Bitmap.SetPixel(x, y, Color.Black);
                    //redImg2.Bitmap.SetPixel(seg.P1.X, seg.P1.Y, Color.Black);
                    x++;
                }
            }
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
          DialogResult r =   this.folderBrowserDialog1.ShowDialog();
           if( r == DialogResult.OK)
            {
                browseBox.Text = this.folderBrowserDialog1.SelectedPath;
                GetFiles();
            }
        }

        private void filesBS_PositionChanged(object sender, EventArgs e)
        {
            DataRowView drv = (DataRowView)filesBS.Current;
            ImgDB.FilesRow first = (ImgDB.FilesRow)drv.Row;
            string filename = first.Path + first.Filename;

            originalBox.Image = imagen.GetBitMap(filename, 5);
            imagen.GetBasicInfo();
            basicInfoBindingSource.DataSource = imagen.BInfo.table;

            getRGB_Click(sender, e);
            threshold_Click(sender, e);

        }

        private void figureClick(object sender, EventArgs e)
        {
            if (lastBitMap == null) return;
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(lastBitMap);
            Button btn = (Button) sender;
            int what = Convert.ToInt32(btn.Tag);
            imagen.GetDetection(ref redImg2,what);


            Rgb std = new Rgb(0, 255, 0);
            Rgb std2 = std;
          
            if (what == 1) std = new Rgb(200, 0, 255);
            else if (what == 3)
            {
                std = new Rgb(0, 0, 255);
                std2 = new Rgb(100, 200, 255);
            }

             if (what==1) imagen.detect.DrawCircles(std);
             else if (what ==2)  imagen.detect.DrawLines(std);
             else if (what ==3) imagen.detect.DrawTriRect(std,std2);

            // segmentBox.Image = imagen.detect.circleImage.Bitmap;
            richTextBox1.Text = imagen.detect.msgBuilder.ToString();
            segmentBox.Image = imagen.detect.figure[what].Bitmap;



        }

       

        private void erodeBtn_Click(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(segmentBox.Image);
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(bm);

            
            redImg2 = redImg2.Erode(Convert.ToInt32(erodeBox.Text));




            segmentBox.Image = redImg2.Bitmap;
        }

        private void dilateBtn_Click(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(segmentBox.Image);
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(bm);


            redImg2 = redImg2.Dilate(Convert.ToInt32(dilatebox.Text));




            segmentBox.Image = redImg2.Bitmap;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            getRGB_Click(sender, e);
           // threshold_Click(sender, e);
        }

        double factor = 0.7;
        double factorDiago = 0.98;

        private void iterateBtn_Click(object sender, EventArgs e)
        {

           


            lastBitMap = imagen.escaledUI.CopyBlank().Bitmap;

            numericUpDown1.Value = 0;
           
            ExtractLines(sender, e);
           
            //MessageBox.Show("0");

            numericUpDown1.Value = 1;
            ExtractLines(sender, e);

            // MessageBox.Show("1");

            numericUpDown1.Value = 2;
            ExtractLines(sender, e);

            MessageBox.Show("0");

            factor = 0.1;

            Image<Rgb, byte> result = null;

            numericUpDown1.Value = 0;

            ExtractLines(sender, e);

            //MessageBox.Show("0");

            numericUpDown1.Value = 1;
            ExtractLines(sender, e);

            // MessageBox.Show("1");

            numericUpDown1.Value = 2;
            ExtractLines(sender, e);


            /*
            // MessageBox.Show(arrRed.Count() + "," + arrGreen.Count() + "," + arrBlue.Count());

           
            imagen.detect.figure[2] = imagen.detect.figure[2].CopyBlank();
            LineSegment2D[] bg;// = arrBlue.Intersect(arrGreen).ToArray();
            LineSegment2D[] ag;//= arrRed.Intersect(arrGreen).ToArray();
            LineSegment2D[] ab;// = arrRed.Intersect(arrBlue).ToArray();
            //  imagen.detect.lines = bg.Union(ag).Union(ab).ToArray();

            bg = imagen.detect.GetLinesByDirection(0, 1, 0).ToArray();
            ag = imagen.detect.GetLinesByDirection(1, 1, 0).ToArray();
            ab = imagen.detect.GetLinesByDirection(2, 1, 0).ToArray();


            DrawRGBLines(bg, ag, ab);

            
            result = imagen.detect.figure[2].Add(result).Clone();

            imagen.detect.figure[2] = imagen.detect.figure[2].CopyBlank();

            bg = imagen.detect.GetLinesByDirection(0, 0, 1).ToArray();
            ag = imagen.detect.GetLinesByDirection(1, 0, 1).ToArray();
            ab = imagen.detect.GetLinesByDirection(2, 0, 1).ToArray();


            DrawRGBLines(bg, ag, ab);

            */





            // result = imagen.detect.figure[2].Add(result).Clone();



            /*
            Func<byte,int,int,byte> action = (o,x,y) =>
            { int val = Convert.ToInt32(o);
                if (val < 250 && val > 0)
                {
                    o = Convert.ToByte(0);
                }
                else if (val != 0)
                {

                }
                return o;
            };
           clone.Convert(action);//new Rgb(250,250,250), new Rgb(255,255,255));
           // ConvolutionKernelF kern = new ConvolutionKernelF(new float[,] { { 0, -1, 0 }, { 0, -1, 0 }, {0, -1, 0}  });
           // clone = clone.Convolution(kern).Convert<Rgb,byte>();
           */
            // MessageBox.Show("c");

            // segmentBox.Image = result.Bitmap;

            /*
            segmentBox.Image = result.Bitmap;

            MessageBox.Show("c");
            result = result.InRange(new Rgb(Color.White), new Rgb(Color.White)).Convert<Rgb, byte>();


            segmentBox.Image = result.Bitmap;

            //MessageBox.Show("c");

            //Teste(result);
            */
        }

        private void ExtractLines(object sender, EventArgs e)
        {
           
            Image<Rgb, byte> result = new Image<Rgb, Byte>(lastBitMap);


            int val = Convert.ToInt16(numericUpDown1.Value);
            IterateAChannel(sender, e);
            LineSegment2D[] arrRed = imagen.detect.ExtractAvgLines(factor, val);
            result = imagen.detect.DrawLines(val, ref arrRed).Add(result).Clone();
            segmentBox.Image = result.Bitmap;

            LineSegment2D[] diag;
            diag = imagen.detect.ExtractAvgDiagonalLines(factorDiago, val);
            result = imagen.detect.DrawLines(val, ref diag).Add(result).Clone();
            lastBitMap = result.Bitmap;
            segmentBox.Image = result.Bitmap;
        }

        private void Teste(Image<Rgb, byte> result)
        {
            // double[] minVals;
            // double[] maxVals;
            // Point[] minLoc;
            // Point[] maxLoc;
            //MCvMoments moments = result.GetMoments(true);
            // result.Sample()
            //result.DrawPolyline(out minVals, out maxVals, out minLoc, out maxLoc);
            List<Point> ints = new List<Point>();
            List<LineSegment2D> counts = new List<LineSegment2D>();

            /*
            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Cols; j++)
                {
                    LineSegment2D s = new LineSegment2D(new Point(i, 0), new Point(i, result.Height));
                    byte[,] b = result.Sample(s);
                    IEnumerable<byte> by = b.Cast<byte>().Where(o => Convert.ToInt16(o) == 255);
                    int c = by.Count();
                    if (c > 15)
                    {
                        ints.Add(i);
                        counts.Add(s);

                    }
                }
            }
            */
            Image<Gray, byte> r = result.Split()[0];
            for (int i = 0; i < r.Rows; i++)
            {
                for (int j = 0; j < r.Cols; j++)
                {
                    if (r[i, j].Intensity == 255)
                    {

                        ints.Add(new Point(i, j));

                    }
                }
            }
            List<Point> intsleft = ints.Where(o => o.X <= result.Width / 2 && o.X > 10 && o.X < 40).ToList();
            List<Point> intsright = ints.Where(o => o.X >= result.Width / 2 && o.X < result.Width - 10 && o.X > result.Width - 40).ToList();
            List<Point> total = intsleft.Union(intsright).ToList();

            foreach (Point p in total)
            {

                LineSegment2D s = new LineSegment2D(new Point(p.X, 0), new Point(p.X, result.Height));
                counts.Add(s);
            }
            imagen.detect.lines = counts.ToArray();
            imagen.detect.figure[2] = imagen.detect.figure[2].CopyBlank();
            imagen.detect.DrawLines(new Rgb(255, 255, 255));
            segmentBox.Image = imagen.detect.figure[2].Bitmap;

            lastBitMap = imagen.detect.figure[2].Bitmap;
        }




        private void DrawRGBLines(LineSegment2D[] bg, LineSegment2D[] ag, LineSegment2D[] ab)
        {
            Rgb color;
            imagen.detect.lines = bg;
            color = new Rgb(255, 0, 0);
            imagen.detect.DrawLines(color,true);

            imagen.detect.lines = ag;
            color = new Rgb(0, 255, 0);
            imagen.detect.DrawLines(color,true);

            imagen.detect.lines = ab;
            color = new Rgb(0, 0, 255);
            imagen.detect.DrawLines(color,true);
           
        }

        private void IterateAChannel(object sender, EventArgs e)
        {
            double d = 10;
            double max = 250;

            List<LineSegment2D[]> ls = new List<LineSegment2D[]>();

            for (double val = d; val <= max; val += 5)
            {
                textBox1.Text = val.ToString();
                threshold_Click(sender, e);
                thresholdbox.Text = val.ToString();
                segment_Click(sender, e);
                figureClick(linesBtn, e);
                ls.Add(imagen.detect.lines);
             
            }
            int v = Convert.ToInt16(numericUpDown1.Value);
            imagen.detect.fullLines[v] = ls.SelectMany(o => o.ToList()).OrderBy(o => o.Length).ToArray();
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            long matchTime;
            using (Mat modelImage = CvInvoke.Imread(imagen.dir.files[0], ImreadModes.Grayscale))
            using (Mat observedImage = CvInvoke.Imread(imagen.dir.files[1], ImreadModes.Grayscale))
            {
                Mat result = DrawMatches.Draw(modelImage, observedImage, out matchTime);
                this.segmentBox.Image = new Bitmap(result.ToImage<Rgb,byte>().Bitmap, new Size(400,400));
            }
        }
    }

}
