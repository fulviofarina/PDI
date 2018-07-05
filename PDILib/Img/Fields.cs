using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace PDILib
{


    public partial class Img
    {


       

        public double[] args;

        public Detector detect;
        public string curentfilename;

        public Image<Rgba, Byte> original;
        public Image<Rgba, Byte> UIOne;
        public Image<Rgba, Byte>[] RGB;

        public void Dispose()
        {
            original.Dispose();
            UIOne.Dispose();
            foreach (var item in RGB)
            {
                item?.Dispose();
            }
            foreach (var item in Thres)
            {
                item?.Dispose();
            }
            foreach (var item in Soby)
            {
                item?.Dispose();
            }
            foreach (var item in ElementSubs)
            {
                item?.Dispose();
            }
            foreach (var item in Divs)
            {
                item?.Dispose();
            }
            originalTwo?.Dispose();
            UITwo?.Dispose();

            this.BInfo?.Dispose();
            this.detect?.Dispose();

        }

        //  public Image<Rgba, Byte> blue;
        //  public Image<Rgba, Byte> green;
        //  public Image<Rgba, Byte> alpha;

        public Image<Rgba, Byte>[] Thres;
        public Image<Rgba, Byte>[] Soby;

        public string curentfilenameToCompare;
        public Image<Rgba, byte> originalTwo;
        public Image<Rgba, byte> UITwo;

        public Image<Rgba, Byte>[] ElementSubs;
        //public Image<Rgba, Byte> blueThres;
        // public Image<Rgba, Byte> greenThres;

        public Image<Rgb, Byte>[] Divs;
        // public Image<Rgb, Byte> blueDiv;
        // public Image<Rgb, Byte> greenDiv;

        public BasicInfo BInfo;
        public Directory dir;
        public string path;
       
       
        public ImgUtil imgUtil;

        public Img()
        {
            CvInvokePreprocess();

            Thres = new Image<Rgba, byte>[4];
            Soby = new Image<Rgba, byte>[4];
            ElementSubs = new Image<Rgba, byte>[4];
            Divs = new Image<Rgb, byte>[3];
            RGB = new Image<Rgba, byte>[4];

            imgUtil = new ImgUtil();
            detect = new Detector();


         

        }

        public static void CvInvokePreprocess()
        {
            CvInvoke.NumThreads = 1000;
            CvInvoke.UseOpenCL = true;
            CvInvoke.UseOptimized = true;
        }


    }

   


}
