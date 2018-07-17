using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;

namespace PDILib
{




    public partial class Img
    {


        public void GetFiles()
        {
            dir = new Directory(path);
        }
      public   MCvMoments[] TwoMoments;
        public MCvMoments[] OneMoments;

        public void GetBasicInfo()
        {
            BInfo = new BasicInfo(ref original); //extra info
        }

        public void GetImgToCompare(string filename, int scale)
        {
            curentfilenameToCompare = filename;

            string fullpath = checkFilename(filename);
            originalTwo = new Image<Rgba, Byte>(fullpath);

            int h = original.Size.Height;
            int w = original.Size.Width;

            UITwo = originalTwo.Resize(w / scale, h / scale, Emgu.CV.CvEnum.Inter.Cubic, true)
                .SmoothGaussian(1);

            //   TwoMoments= CvInvoke.Moments(UITwo[3], true);

            //    SwitchColor(ref UITwo, imgUtil.pitchBlack, imgUtil.pitchWhite);
            //  SwitchColor(ref UIOne,  imgUtil.pitchBlack, imgUtil.pitchWhite);
            OneMoments = new MCvMoments[4];
            TwoMoments = new MCvMoments[4];

            for (int i = 0; i < 4; i++)
            {
                OneMoments[i] = CvInvoke.Moments(UIOne[i], false);
                TwoMoments[i] = CvInvoke.Moments(UITwo[i], false);

            }

         //   ExpandTwice();

        }

        public void ExpandTwice()
        {
            int j = 0;
            //take originals
            imgUtil.expandedOne[j] = UIOne;
            imgUtil.expandedTwo[j] = UITwo;
            firstExpansion(j);
            j++;
            secondExpansion(j);
            SwitchColor(ref imgUtil.expandedTwo[1], imgUtil.pitchWhite, imgUtil.pitchBlack);
            SwitchColor(ref imgUtil.expandedTwo[0], imgUtil.pitchWhite, imgUtil.pitchBlack);
            SwitchColor(ref imgUtil.expandedOne[1], imgUtil.pitchWhite, imgUtil.pitchBlack);
            SwitchColor(ref imgUtil.expandedOne[0], imgUtil.pitchWhite, imgUtil.pitchBlack);
        }

        private void secondExpansion(int j)
        {
            Image<Rgba, byte> expanded;
            Point middle;
            double raidus;

            expanded = MakeCanvas(ref imgUtil.expandedTwo[j - 1], out raidus);

            imgUtil.lastCanvas.Dispose();
            imgUtil.lastCanvas = expanded.Clone();
            middle = GetMiddlePointCanvas(raidus, ref imgUtil.expandedTwo[j - 1]);
            //testCanvas = expanded.CopyBlank();
            Copy(ref middle, ref expanded, ref imgUtil.expandedTwo[j - 1]);
            imgUtil.expandedTwo[j] = expanded.Clone();

            expanded.Dispose();

            //  testCanvas = expanded.CopyBlank();
            expanded = imgUtil.lastCanvas.Clone();

            middle = GetMiddlePointCanvas(raidus, ref imgUtil.expandedOne[j - 1]);
            //testCanvas = expanded.CopyBlank();
            Copy(ref middle, ref expanded, ref imgUtil.expandedOne[j - 1]);
            imgUtil.expandedOne[j] = expanded.Clone();

            expanded.Dispose();
            expanded = null;


        }

        private void firstExpansion(int j)
        {

            Image<Rgba, byte> bigger;
            Image<Rgba, byte> expanded;
            Point middle;
            double raidus;
            //get bigger image
            bigger = GetBiggerSmallerImages(ref imgUtil.expandedTwo[j], ref imgUtil.expandedOne[j])[0];
            expanded = MakeCanvas(ref bigger, out raidus);
            imgUtil.lastCanvas = expanded.Clone();


            middle = GetMiddlePointCanvas(raidus, ref imgUtil.expandedTwo[j]);
            Copy(ref middle, ref expanded, ref imgUtil.expandedTwo[j]);
            imgUtil.expandedTwo[j] = expanded.Clone();

            expanded.Dispose();

            expanded = imgUtil.lastCanvas.Clone();

            middle = GetMiddlePointCanvas(raidus, ref imgUtil.expandedOne[j]);
            Copy(ref middle, ref expanded, ref imgUtil.expandedOne[j]);
            imgUtil.expandedOne[j] = expanded.Clone();

            expanded.Dispose();
            expanded = null;
        }

      
        public Point GetMiddlePointCanvas(double radiusTestCanvas, ref Image<Rgba, byte> actual)
        {
          
            Point middle;
            double originx;
            originx = (2* radiusTestCanvas) - actual.Width;
            //  originx = (2 * radiusTestCanvas) - actual.Width;
              originx /= 2;
            double originy;
            originy = (2 * radiusTestCanvas) - actual.Height;
           originy /= 2;
            middle = new Point(Convert.ToInt32(originx),Convert.ToInt32( originy));
            return middle;
        }

        public Image<Rgba, byte> MakeCanvas(ref Image<Rgba, byte> actual, out double radius)
        {
            radius = CalculateDiagonalLenght(actual.Width, actual.Height);
            radius /= 2;
            int newW = Convert.ToInt32(radius);
            int newH = newW;
            newW *= 2;
            newH *= 2;

            //toma todo

            Image<Rgba, byte> expanded = new Image<Rgba, byte>(newW, newH, imgUtil.pitchBlack);
            return expanded;
        }


        public void GetImg(string filename, int scale)
        {
            curentfilename = filename;
            string fullpath = checkFilename(filename);
            original = new Image<Rgba, Byte>(fullpath);
            int h = original.Size.Height;
            int w = original.Size.Width;


            UIOne = original.Resize(w / scale, h / scale, Emgu.CV.CvEnum.Inter.Cubic, true)
                .SmoothGaussian(1);


         //   OneMoments = CvInvoke.Moments(UIOne[3], true);


            //BORRAR STO
            // GetImgToCompare(filename, scale);
            //  UIOne = imgUtil.expandedOne[0];

            //.Sobel(1,1,13).Convert<Rgba,byte>();
            // SwitchColor(ref escaledUI, 0, imgUtil.pitchWhite, new Rgba(0, 0, 0, 0));

        }

        private string checkFilename(string filename)
        {
            // Image i = Image.FromFile(filename, true);

            string fullpath = path + "\\" + filename;
            bool exist = System.IO.File.Exists(fullpath);
            if (!exist) throw new Exception("no image provided");
            return fullpath;
        }

        public Image<Rgba, byte> GetChannel(int index)
        {
            Image<Rgba, Byte> img = UIOne;
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


     
        public void GetChannels()
        {
            GetChannel(0);
            GetChannel(1);
            GetChannel(2);
            GetChannel(3);
        }


    }


  



}
