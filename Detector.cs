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
        public Image<Rgb, byte> DrawLines( int val, ref LineSegment2D[] arrRed)
        {
        
           
            int[] rgb = new int[3] { 0, 0, 0 };
            rgb[val] = 255;
            Rgb color = new Rgb(rgb[0], rgb[1], rgb[2]);
            DrawLines(color, ref arrRed);
            return figure[2].Clone();
        }
        public Image<Rgb, byte> DrawLines(Rgb color, ref LineSegment2D[] arrRed)
        {
            lines = arrRed;
            DrawLines(color, false);
            return figure[2].Clone();
        }


        public LineSegment2D[] ExtractAvgLines(double factor, int channel, bool onlyStraight = true)
        {
            List<LineSegment2D> ls = new List<LineSegment2D>();

            for (int i = 0; i < 4; i++)
            {
                LineSegment2D[] ul = GetUDLRLines(channel, raw.Height, factor, i, onlyStraight);

                LineSegment2D du = HistoLine(ref ul);
                ls.Add(du);
             
            }
            
            return ls.ToArray();
        }
        public LineSegment2D[] ExtractAvgDiagonalLines(double factor, int channels)
        {
            LineSegment2D[] diag = GetDiagonals(channels, raw.Height, factor);

            LineSegment2D[] da = diag.Where(o => o.Direction.Y / o.Direction.X > 0).ToArray();
            LineSegment2D d1 = HistoLine(ref  da );
            LineSegment2D[] db = diag.Where(o => o.Direction.Y / o.Direction.X < 0).ToArray();
            LineSegment2D d2 = HistoLine(ref db);
            LineSegment2D[] newArr = new LineSegment2D[] { d1,d2 };
            return newArr;
        }
        public LineSegment2D GetAvgLine(ref LineSegment2D[] arrRed)
        {
            if (arrRed!=null && arrRed.Count() == 0) return new LineSegment2D();
           
            double avgp1x = arrRed.Average(o => o.P1.X);
            double avgp2x = arrRed.Average(o => o.P2.X);
            double avgp1y = arrRed.Average(o => o.P1.Y);
            double avgp2y = arrRed.Average(o => o.P2.Y);
            Point p1 = new Point(Convert.ToInt32(avgp1x), Convert.ToInt32(avgp1y));
            Point p2 = new Point(Convert.ToInt32(avgp2x), Convert.ToInt32(avgp2y));
            LineSegment2D d = new LineSegment2D(p1, p2);
            return d;
        }
        public LineSegment2D HistoLine(ref LineSegment2D[] arrRed)
        {
            

               if (arrRed != null && arrRed.Count() == 0) return new LineSegment2D();
          
                arrRed = arrRed.OrderBy(o => o.P1.X).ToArray();
                HashSet<int> p1x = new HashSet<int>();
                Hashtable ls =  new Hashtable();
                
                foreach (LineSegment2D s in arrRed)
                {
                    if (p1x.Add(s.P1.X))
                    {
                        int count = arrRed.Where(o => o.P1.X == s.P1.X).Count();
                     //   count--;
                        if (count > 0)
                        {
                            ls.Add(s, count );
                        }
                    }

                }
            p1x.Clear();
            foreach (LineSegment2D s in arrRed)
            {
                if (p1x.Add(s.P1.Y))
                {
                    int count = arrRed.Where(o => o.P1.Y == s.P1.Y).Count();
                    
                    if (count > 0)
                    {
                        if (ls.ContainsKey(s)) ls[s] = (int)ls[s]+ count;
                        else ls.Add(s, count);
                    }
                }

            }
            if (ls.Count == 0) return new LineSegment2D();
            double N = ls.Values.Cast<int>().Sum();
            double avgp1x = ls.Keys.Cast<LineSegment2D>().Sum(o => (int)ls[o] * o.P1.X)/N;
            double avgp2x = ls.Keys.Cast<LineSegment2D>().Sum(o => (int)ls[o] * o.P2.X) / N;
            double avgp1y = ls.Keys.Cast<LineSegment2D>().Sum(o => (int)ls[o] * o.P1.Y) / N;
            double avgp2y = ls.Keys.Cast<LineSegment2D>().Sum(o => (int)ls[o] * o.P2.Y) / N;
            Point p1 = new Point(Convert.ToInt32(avgp1x), Convert.ToInt32(avgp1y));
            Point p2 = new Point(Convert.ToInt32(avgp2x), Convert.ToInt32(avgp2y));
            return new LineSegment2D(p1,p2);

        }

        public StringBuilder msgBuilder;
        public CircleF[] circles;
        public LineSegment2D[] lines;
        public List<Triangle2DF> triangleList;//= new List<Triangle2DF>();
        public List<RotatedRect> boxList;//= new List<RotatedRect>(); //a box is a rotated rectangle

        public LineSegment2D[][] fullLines;

        public Image<Rgb, Byte>[] figure;
       

        public Image<Rgb, byte> raw = null;

        private Func<LineSegment2D, bool> lineDirSelector(float dirx, float diry, int type)
        {
            Func<LineSegment2D, bool> func;
          
            //for paralell or perpendicular lines or exact values
            func = o =>
            {
                return Math.Abs(o.Direction.X) == dirx && Math.Abs(o.Direction.Y) == diry;
            };
            //for absolute direction or ratios
            if (type ==1)
            {
                func = o =>
                {
                    return Math.Abs(o.Direction.Y / o.Direction.X) <= dirx && Math.Abs(o.Direction.Y / o.Direction.X) >= diry;
                };
            }
            //for not equal = differences
            else if (type ==2)
            {
                func = o =>
                {
                    return o.Direction.X != dirx && o.Direction.Y != diry;
                };
            }
            return func;
        }


        private Func< LineSegment2D,bool> quadrantSelector(int type, int widthHeight)
        {
            Func< LineSegment2D,bool> func;
            int halfwidth= widthHeight / 2;
            func = o =>
            {
                return (o.P1.Y > halfwidth && o.P2.Y > halfwidth);
            };

            if (type == 1)
            {
                func = o =>
                {
                    return (o.P1.Y < halfwidth && o.P2.Y < halfwidth);
                };
            }
            else if (type ==2)
            {
                func = o =>
                {
                    return (o.P1.X > halfwidth && o.P2.X > halfwidth);
                };
            }
            else if (type == 3)
            {
                func = o =>
                {
                    return (o.P1.X < halfwidth && o.P2.X < halfwidth);
                };
            }

            return func;
        }

        /// <summary>
        /// Perpendiculares y paralelas
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="heightOrWidth"></param>
        /// <param name="sizeFactor"></param>
        /// <param name="UDLR"></param>
        /// <param name="onlyStraight"></param>
        /// <returns></returns>
        public LineSegment2D[] GetUDLRLines(int channel, int heightOrWidth, double sizeFactor, int UDLR, bool onlyStraight = true)
        {
            
            LineSegment2D[] arrGreen = GetLines(channel, heightOrWidth, sizeFactor, onlyStraight)
                .Where(quadrantSelector(UDLR, heightOrWidth))
                .ToArray();
            return arrGreen;
        }
       
        public LineSegment2D[] GetDiagonals(int channel, int heightOrWidth, double sizeFactor, float fracX = 1.02f, float fracY = 0.98f)
        {

            LineSegment2D[] arrGreen = GetLines(channel, heightOrWidth, sizeFactor, false)
                .Where(lineDirSelector(0,0,2)).ToArray();
            arrGreen = arrGreen
                .Where(lineDirSelector(fracX, fracY, 1)).ToArray();
            return arrGreen;
        }


        public LineSegment2D[] GetLines(int channel, int width, double sizeFactor, bool onlyStraight=true)
        {
            LineSegment2D[] arr = null;
            if (onlyStraight) arr = GetLinesByDirection(channel, 1, 0).Union(GetLinesByDirection(channel, 0, 1)).ToArray();
            else arr = fullLines[channel];

            return arr
                .Where(o => o.Length >= width * sizeFactor).OrderBy(o=> o.Length).ToArray();
        }

        public Detector()
        {
            figure = new Image<Rgb, byte>[4];
            msgBuilder = new StringBuilder("Performance: ");
            fullLines = new LineSegment2D[4][];
        }
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



        public IEnumerable<LineSegment2D> GetLinesByDirection(int channel, float dirx, float diry )
        {
           
                return fullLines[channel].Where(lineDirSelector(dirx,diry,0));
                         
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

        public void DrawTriRect(Rgb color1, Rgb color2, bool append = false)
        {
            if (!append) figure[3] = raw.CopyBlank();
            foreach (Triangle2DF triangle in triangleList)
            {
                figure[3].Draw(triangle, color1, 1);
            }
            foreach (RotatedRect box in boxList)
            {
                figure[3].Draw(box, color2, 1);
            }
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

        public void DrawLines(Rgb color, bool append = false)
        {
            if (!append) figure[2] = raw.CopyBlank();
            foreach (LineSegment2D line in lines)
            {
                figure[2].Draw(line,color, 1);
            }
        }

        public void Circles(ref UMat uimage, double cannyThreshold, double circleAccumulatorThreshold, double dp, double minDist, int minRadio = 0, int maxRadio =0)
        {

            Stopwatch watch = Stopwatch.StartNew();


            circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, dp, minDist, cannyThreshold, circleAccumulatorThreshold, minRadio, maxRadio);

            watch.Stop();
            msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));

           
            // circleImageBox.Image = circleImage;

           
        }

        public void DrawCircles(Rgb color, bool append=false)
        {
            if (!append) figure[1] = raw.CopyBlank();
            foreach (CircleF circle in circles)
                figure[1].Draw(circle, color, 1);
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
