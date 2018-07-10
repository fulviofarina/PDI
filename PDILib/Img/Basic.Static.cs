using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PDILib
{




   


    public partial class Img
    {

        public static Image<Rgba, byte>[] GetBiggerSmallerImages(ref Image<Rgba, byte> one, ref Image<Rgba, byte> two)
        {
            Image<Rgba, byte> bigger = one;
            Image<Rgba, byte> smaller = two;
            bool lessW = (bigger.Width * bigger.Height) < (smaller.Width * smaller.Height);
         //   bool lessW = bigger.Width < smaller.Width;
          //  lessW = lessW && bigger.Height < smaller.Height;
            if (lessW)
            {
                bigger = two;
                smaller = one;
            }

            return new Image<Rgba, byte>[] { bigger, smaller };
        }
        public static int GetBiggerWidth(ref Image<Rgba, byte> one, ref Image<Rgba, byte> two)
        {
            int width = one.Width;
            bool lessW = (one.Width) < (two.Width);
            if (lessW)
            {
                width = two.Width;
            }

            return width;
        }
        public static int GetBiggerHeight(ref Image<Rgba, byte> one, ref Image<Rgba, byte> two)
        {
            int Height = one.Height;
            bool lessW = (one.Height) < (two.Height);
            if (lessW)
            {
                Height = two.Height;
            }

            return Height;
        }

        public static Rgba PureColor(int index, double valueToSet)
        {
            double[] vals = new double[4];
            for (int i = 0; i < 4; i++)
            {
                if (index != i) vals[i] = 255;
                else vals[i] = valueToSet;
            }
            return new Rgba(vals[0], vals[1], vals[2], vals[3]);
        }
        public static void FindAngle(LineSegment2D a, LineSegment2D b, double referenceAngle, out double angle, out string text)
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
        public static void DisposeArrayOfImages(ref List<object> alles)
        {

          
            if (alles != null)
            {
                List<Image<Rgba, byte>> all = alles.OfType<Image<Rgba, byte>>().ToList();
                foreach (var item in all)
                {
                    item.Dispose();
                }
                all.Clear();
                all = null;
            }
        }
        public static Image<Rgba, byte>[] GetOtheChannels(int channel, ref Image<Rgba, byte>[] array)
        {
            Image<Rgba, byte> current = array[channel];
            return array.Where(o => o != current).ToArray();
        }
        public static Image<Rgb, byte>[] GetOtheChannels(int channel, ref Image<Rgb, byte>[] array)
        {
            Image<Rgb, byte> current = array[channel];
            return array.Where(o => o != current).ToArray();
        }
        public static double CalculateDiagonalLenght(int widht, int height)
        {
            LineSegment2D p = new LineSegment2D(new Point(0, 0), new Point(widht, height));
            return p.Length;
        }
        public static void Copy(ref Point middle, ref Image<Rgba, byte> destiny, ref Image<Rgba, byte> source)
        {
            Rectangle rec = new Rectangle(middle.X, middle.Y, source.Width, source.Height);
            destiny.ROI = rec;
            source.CopyTo(destiny);
            destiny.ROI = Rectangle.Empty;
        }
        public static Image<Gray, float> MatchTemplate(ref Image<Rgba, byte> uno, ref Image<Rgba, byte> dos)
        {
            //  imagen.UITwo = imagen.UITwo.LogPolar(new PointF(x,y), 50, Emgu.CV.CvEnum.Inter.Cubic, Emgu.CV.CvEnum.Warp.Default);



            float x = (float)(uno.Width)/1.2f ;
            float y = (float)(uno.Height)/1.2f ;

            uno.ROI = new Rectangle(0, 0, (int)x, (int)y);
            Image<Gray, float> gray = dos.MatchTemplate(uno, Emgu.CV.CvEnum.TemplateMatchingType.Ccoeff);
            uno.ROI = Rectangle.Empty;


           // CvInvoke.Imshow("Gary", gray);

            return gray;
        }
        public static void SwitchColor(ref Image<Rgba,byte> source, Rgba compare, Rgba set)
        {
            for (int x = 0; x < source.Height; x++)
            {
                for (int y = 0; y < source.Width; y++)
                {
                    var pixel = source[x,y];
                    bool isRed = compare.Red == pixel.Red;
                    bool isBlue = compare.Blue == pixel.Blue;
                    bool isGreen = compare.Green == pixel.Green;
                    if (isRed && isBlue && isGreen)
                    {
                       source[x,y]= set;
                    }
                    
                }
            }
        }
        public static void ChangeColor(ref Image<Rgba, byte> source, float redF, float greenF, float blueF, float alphaF)
        {
            for (int x = 0; x < source.Height; x++)
            {
                for (int y = 0; y < source.Width; y++)
                {
                    var pixel = source[x, y];

                    source[x, y] = new Rgba(pixel.Red * redF, pixel.Green*greenF,pixel.Blue * blueF, pixel.Alpha * alphaF);

                }
            }
        }
    }



}
