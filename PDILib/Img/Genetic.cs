using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using Emgu.CV.Cuda;
using System.Collections.Generic;

namespace PDILib
{




    public partial class Img
    {
        public object[] PerformRotationCompareFUNCIONA(double angulo, double scalex, double scaley, int tx, int ty, double skX = 0, double skY = 0)
        {


            Image<Rgba, byte> two = null;
            Image<Rgba, byte> one = null;
            Mat resultMat = new Mat();
            Mat destiny = null;
            Mat compare = null;
            object[] array = null;
            double raidus = 0;
            object suma = null;
            object count = null;
         //   bool firstStepOk = false;

            try
            {


                //VOLVI A CAMBIARLOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO////////////////////////
                // one = imgUtil.expandedOne[0];
                one = imgUtil.expandedOne[0];
                two = UITwo;
                //     one = UIOne;

                performRotateScale(angulo, scalex, scaley, ref one);
                performSkew(skX, skY, ref one);


                //determine bigger between one and two
                int width = GetBiggerWidth(ref two, ref one);
                int height = GetBiggerHeight(ref two, ref one);

                Image<Rgba, byte> canvas = new Image<Rgba, byte>(width, height, imgUtil.pitchBlack);
                //  Image<Rgba, byte>[] biggerSmaller = GetBiggerSmallerImages(ref UITwo, ref one);

                //make canvas from bigger and copy the smaller into this canvas
                //int area = (biggerSmaller[0]).Height * (biggerSmaller[0]).Width;

                two = MakeCanvas(ref canvas, out raidus);

                canvas.Dispose();
                ///CAMBIE ESTO
                Point middle = GetMiddlePointCanvas(raidus, ref one);
                //  Point original = new Point(middle.X, middle.Y);

                middle.X = tx;
                middle.Y = ty;
                Copy(ref middle, ref two, ref one);
                //get Mat
                destiny = two.Mat.Clone();


               // firstStepOk = true;

            }
            catch (Exception ex)
            {


            }


            one?.Dispose();


            try
            {

                one = UITwo;
                two = two.CopyBlank();
                //make canvas from bigger again
                // Copy the bigger into the canvas
                Point middle = GetMiddlePointCanvas(raidus, ref one);
                Copy(ref middle, ref two, ref one);


                compare = two.Mat.Clone();
                two.Dispose();

                object result = null;


                CvInvoke.BitwiseXor(compare, destiny, resultMat, null);// Emgu.CV.CvEnum.CmpType.Equal);
                                                                       //  CvInvoke.Subtract(compare, destiny, resultMat);
                                                                       //  CvInvoke.BitwiseAnd(compare, destiny, resultMat, null);// Emgu.CV.CvEnum.CmpType.Equal);

                // CvInvoke.Compare(compare, destiny, resultMat, Emgu.CV.CvEnum.CmpType.Equal);// Emgu.CV.CvEnum.CmpType.Equal);
                //    CvInvoke.Pow(resultMat, 2, resultMat);

                two = resultMat.ToImage<Rgba, byte>();


                suma = two.GetSum();

                count = two.CountNonzero();
                int area = Convert.ToInt32(1e4);
                //    (count as int[])[3] = area;

                array = new object[] { null, count, suma };


                Mat sumaImg = new Mat();

                CvInvoke.Add(compare, destiny, sumaImg);//.ass.ToImage<Rgba,byte>().Add(destiny.ToImage<Rgba,byte>()).Mat;

                result = sumaImg;


                //result = resultMat;
                array[0] = result;
                //   result = two.Mat.Clone();
                //  CvInvoke.Subtract(compare, destiny, resultMat);



            }
            catch (Exception ex)
            {


            }

            destiny?.Dispose();
            compare?.Dispose();
            resultMat?.Dispose();
            two?.Dispose();


            return array;
        }

        public object[] PerformRotationCompare(double angulo, double scalex, double scaley, ref int tx, ref int ty, double skX = 0, double skY = 0, double repairFactor=0.95)
        {

            SystemException exception = null;
            Image<Rgba, byte> two = null;
            Image<Rgba, byte> one = null;
            Mat resultMat=null;
            Image<Rgba, byte> canvas = null;
           Mat destiny = null;
            Mat compare = null;
            object[] array =new object[6] ;
            double raidus = 0;
         //   MCvMoments onemoment;
         //   MCvMoments twomoment;
            int width = 0;
            int height = 0;
        
            bool repairTX = false;
            bool repairTY = false;
            try
            {



                two = UITwo;
                one = this.imgUtil.expandedOne[0];

                performRotateScale(angulo, scalex, scaley, ref one);
                performSkew(skX, skY, ref one);


            }

            catch (Exception ex)
            {

                exception = new SystemException(ex.Message + " at Module#1.1");
            }

    try
            {


                if (exception != null) throw new Exception(exception.Message);

                //determine bigger between one and two
                width = GetBiggerWidth(ref two, ref one);
                height = GetBiggerHeight(ref two, ref one);
                canvas = new Image<Rgba, byte>(width, height, imgUtil.pitchBlack);
                //make canvas from bigger and copy the smaller into this canvas
                canvas = MakeCanvas(ref canvas, out raidus);

            
            }
            catch (Exception ex)
            {

                exception = new SystemException(ex.Message + " at Module#1.2");
            }

            try
            {


                if (exception != null) throw new Exception(exception.Message);

                two = canvas.CopyBlank();

                destiny = performTranslate(ref tx, ref ty, repairFactor, ref two, ref one, ref repairTX, ref repairTY);

            }
            catch (Exception ex)
            {

                exception = new SystemException(ex.Message + " at Module#1.2");
            }

            two?.Dispose();
            one?.Dispose();


            try
            {

                if (exception != null) throw new Exception(exception.Message);

                 one = UITwo;
                 two = canvas.CopyBlank();
                //make canvas from bigger again
                // Copy the bigger into the canvas
                Point middle;
                middle = GetMiddlePointCanvas(raidus, ref one);
                Copy(ref middle, ref two, ref one);
                compare = two.Mat.Clone();

              //  two.Dispose();


                resultMat = new Mat();
              CvInvoke.BitwiseXor(compare, destiny, resultMat, null);// Emgu.CV.CvEnum.CmpType.Equal);
              //  two = resultMat.ToImage<Rgba, byte>();


               int[] counts = resultMat.ToImage<Rgba, byte>().CountNonzero();
               List<int> cnt = new List<int>(counts);
                array[1] = cnt.ToArray();

                Mat sumaImg = new Mat();
                 CvInvoke.Add(compare, destiny, sumaImg);//.ass.ToImage<Rgba,byte>().Add(destiny.ToImage<Rgba,byte>()).Mat;
                array[0] = sumaImg;

            }
            catch (Exception ex)
            {
                string previous = string.Empty;
                if (exception != null)
                {
                    previous = exception?.Message + " +++ ";
                }
                exception = new SystemException(previous + ex.Message + " at Module#2");

            }

            destiny?.Dispose();
            compare?.Dispose();
            resultMat?.Dispose();
            two?.Dispose();
            canvas?.Dispose();

            array[3] = exception;
            array[4] = repairTX;
            array[5] = repairTY;


            return array;
        }

        public static Mat performTranslate(ref int tx, ref int ty, double repairFactor, ref Image<Rgba, byte> two, ref Image<Rgba, byte> one,   ref bool repairTX, ref bool repairTY)
        {
            int x;
            while ( (x= (tx + one.Width- two.Width)) >= 0)
            {
                //  tx = Convert.ToInt32(tx * (repairFactor));
                tx = tx - x -1;
                if (tx < 0) tx = 0;
                repairTX = true;
            }
            int y;
            while ( (y =(ty + one.Height - two.Height)) >=0)
            {
                ty = ty - y-1 ;
                if (ty < 0) ty = 0;
                //  ty = Convert.ToInt32(ty * (repairFactor));
                repairTY = true;
            }

            Point middle = new Point(tx, ty);
            Copy(ref middle, ref two, ref one);
            //get Mat
            return  two.Mat.Clone();
        }

        private void performRotateScale(double angulo, double scalex, double scaley, ref  Image<Rgba, byte> one)
        {
            int newWidth = Convert.ToInt32(one.Width * scalex);
            int newHeight = Convert.ToInt32(one.Height * scaley);

            one = one.Rotate(angulo, imgUtil.pitchBlack,false).Resize(newWidth, newHeight, Emgu.CV.CvEnum.Inter.Linear);

        }
      

        private void performSkew(double skX, double skY,ref Image<Rgba, byte> one)
        {
                    if (skX == 0 && skY == 0) return;
            
                //comparison
                int rows = one.Rows;
                int cols = one.Cols;
                int maxXOffset = Convert.ToInt32(Math.Abs(cols * skX));
                int maxYOffset = Convert.ToInt32(Math.Abs(rows * skY));

                Image<Rgba, byte> sheared = new Image<Rgba, byte>(rows + maxYOffset, cols + maxXOffset, imgUtil.pitchBlack);
                try
                {
                    for (int r = 0; r < sheared.Rows; r++)
                    {
                        for (int c = 0; c < sheared.Cols; c++)
                        {
                            int newR = r + Convert.ToInt32(c * skY) - maxYOffset / 2;
                            int newC = c + Convert.ToInt32(r * skX) - maxXOffset / 2;
                            if (newR >= 0 && newR < rows && newC >= 0 && newC < cols)
                            {

                                sheared[r, c] = one[newR, newC];

                            }

                        }
                    }
                }
                catch (Exception ex)
                {


                }
                //    one.Dispose();
                one.Dispose();
                one = sheared;
                
          
           
        }

        public object[] PerformRotationCompareCuda(double angulo, double scalex, double scaley, int tx, int ty, double skX=0, double skY=0)
        {

          
            Image<Rgba, byte> two=null;
            Image<Rgba, byte> one = null;
            GpuMat resultMat = new GpuMat();
            GpuMat destiny = null;
            GpuMat compare = null;
            object[] array = null;
            GpuMat sumaImg = new GpuMat();

            try
            {


                //   tx += imgUtil.TX/2;


                //make one from all tranformations
             // one = imgUtil.expandedOne[0];
                one = UIOne;
                double raidus;
                int newWidth = Convert.ToInt32(UIOne.Width * scalex);
                int newHeight = Convert.ToInt32(UIOne.Height * scaley);


                GpuMat gOne = new GpuMat(one.Mat);
                GpuMat gNew = new GpuMat();


               

                CudaInvoke.Rotate(gOne, gNew, gOne.Size, angulo,tx,ty);

                gOne.Dispose();
                gOne = new GpuMat();

                CudaInvoke.Resize(gNew, gOne, new Size(newWidth, newHeight));

                // one = one.Rotate(angulo, imgUtil.pitchBlack).Resize(newWidth, newHeight, Emgu.CV.CvEnum.Inter.Cubic);

                one = gOne.ToMat().ToImage<Rgba,byte>();

                //gNew.Dispose();

              //  gOne.Dispose();


                if (skX != 0 && skY != 0)
                {
                    //comparison
                    int rows = one.Rows;
                    int cols = one.Cols;
                    int maxXOffset = Convert.ToInt32(Math.Abs(cols * skX));
                    int maxYOffset = Convert.ToInt32(Math.Abs(rows * skY));

                    CudaImage<Rgba, byte> sheared = new CudaImage<Rgba, byte>(rows + maxYOffset, cols + maxXOffset,true);
                    try
                    {
                        for (int r = 0; r < sheared.Bitmap.Height; r++)
                        {
                            for (int c = 0; c < sheared.Bitmap.Width; c++)
                            {
                                int newR = r + Convert.ToInt32(c * skY) - maxYOffset/2;
                                int newC = c + Convert.ToInt32(r * skX) - maxXOffset/2;
                                if (newR >= 0 && newR < rows && newC >= 0 && newC < cols)
                                {

                                    sheared.Bitmap.SetPixel(r, c, one.Bitmap.GetPixel(newR, newC));

                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {


                    }
                    //    one.Dispose();
                    one.Dispose();
                    one = sheared.ToMat().ToImage<Rgba,byte>();
                    sheared.Dispose();
                }


                //   ty += imgUtil.TY/2;

                // MakeCanvas(ref one, out raidus);


                // int newWidth = Convert.ToInt32(imgUtil.expandedOne[0].Width*scalex);
                // int newHeight = Convert.ToInt32(imgUtil.expandedOne[0].Height*scaley);
                //    int newWidth = Convert.ToInt32(UIOne.Width * scalex);
                //    int newHeight = Convert.ToInt32(UIOne.Height * scaley);
                //    int newWidth = Convert.ToInt32(one.Width * scalex);
                //   int newHeight = Convert.ToInt32(one.Height * scaley);

                //  CvInvoke.MinAreaRect(UITwo.Bitmap.GetBounds().)


                //   int area = (UITwo).Height * (UITwo).Width;
                //  area/=3;

              //  int area = Convert.ToInt32( 4e5);
                //determine bigger between one and two
                Image<Rgba,byte>[] biggerSmaller= GetBiggerSmallerImages(ref UITwo, ref one);

                //make canvas from bigger and copy the smaller into this canvas

                //int area = (biggerSmaller[0]).Height * (biggerSmaller[0]).Width;


                two = MakeCanvas(ref biggerSmaller[0], out raidus);
                ///CAMBIE ESTO
               Point middle = GetMiddlePointCanvas(raidus, ref biggerSmaller[1]);
              //  middle.X += tx;
              //  middle.Y += ty;

           
                ///////////// // Point middle = GetMiddlePointCanvas(raidus, ref biggerSmaller[0]);
                Copy(ref middle, ref two, ref biggerSmaller[1]);

                //get Mat
                destiny = new GpuMat(two.Mat);
                two = two.CopyBlank();
               // two.Dispose();

                //make canvas from bigger again
             //   two = MakeCanvas(ref biggerSmaller[0], out raidus);//;.Width, expandedEscaledUIToCompare[1].Height, pitchWhite);

                // Copy the bigger into the canvas
                middle = GetMiddlePointCanvas(raidus, ref biggerSmaller[0]);
              
                Copy(ref middle, ref two, ref biggerSmaller[0]);


                compare = new GpuMat(two.Mat); //two.Mat.Clone();
              
                two.Dispose();

                object result = null;

               
                CudaInvoke.BitwiseXor(compare, destiny, resultMat, null);// Emgu.CV.CvEnum.CmpType.Equal);
            //  CudaInvoke.Subtract(compare, destiny, resultMat);
               

              // CudaInvoke.Pow(resultMat, 2, resultMat);

           
                //   CvInvoke.Compare(compare, destiny, resultMat, Emgu.CV.CvEnum.CmpType.Equal);
                CudaInvoke.Add(compare, destiny, sumaImg);//.ass.ToImage<Rgba,byte>().Add(destiny.ToImage<Rgba,byte>()).Mat;

               result = resultMat.ToMat();

                two = resultMat.ToMat().ToImage<Rgba, byte>();

                object suma = null;
              
                object count = null;
            
                suma = two.GetSum();
                count = two.CountNonzero();
               int area = Convert.ToInt32(2e5);

                (count as int[])[3] = area;
                //   result = two.Mat.Clone();


                //  CvInvoke.Subtract(compare, destiny, resultMat);


                array = new object[] { result, count, suma };

            }
            catch (Exception ex)
            {


            }
            destiny?.Dispose();
            compare?.Dispose();
            resultMat?.Dispose();
            two?.Dispose();
            one?.Dispose();
            sumaImg?.Dispose();
            return array;
        }

        
    }
   

   


}
