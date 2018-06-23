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
        //public Image<Rgba, Byte> blueThres;
       // public Image<Rgba, Byte> greenThres;

        public Image<Rgb, Byte>[] Divs;
       // public Image<Rgb, Byte> blueDiv;
       // public Image<Rgb, Byte> greenDiv;

        public BasicInfo BInfo;
        public Directory dir;
        public string path;

        public Img()
        {
            Thres = new Image<Rgba, byte>[4];
            Divs = new Image<Rgb, byte>[3];
            RGB = new Image<Rgba, byte>[4];
            detect = new Detector();
        }
        public void GetDetection(ref Image<Rgb, byte> aux, int what)
        {
            detect.Detect(ref aux, ref args, what);
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

        public void Threshold(double d, double max, int channel)
        {
            Rgba r = PureColor(channel, d);
            Rgba g = PureColor(channel, max);
            Thres[channel] = RGB[channel].ThresholdBinary(r, g);
        }

        public void Divide()
        {
            for (int i = 0; i < 3; i++)
            {
                Divs[i] = escaledUI.Convert<Rgb, byte>().Mul(Thres[i].Convert<Rgb, byte>().Pow(-1));
            }
           
        }
       

    

    }

   
}
