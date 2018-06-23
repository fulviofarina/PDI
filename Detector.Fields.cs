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
      
        //per channel per section/quadrant
        public LineSegment2D[,] UDLRLines;
        public LineSegment2D[,] UDLRLines_H;
        public LineSegment2D[,] UDLRLines_V;

        //per channel, positive or negative, arrays
        public LineSegment2D[,][] Diagonals;
        //per channel
        public LineSegment2D[] avgDiagonalPosCh;
        public LineSegment2D[] avgDiagonalNegCh;

        //avg all channels
        public LineSegment2D avgDiagonalNeg;
        public LineSegment2D avgDiagonalPos;
      
        //avg all channels given per section/quadrant
        public LineSegment2D[] avgUDLRLines_H;
        public LineSegment2D[] avgUDLRLines_V;
        public LineSegment2D[] avgUDLRLines;
        //channel, sectionquadrant, type(hori, verti, others)
        public LineSegment2D[,,][] chUDLR_HVO;



        public StringBuilder msgBuilder;
        public CircleF[] circles;
        public LineSegment2D[] lines;
        public List<Triangle2DF> triangleList;//= new List<Triangle2DF>();
        public List<RotatedRect> boxList;//= new List<RotatedRect>(); //a box is a rotated rectangle

        public LineSegment2D[][] fullLines;

        public Image<Rgb, Byte>[] figure;
       

        public Image<Rgb, byte> raw = null;

        private LineSegment2D refPos;

        public LineSegment2D RefPos
        {
            get => refPos = new  LineSegment2D(new Point(0, 0), new Point(raw.Width, raw.Height));

            set => refPos = value;
        }
        private LineSegment2D refNeg;

        public LineSegment2D RefNeg
        {
            get => refNeg = new LineSegment2D(new Point(raw.Width, 0), new Point(0, raw.Height));

            set => refNeg = value;
        }
        public Detector()
        {
            Initialize();

        }

        private void Initialize()
        {
            figure = new Image<Rgb, byte>[4];
            msgBuilder = new StringBuilder("Performance: ");
            fullLines = new LineSegment2D[4][];
            UDLRLines = new LineSegment2D[3, 4];
            UDLRLines_H = new LineSegment2D[3, 4];
            UDLRLines_V = new LineSegment2D[3, 4];
            avgDiagonalNegCh = new LineSegment2D[3];
            avgDiagonalPosCh = new LineSegment2D[3];
            Diagonals = new LineSegment2D[4, 2][];
            chUDLR_HVO = new LineSegment2D[3, 4, 3][];
        }
    }

   

   
}
