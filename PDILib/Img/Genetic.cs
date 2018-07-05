using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using Emgu.CV.Cuda;

namespace PDILib
{




    public partial class Img
    {

        public object[] PerformRotationCompareOld(double angulo, double scalex, double scaley, int tx, int ty, double skX = 0, double skY = 0)
        {


            Image<Rgba, byte> two = null;
            Image<Rgba, byte> one = null;
            Mat resultMat = new Mat();
            Mat destiny = null;
            Mat compare = null;
            object[] array = null;

            try
            {


                //   tx += imgUtil.TX/2;


                //make one from all tranformations
                // one = imgUtil.expandedOne[0];
                one = UIOne;
                double raidus;
                int newWidth = Convert.ToInt32(UIOne.Width * scalex);
                int newHeight = Convert.ToInt32(UIOne.Height * scaley);


                one = one.Rotate(angulo, imgUtil.pitchBlack).Resize(newWidth, newHeight, Emgu.CV.CvEnum.Inter.Cubic);

               


                if (skX != 0 && skY != 0)
                {
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

                    one = sheared.Clone();
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
                Image<Rgba, byte>[] biggerSmaller = GetBiggerSmallerImages(ref UITwo, ref one);

                //make canvas from bigger and copy the smaller into this canvas

                //int area = (biggerSmaller[0]).Height * (biggerSmaller[0]).Width;


                two = MakeCanvas(ref biggerSmaller[0], out raidus);
                ///CAMBIE ESTO
                Point middle = GetMiddlePointCanvas(raidus, ref biggerSmaller[1]);
                middle.X += tx;
                middle.Y += ty;


                ///////////// // Point middle = GetMiddlePointCanvas(raidus, ref biggerSmaller[0]);
                Copy(ref middle, ref two, ref biggerSmaller[1]);

                //get Mat
                destiny = two.Mat.Clone();
                two = two.CopyBlank();
                // two.Dispose();

                //make canvas from bigger again
                //   two = MakeCanvas(ref biggerSmaller[0], out raidus);//;.Width, expandedEscaledUIToCompare[1].Height, pitchWhite);

                // Copy the bigger into the canvas
                middle = GetMiddlePointCanvas(raidus, ref biggerSmaller[0]);

                Copy(ref middle, ref two, ref biggerSmaller[0]);


                compare = two.Mat.Clone();

                two.Dispose();

                object result = null;
                CvInvoke.BitwiseXor(compare, destiny, resultMat, null);// Emgu.CV.CvEnum.CmpType.Equal);
                                                                       // CvInvoke.Subtract(compare, destiny, resultMat);


                // CvInvoke.Pow(resultMat, 2, resultMat);

                Mat sumaImg = new Mat();
                //   CvInvoke.Compare(compare, destiny, resultMat, Emgu.CV.CvEnum.CmpType.Equal);
                CvInvoke.Add(compare, destiny, sumaImg);//.ass.ToImage<Rgba,byte>().Add(destiny.ToImage<Rgba,byte>()).Mat;

                result = sumaImg;

                two = resultMat.ToImage<Rgba, byte>();

                object suma = null;

                object count = null;

                suma = two.GetSum();
                count = two.CountNonzero();
                int area = Convert.ToInt32(6e5);

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

            return array;
        }


        public object[] PerformRotationCompare(double angulo, double scalex, double scaley, int tx, int ty, double skX=0, double skY=0)
        {

          
            Image<Rgba, byte> two=null;
            Image<Rgba, byte> one = null;
            GpuMat resultMat = new GpuMat();
            GpuMat destiny = null;
            GpuMat compare = null;
            object[] array = null;

            try
            {


                //   tx += imgUtil.TX/2;


                //make one from all tranformations
             // one = imgUtil.expandedOne[0];
                one = UIOne;
                double raidus;
               // int newWidth = Convert.ToInt32(UIOne.Width * scalex);
               // int newHeight = Convert.ToInt32(UIOne.Height * scaley);


                GpuMat gOne = new GpuMat(one);

                CudaInvoke.Rotate(gOne, gOne, new Size(0,0), angulo);

                CudaInvoke.Resize(gOne, gOne, new Size(0, 0), scalex, scaley, Emgu.CV.CvEnum.Inter.Cubic);

                // one = one.Rotate(angulo, imgUtil.pitchBlack).Resize(newWidth, newHeight, Emgu.CV.CvEnum.Inter.Cubic);

                one = gOne.ToMat().ToImage<Rgba,byte>();


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
                middle.X += tx;
                middle.Y += ty;

           
                ///////////// // Point middle = GetMiddlePointCanvas(raidus, ref biggerSmaller[0]);
                Copy(ref middle, ref two, ref biggerSmaller[1]);

                //get Mat
                destiny = new GpuMat(two);
                two = two.CopyBlank();
               // two.Dispose();

                //make canvas from bigger again
             //   two = MakeCanvas(ref biggerSmaller[0], out raidus);//;.Width, expandedEscaledUIToCompare[1].Height, pitchWhite);

                // Copy the bigger into the canvas
                middle = GetMiddlePointCanvas(raidus, ref biggerSmaller[0]);
              
                Copy(ref middle, ref two, ref biggerSmaller[0]);


                compare = new GpuMat(two); //two.Mat.Clone();
              
                two.Dispose();

                object result = null;

               
                  CudaInvoke.BitwiseXor(compare, destiny, resultMat, null);// Emgu.CV.CvEnum.CmpType.Equal);
               // CvInvoke.Subtract(compare, destiny, resultMat);
               

               // CvInvoke.Pow(resultMat, 2, resultMat);

                GpuMat sumaImg = new  GpuMat();
                //   CvInvoke.Compare(compare, destiny, resultMat, Emgu.CV.CvEnum.CmpType.Equal);
                CudaInvoke.Add(compare, destiny, sumaImg);//.ass.ToImage<Rgba,byte>().Add(destiny.ToImage<Rgba,byte>()).Mat;

                result = sumaImg;

                two = resultMat.ToMat().ToImage<Rgba, byte>();

                object suma = null;
              
                object count = null;
            
                suma = two.GetSum();
                count = two.CountNonzero();
               int area = Convert.ToInt32(6e5);

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

            return array;
        }

        
    }
   

   


}
