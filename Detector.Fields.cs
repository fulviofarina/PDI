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
      

        public LineSegment2D[,] UDLRLines;
        public LineSegment2D[,] UDLRLines_H;
        public LineSegment2D[,] UDLRLines_V;
        public LineSegment2D[] DiagonalPos;
        public LineSegment2D[] DiagonalNeg;
        public LineSegment2D avgDiagonalNeg;
        public LineSegment2D avgDiagonalPos;
        public LineSegment2D[] avgUDLRLines_H;
        public LineSegment2D[] avgUDLRLines_V;
        public LineSegment2D[] avgUDLRLines;

       

        public StringBuilder msgBuilder;
        public CircleF[] circles;
        public LineSegment2D[] lines;
        public List<Triangle2DF> triangleList;//= new List<Triangle2DF>();
        public List<RotatedRect> boxList;//= new List<RotatedRect>(); //a box is a rotated rectangle

        public LineSegment2D[][] fullLines;

        public Image<Rgb, Byte>[] figure;
       

        public Image<Rgb, byte> raw = null;

       
        public Detector()
        {
            figure = new Image<Rgb, byte>[4];
            msgBuilder = new StringBuilder("Performance: ");
            fullLines = new LineSegment2D[4][];
            UDLRLines = new LineSegment2D[3,4];
            UDLRLines_H = new LineSegment2D[3,4];
            UDLRLines_V = new LineSegment2D[3,4];
            DiagonalNeg = new LineSegment2D[3];
            DiagonalPos = new LineSegment2D[3];
        }
       
    }

   

   
}
