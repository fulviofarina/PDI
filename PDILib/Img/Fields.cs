using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        public static byte[] ExtractImage(ref object o, string texto)
        {
            Mat mat = o as Mat;
            Image<Rgba, byte> im = mat.ToImage<Rgba, byte>();
            
            MCvScalar font = new MCvScalar(255, 255, 0, 0);
            CvInvoke.PutText(im, texto, new Point(0, 50), Emgu.CV.CvEnum.FontFace.HersheyPlain, 3, font, 2);
            im = im.Resize(mat.Width / 3, mat.Height / 3, Emgu.CV.CvEnum.Inter.Cubic);
            byte[] b = im.ToJpegData();

            mat?.Dispose();
            return b;
        }

        public static void CvInvokePreprocess()
        {
            CvInvoke.SetNumThreads(1000);
            CvInvoke.UseOpenCL = true;
            CvInvoke.UseOptimized = true;

            bool hasCuda = CudaInvoke.HasCuda;
            bool hasCL = CvInvoke.HaveOpenCL;
         //   Emgu.CV.Cuda.CudaDeviceInfo i = new CudaDeviceInfo();
            
            bool okGPU = CvInvoke.HaveOpenCLCompatibleGpuDevice;
        }

        public static void Concatenate(string title, ref List<object> allImages)
        {

            List<Image<Rgba, byte>> all = allImages.OfType< Image<Rgba, byte>>().ToList();


            List<Image<Rgba, byte>> newArr = new List<Image<Rgba, byte>>();
            Image<Rgba, byte> current = null;



            int changeEvery = 3;
             int  horizontalCounter = 1;

            foreach (var item in all)
            {
                if (current == null)
                {
                    current = item;
                }
                else
                {
                    if (horizontalCounter <= changeEvery)
                    {
                        current = current.ConcateHorizontal(item);
                        horizontalCounter++;
                        if (horizontalCounter == changeEvery + 1)
                        {
                            horizontalCounter = 1;
                            newArr.Add(current.Clone());
                            current = null;
                        }

                    }

                }
            }



            foreach (var item in newArr)
            {
                if (current == null)
                {
                    current = item;
                }
                else
                {
                    current = current.ConcateVertical(item);
                }
            }





            if (current != null)
            {
                current = current.Resize(0.60, Emgu.CV.CvEnum.Inter.Cubic);

                CvInvoke.Imshow(title, current.Clone());

                current.Dispose();
            }
         

            Img.DisposeArrayOfImages(ref allImages);

            allImages = newArr.Cast<object>().ToList();

            Img.DisposeArrayOfImages(ref allImages);


        }
    }

   


}
