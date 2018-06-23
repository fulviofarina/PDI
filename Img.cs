using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.Util;
using System.Collections;

namespace PDI
{
    

    public partial class Img
    {
        public double[] args;

        public Detector detect;

        public Image<Rgba, Byte> original;
       public  Image<Rgba, Byte> escaledUI;
       public  Image<Rgba, Byte>[] RGB;
      //  public Image<Rgba, Byte> blue;
      //  public Image<Rgba, Byte> green;
      //  public Image<Rgba, Byte> alpha;

        public Image<Rgba, Byte>[] Thres;


        public Image<Rgba, Byte>[] ElementSubs;
        //public Image<Rgba, Byte> blueThres;
        // public Image<Rgba, Byte> greenThres;

        public Image<Rgb, Byte>[] Divs;
       // public Image<Rgb, Byte> blueDiv;
       // public Image<Rgb, Byte> greenDiv;

        public BasicInfo BInfo;
        public Directory dir;
        public string path;
        public  Image<Rgba, byte> rotated;
      

        public Img()
        {
            Thres = new Image<Rgba, byte>[4];
            ElementSubs = new Image<Rgba, byte>[4];
            Divs = new Image<Rgb, byte>[3];
            RGB = new Image<Rgba, byte>[4];
            detect = new Detector();
           
        }
        public void BeginDetection(ref Image<Rgb, byte> aux, int what, bool draw = false)
        {
            detect.Detect(ref aux, ref args, what);

            if (draw) DrawDetected(what);
        }

        public void DrawDetected(int what)
        {
          
                Rgb std = new Rgb(0, 255, 0);
                Rgb std2 = std;

                if (what == 1) std = new Rgb(200, 0, 255);
                else if (what == 3)
                {
                    std = new Rgb(0, 0, 255);
                    std2 = new Rgb(100, 200, 255);
                }

                if (what == 1) detect.DrawCircles(std);
                else if (what == 2) detect.DrawLines(std);
                else if (what == 3) detect.DrawTriRect(std, std2);
           
        }

        public void GetFiles()
        {
            dir = new Directory(path);
        }

        
         public void GetBasicInfo()
        {
            BInfo = new BasicInfo(ref original); //extra info
        }
        public  Bitmap GetBitMap(string filename, int scale)
        {
            // Image i = Image.FromFile(filename, true);
            original = new Image<Rgba, Byte>(filename);
            int h = original.Size.Height;
            int w = original.Size.Width;

            escaledUI = original.Resize(w/scale, h/scale, Emgu.CV.CvEnum.Inter.LinearExact, true);
          //  Bitmap bm = new Bitmap(original.ToBitmap(),si);
          //  escaledUI = new Image<Rgba, byte>(bm); //salva

           

            //Image<Rgba, byte> img = new Image<Rgba, byte>(bm);
            // double x = 20;
            // img.Rotate(x, new Rgba(255, 255, 255, 255), false);
            return escaledUI.Bitmap;
        }
        public Image<Rgba, byte> GetChannel(int index)
        {
            Image<Rgba, Byte> img = escaledUI;
            Image<Gray, Byte>[] array = new Image<Gray, byte>[4];
            for (int i = 0; i < 4; i++)
            {
                if (i != index && i!=3)
                {
                    array[i] = new Image<Gray, Byte>(img.Width, img.Height);
                }
                else array[i] = img[i];
            }
            RGB[index] = new Image<Rgba, Byte>(array);
            return RGB[index];
        }

        public Rgba PureColor(int index, double valueToSet)
        {
            double[] vals = new double[4];
            for (int i = 0; i < 4; i++)
            {
                if (index != i) vals[i] = 255;
                else vals[i] = valueToSet;
            }
            return new Rgba(vals[0], vals[1], vals[2], vals[3]);
        }
        public void Threshold(double d, double max)
        {
           
            for (int i = 0; i < 4; i++)
            {
                Threshold(d, max, i);
            }
        }
        public void ElementSubstraction()
        {
            for (int i = 0; i < 4; i++)
            {
                ElementSubstraction(i);
            }
        }
            public void ElementSubstraction( int channel)
        {

            Image<Rgba, byte>[] otherChannels = RGB.Where(o => o != RGB[channel]).ToArray();
            ElementSubs[channel] = Thres[channel];
            foreach (Image<Rgba, byte> item in otherChannels)
            {
                ElementSubs[channel] = ElementSubs[channel].Add(item);

            }
        }
            public void Threshold(double d, double max, int channel)
        {
            Rgba r = PureColor(channel, d);
            Rgba g = PureColor(channel, max);
            Thres[channel] = RGB[channel].ThresholdToZeroInv(r);

            Rgba black = new Rgba(0, 0, 0, 0);
            Rgba set = new Rgba(255, 255, 255, 0);
            SwitchColor(channel, black, set);


            //  Thres[channel] = RGB[channel].ThresholdBinary(r,g);
            //   Thres[channel] = RGB[channel].Sub(Thres[channel]);
            //Thres[channel] = Thres[channel].AbsDiff(g);
            // Rgba r = PureColor(channel, d);
            //    Thres[channel] = Thres[channel].InRange(r, r).Convert<Rgba,byte>();

            //  Thres[channel] = RGB[channel].trunc(r);

            //   Thres[channel] = RGB[channel].ThresholdBinary(r, g);
            // Image<Gray, byte> Imae = RGB[channel].ThresholdBinary(r, g).Convert<Gray, byte>();
            //Image<Gray, byte>[] Imae2 = new Image<Gray, byte>[] { Imae, Imae, Imae, Imae };
            //hres[channel] = new Image<Rgba,byte>(Imae2);
            // 
            //Thres[channel] = RGB[channel].ThresholdToZeroInv(r);
        }
        public void FindAngle(LineSegment2D a, LineSegment2D b, double referenceAngle, out double angle, out string text)
        {
            double slopeA = a.Direction.Y / a.Direction.X;
            double slopeB = b.Direction.Y / b.Direction.X;

            double tan = (slopeA - slopeB);
            tan /= 1 + (slopeB * slopeA);
            tan = Math.Atan(tan);
            tan *= 180;
            tan /= Math.PI;

           // angle = tan;
            //imagen.detect.avgDiagonalNeg
            angle = (tan - referenceAngle);
            text = "\nAngle: " + tan.ToString();
            text += "\tDiff: " + angle.ToString();
            text += "\n";
        }
        private void SwitchColor(int channel, Rgba compare, Rgba set)
        {
            for (int x = 0; x < Thres[channel].Height; x++)
            {
                for (int y = 0; y < Thres[channel].Width; y++)
                {
                    var pixel = Thres[channel][x, y];
                    bool isRed = compare.Red == pixel.Red;
                    bool isBlue = compare.Blue == pixel.Blue;
                    bool isGreen = compare.Green == pixel.Green;
                    if (isRed && isBlue && isGreen)
                    {
                        Thres[channel][x, y] = set;
                    }
                    // Color.White;
                }
            }
        }

        public void ThresholdClassic(double d, double max, int channel)
        {
            Rgba r = PureColor(channel, d);
            Rgba g = PureColor(channel, max);
            Thres[channel] = RGB[channel].ThresholdBinary(r, g);
           // Image<Gray, byte> Imae = RGB[channel].ThresholdBinary(r, g).Convert<Gray, byte>();
           // Image<Gray, byte>[] Imae2 = new Image<Gray, byte>[] { Imae, Imae, Imae, Imae };
           // Thres[channel] = new Image<Rgba, byte>(Imae2);
            //  Thres[channel] = RGB[channel].ThresholdToZero(r);
            //Thres[channel] = RGB[channel].ThresholdToZeroInv(r);
        }

        public void Divide()
        {
            for (int i = 0; i < 3; i++)
            {
                Divide(i);
            }

        }

        public void Divide(int channel)
        {
            Image<Rgba,byte> input2 = Thres[channel];
            Image<Rgba, byte> input = escaledUI;

           // Divs[channel] = input.Mul(input2.Pow(-1)).Convert<Rgb, byte>();

            Divs[channel] = input2.Sub(input).Not().Convert<Rgb, byte>();

        }

        internal Image<Rgb, byte>[] DrawDetectedAvg(ref Image<Rgb, byte> final, ref Rgb[] color, bool append = true)
        {
            detect.figure[2] = final.Clone();


            detect.lines = detect.GetAvgUDLR(true);
            Image<Rgb, byte> nuevoResult0 = detect.DrawLines(color[0], append);
            ////////////////////////////////////////////////////////////
            detect.GetAvgDiagonalsPosNeg(true);
            ///print red DIAGONALSS //////////////
           // imagen.detect.lines = new LineSegment2D[] { imagen.detect.avgDiagonalPosCh[0] };
            detect.lines = new LineSegment2D[] { detect.avgDiagonalPos };
            Image<Rgb, byte> nuevoResult1 = detect.DrawLines(color[1], append);
            //
            // imagen.detect.lines = new LineSegment2D[] { imagen.detect.avgDiagonalPosCh[1] };
            detect.lines = new LineSegment2D[] { detect.avgDiagonalNeg };
            Image<Rgb, byte> nuevoResult2 = detect.DrawLines(color[2], append);


            return new Image<Rgb, byte>[] { nuevoResult0, nuevoResult1, nuevoResult2 };
        }
    }

   
}
