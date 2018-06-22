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
    

    public partial class Detector
    {
       
      

      
            public void  Detect(ref Image<Rgb, byte> aux, ref double[] args, int what = 0)
        {

          
            raw = aux;

            

            //Load the image from file and resize it for display
            //  Image<Bgr, Byte> img =
            //   new Image<Bgr, byte>(fileNameTextBox.Text)
            //   .Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);

            //Convert the image to grayscale and filter out the noise
            UMat uimage = new UMat();
            CvInvoke.CvtColor(raw, uimage, ColorConversion.Bgr2Gray);

            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);

            //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            double cannyThreshold = args[0];
            double cannyThresholdLinking = args[1];
            double circleAccumulatorThreshold = args[2];
            double dp = args[3];
            double minDist = args[4];
            int minRadio = (int)args[5];
            double rho = args[6];
            int threshold = (int) args[7];
            double minWithLine = args[8];
            double gap = args[9];
            double epsilonFact = args[10];
            double minArea = args[11];

            //1 para circles
            if (what ==0 || what == 1) Circles(ref uimage, cannyThreshold, circleAccumulatorThreshold, dp, minDist,minRadio);

            //2 para lines 
            //3 para triang rect
            if (what == 0 || what == 2 || what == 3)
            {
                VectorOfVectorOfPoint contours = Canny(ref uimage, cannyThreshold, cannyThresholdLinking, rho, threshold, minWithLine, gap);
                if (what == 3)   TriangleRectangleImage(ref contours, epsilonFact, minArea);
            }
          
        }



       
        public void TriangleRectangleImage(ref VectorOfVectorOfPoint contours, double epsilonFact =0.05, double minArea= 10)
        {

            Stopwatch watch = Stopwatch.StartNew();
            triangleList = new List<Triangle2DF>();
            boxList = new List<RotatedRect>(); //a box is a rotated rectangle



            int count = contours.Size;
            for (int i = 0; i < count; i++)
            {
                VectorOfPoint contour = contours[i];
                VectorOfPoint approxContour = new VectorOfPoint();
                double epsilon = CvInvoke.ArcLength(contour, true) * epsilonFact;

                CvInvoke.ApproxPolyDP(contour, approxContour, epsilon, true);

                //sign of area gives orientation
                double area = CvInvoke.ContourArea(approxContour, true);
                area = Math.Abs(area);

                if (area > minArea) //only consider contours with area greater than 250
                {
                    if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                    {
                        Point[] pts = approxContour.ToArray();
                        Triangle2DF triangle = new Triangle2DF(pts[0], pts[1], pts[2]);
                        triangleList.Add(triangle);
                    }
                    else if (approxContour.Size == 4) //The contour has 4 vertices.
                    {
                        //determine if all the angles in the contour are within [80, 100] degree
                        bool isRectangle = true;
                        Point[] pts = approxContour.ToArray();
                        LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                        for (int j = 0; j < edges.Length; j++)
                        {
                            int index = (j + 1) % edges.Length;
                            double angle = edges[index].GetExteriorAngleDegree(edges[j]);
                            angle = Math.Abs(angle);
                            if (angle < 80 || angle > 100)
                            {
                                isRectangle = false;
                                break;
                            }
                        }
                        //
                        RotatedRect rotatedRect = CvInvoke.MinAreaRect(approxContour);
                        if (isRectangle)
                        {
                            boxList.Add(rotatedRect);
                        }
                    }
                }
            }

            //   }

            watch.Stop();
            msgBuilder.Append(String.Format("Triangles & Rectangles - {0} ms; ", watch.ElapsedMilliseconds));
           

        }

       

        public VectorOfVectorOfPoint Canny(ref UMat uimage, double cannyThreshold, double cannyThresholdLinking, double rho, int thresh, double minwidth, double gap)
        {

            Stopwatch watch = Stopwatch.StartNew();

            UMat cannyEdges = new UMat();
            CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

            lines = CvInvoke.HoughLinesP(
              cannyEdges,
              rho, //Distance resolution in pixel-related units
              Math.PI / 180, //Angle resolution measured in radians.
                             //  Math.PI / 45, //Angle resolution measured in radians.
                 thresh, //threshold 100
              minwidth, //min Line width 2
              gap); //gap between lines
                    // 20, //threshold
                    //    30, //min Line width
                    //    10); //gap between lines

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

            watch.Stop();
            msgBuilder.Append(String.Format("Canny & Hough lines - {0} ms; ", watch.ElapsedMilliseconds));

           
            // lineImageBox.Image = lineImage;


            return contours;
        }

     

      

        public void Circles(ref UMat uimage, double cannyThreshold, double circleAccumulatorThreshold, double dp, double minDist, int minRadio = 0, int maxRadio =0)
        {

            Stopwatch watch = Stopwatch.StartNew();


            circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, dp, minDist, cannyThreshold, circleAccumulatorThreshold, minRadio, maxRadio);

            watch.Stop();
            msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));

           
            // circleImageBox.Image = circleImage;

           
        }

        public Image<Rgba, byte> Hough(ref Image<Rgb, byte> redImg2)
        {
            //  WrongHough(redImg2);

            Mat imageIn = redImg2.Mat;//.ImRead(filename, ImreadModes.GrayScale).Resize(new Size(800, 600));
            Mat edges = new Mat();

            CvInvoke.Canny(imageIn, edges, 95, 100);

            //HoughLinesP
            double theta = Math.PI / 180;
            LineSegment2D[] segHoughP = CvInvoke.HoughLinesP(edges, 1, theta, 100, 100, 10);

            Mat imageOutP = imageIn.Clone();

            MCvScalar c = new MCvScalar(0);

            foreach (LineSegment2D s in segHoughP)
            {
                CvInvoke.Line(imageOutP, s.P1, s.P2, c, 1, Emgu.CV.CvEnum.LineType.AntiAlias, 0);
            }
            //  using (new Window("Edges", WindowMode.AutoSize, edges))
            //  using (new Window("HoughLinesP", WindowMode.AutoSize, imageOutP))
            //  {
            // Window.WaitKey(0);
            //  }


            //  MessageBox.Show(hs.Count().ToString());
            return imageOutP.ToImage<Rgba, Byte>();
        }
    }

   

   
}
