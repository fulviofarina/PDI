using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;


namespace PDI
{
    public partial class Form1
    {

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

                        SlopeCutoof(ref redImg2, seg);
                        // redImg2.Bitmap.SetPixel(seg.P2.X, seg.P2.Y, Color.Black);
                        // redImg2.Bitmap.SetPixel(seg.P1.X, seg.P1.Y, Color.Black);
                    }

                }
            }
        }


        private void basura(object sender, EventArgs e)
        {
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

        /*
private void ExtractLines(object sender, EventArgs e)
{

   Image<Rgb, byte> result = new Image<Rgb, Byte>(lastBitMap);


   int channel = Convert.ToInt16(numericUpDown1.Value);

   IterateAChannel(sender, e);


   imagen.detect.GetAllUDLRLines(factor, channel, 4.5, true);
   imagen.detect.GetDiagonalsPosNeg(factorDiago, channel);



}
*/
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


        private void button2_Click(object sender, EventArgs e)
        {
            long matchTime;
            using (Mat modelImage = CvInvoke.Imread(imagen.dir.files[0], ImreadModes.Grayscale))
            using (Mat observedImage = CvInvoke.Imread(imagen.dir.files[1], ImreadModes.Grayscale))
            {
                Mat result = DrawMatches.Draw(modelImage, observedImage, out matchTime);
                this.segmentBox.Image = new Bitmap(result.ToImage<Rgb, byte>().Bitmap, new Size(400, 400));
            }
        }

        private void DrawRGBLines(LineSegment2D[] bg, LineSegment2D[] ag, LineSegment2D[] ab)
        {
            Rgb color;
            imagen.detect.lines = bg;
            color = new Rgb(255, 0, 0);
            imagen.detect.DrawLines(color, true);

            imagen.detect.lines = ag;
            color = new Rgb(0, 255, 0);
            imagen.detect.DrawLines(color, true);

            imagen.detect.lines = ab;
            color = new Rgb(0, 0, 255);
            imagen.detect.DrawLines(color, true);

        }

    }
}
