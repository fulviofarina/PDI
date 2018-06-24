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

        public void GetFiles()
        {
            dir = new Directory(path);
        }


        public void GetBasicInfo()
        {
            BInfo = new BasicInfo(ref original); //extra info
        }
        public Bitmap GetBitMap(string filename, int scale)
        {
            // Image i = Image.FromFile(filename, true);
            original = new Image<Rgba, Byte>(filename);
            int h = original.Size.Height;
            int w = original.Size.Width;

            escaledUI = original.Resize(w / scale, h / scale, Emgu.CV.CvEnum.Inter.LinearExact, true);
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
                if (i != index && i != 3)
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

        public void GetChannels()
        {
            GetChannel(0);
            GetChannel(1);
            GetChannel(2);
            GetChannel(3);
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
    }
     


}
