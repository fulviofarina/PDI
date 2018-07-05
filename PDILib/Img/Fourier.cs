using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PDILib
{

    public partial class Img
    {
    
        public static class Fourier
        {
            public static Bitmap Matrix2Bitmap(Matrix<float> matrix)
            {
                CvInvoke.Normalize(matrix, matrix, 0.0, 255.0, Emgu.CV.CvEnum.NormType.MinMax);

                Image<Gray, float> image = new Image<Gray, float>(matrix.Size);
                matrix.CopyTo(image);

                return image.ToBitmap();
            }

            // Real part is magnitude, imaginary is phase. 
            // Here we compute log(sqrt(Re^2 + Im^2) + 1) to get the magnitude and 
            // rescale it so everything is visible
            private static Matrix<float> getDftMagnitude(Matrix<float> fftData)
            {
                //The Real part of the Fourier Transform
                Matrix<float> outReal = new Matrix<float>(fftData.Size);
                //The imaginary part of the Fourier Transform
                Matrix<float> outIm = new Matrix<float>(fftData.Size);
                CvInvoke.Split(fftData, outReal);
                CvInvoke.Split(fftData, outIm);

                CvInvoke.Pow(outReal, 2, outReal);
                CvInvoke.Pow(outIm, 2, outIm);

                CvInvoke.Add(outReal, outIm, outReal);
                CvInvoke.Pow(outReal, 0.5, outReal);
                Matrix<float> aux = new Matrix<float>(outReal.Rows, outReal.Cols);
                aux.SetIdentity(new MCvScalar(1.0));
                CvInvoke.Add(outReal, aux, outReal); // 1 + Mag
                CvInvoke.Log(outReal, outReal); // log(1 + Mag)            

                return outReal;
            }

            public static Bitmap DFT(Bitmap bmp)
            {
                Image<Gray, float> image = new Image<Gray, float>(bmp);

                // Transform 1 channel grayscale image into 2 channel image
                IntPtr complexImage = CvInvoke.cvCreateImage(image.Size, Emgu.CV.CvEnum.IplDepth.IplDepth32F, 2);
                CvInvoke.cvSetImageCOI(complexImage, 1); // Select the channel to copy into
                CvInvoke.cvCopy(image, complexImage, IntPtr.Zero);
                CvInvoke.cvSetImageCOI(complexImage, 0); // Select all channels

                // This will hold the DFT data
                Matrix<float> forwardDft = new Matrix<float>(image.Rows, image.Cols, 2);

                Matrix<float> complexdft = new Matrix<float>(image.Rows, image.Cols, complexImage);



                //  Matrix<float> comple = new Matrix<float>(.Rows, forwardDft.Cols, 2);

                CvInvoke.Dft(complexdft, forwardDft, Emgu.CV.CvEnum.DxtType.Forward, 0);

                CvInvoke.cvReleaseImage(ref complexImage);

                // We'll display the magnitude
                Matrix<float> forwardDftMagnitude = getDftMagnitude(forwardDft);
                SwitchQuadrants(ref forwardDftMagnitude);

                // Now compute the inverse to see if we can get back the original


                return Matrix2Bitmap(forwardDftMagnitude);

            }
            public static Bitmap IDFT(Matrix<float> forwardDft)
            {
                Matrix<float> reverseDft = new Matrix<float>(forwardDft.Rows, forwardDft.Cols, 2);
                CvInvoke.Dft(forwardDft, reverseDft, Emgu.CV.CvEnum.DxtType.InvScale, 0);
                Matrix<float> reverseDftMagnitude = getDftMagnitude(reverseDft);
                return Matrix2Bitmap(reverseDftMagnitude);
            }
            // We have to switch quadrants so that the origin is at the image center
            public static void SwitchQuadrants(ref Matrix<float> matrix)
            {
                int cx = matrix.Cols / 2;
                int cy = matrix.Rows / 2;

                Matrix<float> q0 = matrix.GetSubRect(new Rectangle(0, 0, cx, cy));
                Matrix<float> q1 = matrix.GetSubRect(new Rectangle(cx, 0, cx, cy));
                Matrix<float> q2 = matrix.GetSubRect(new Rectangle(0, cy, cx, cy));
                Matrix<float> q3 = matrix.GetSubRect(new Rectangle(cx, cy, cx, cy));
                Matrix<float> tmp = new Matrix<float>(q0.Size);

                q0.CopyTo(tmp);
                q3.CopyTo(q0);
                tmp.CopyTo(q3);
                q1.CopyTo(tmp);
                q2.CopyTo(q1);
                tmp.CopyTo(q2);
            }


        }
    }



   
   


}
