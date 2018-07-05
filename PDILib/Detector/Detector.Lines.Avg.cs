using Emgu.CV.Structure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace PDILib
{
    public partial class Detector
    {
       

        public LineSegment2D GetAvgLine(ref LineSegment2D[] arrRed)
        {
            arrRed = arrRed.Where(s_isValid()).ToArray();
            if (arrRed != null && arrRed.Count() == 0) return new LineSegment2D();



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


            if (arrRed != null && arrRed.Count() == 0)
            {
                return new LineSegment2D();
            }
            arrRed = arrRed.OrderBy(o => o.P1.X).ToArray();
            HashSet<int> p1x = new HashSet<int>();
            Hashtable ls = new Hashtable();

            foreach (LineSegment2D s in arrRed)
            {
                if (p1x.Add(s.P1.X))
                {
                    int count = arrRed.Where(o => o.P1.X == s.P1.X).Count();
                    //   count--;
                    if (count > 0)
                    {
                        ls.Add(s, count);
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
                        if (ls.ContainsKey(s)) ls[s] = (int)ls[s] + count;
                        else ls.Add(s, count);
                    }
                }

            }
            if (ls.Count == 0) return new LineSegment2D();
            double N = ls.Values.Cast<int>().Sum();
            double avgp1x = ls.Keys.Cast<LineSegment2D>().Sum(o => (int)ls[o] * o.P1.X) / N;
            double avgp2x = ls.Keys.Cast<LineSegment2D>().Sum(o => (int)ls[o] * o.P2.X) / N;
            double avgp1y = ls.Keys.Cast<LineSegment2D>().Sum(o => (int)ls[o] * o.P1.Y) / N;
            double avgp2y = ls.Keys.Cast<LineSegment2D>().Sum(o => (int)ls[o] * o.P2.Y) / N;
            Point p1 = new Point(Convert.ToInt32(avgp1x), Convert.ToInt32(avgp1y));
            Point p2 = new Point(Convert.ToInt32(avgp2x), Convert.ToInt32(avgp2y));
            return new LineSegment2D(p1, p2);

        }

      
    }

    public partial class Detector
    {

        public LineSegment2D[] GetAvgDiagonalsPosNeg(bool extendedLenght = false)
        {
            List<LineSegment2D> ls = new List<LineSegment2D>();

            //////////////
            LineSegment2D[] input = null;
            input = avgDiagonalPosCh;
            avgDiagonalPos = GetAvgLine(ref input);

            ///////////
            input = avgDiagonalNegCh;
            avgDiagonalNeg = GetAvgLine(ref input);

            ls.Add(avgDiagonalPos);
            ls.Add(avgDiagonalNeg);

            return ls.ToArray();
        }
        public LineSegment2D[] GetAvgDiagonalsPosNegPerChannel(int channels)
        {
            LineSegment2D d1 = HistoLine(ref Diagonals[channels, 0]);
            avgDiagonalPosCh[channels] = (LineSegment2D)d1;
            LineSegment2D d2 = HistoLine(ref Diagonals[channels, 1]);
            avgDiagonalNegCh[channels] = (LineSegment2D)d2;

            LineSegment2D[] newArr = new LineSegment2D[] { (LineSegment2D)d1, (LineSegment2D)d2 };
            return newArr;
        }



        public void GetAvgUDLR(bool extendeLenght = false)
        {



            LineSegment2D s;

            avgUDLRLines_H = getAvgUDLR(ref UDLRLines_H);


            LineSegment2D[] aux = avgUDLRLines_H;
            if (extendeLenght)
            {
                for (int q = 0; q < 4; q++)
                {

                    s = aux[q];
                    LineSegment2D neo = LineResizeY(ref s);
                    aux[q] = neo;

                }
            }

            avgUDLRLines_V = getAvgUDLR(ref UDLRLines_V);

            aux = avgUDLRLines_V;
            if (extendeLenght)
            {
                for (int q = 0; q < 4; q++)
                {

                    s = aux[q];
                    LineSegment2D neo = LineResizeX(ref s);
                    aux[q] = neo;

                }
            }

            avgUDLRLines = getAvgUDLR(ref UDLRLines);

            
        }

        private LineSegment2D[] makeavgUDLRArray()
        {
            List<LineSegment2D> ls = new List<LineSegment2D>();
            if (avgUDLRLines!=null) ls.AddRange(avgUDLRLines);
            if (avgUDLRLines_H != null) ls.AddRange(avgUDLRLines_H);
            if (avgUDLRLines_V != null) ls.AddRange(avgUDLRLines_V);
          

            //GetAvgDiagonals();

            return ls.ToArray();
        }

        private LineSegment2D[] getAvgUDLR(ref LineSegment2D[,] aux)
        {
            List<LineSegment2D> ls = new List<LineSegment2D>();
            List<LineSegment2D> ls2 = new List<LineSegment2D>();

            for (int udlr = 0; udlr < 4; udlr++)
            {
                ls2.Clear();
                LineSegment2D s;
                for (int channel = 0; channel < 3; channel++)
                {
                    s = aux[channel, udlr];

                    ls2.Add(s);
                }
                LineSegment2D[] input = ls2.ToArray(); //horizontales
                s = GetAvgLine(ref input);
                ls.Add(s);
            }
            return ls.ToArray();
        }


        public LineSegment2D[][,] GetAvgUDLR_HVOPerChannel(int channel)
        {

            getAvgUDLR_HVOPerChannel(channel, ref UDLRLines_H, chUDLR_HVO, 0); //horizontals 0
            getAvgUDLR_HVOPerChannel(channel, ref UDLRLines_V, chUDLR_HVO, 1); //veticals 1
            getAvgUDLR_HVOPerChannel(channel, ref UDLRLines, chUDLR_HVO, 2); //others

            LineSegment2D[][,] ls = { UDLRLines_H, UDLRLines_V, UDLRLines };


            return ls;
        }

        public void getAvgUDLR_HVOPerChannel(int channel, ref LineSegment2D[,] destiny, LineSegment2D[,,][] arry, int type)
        {

            for (int i = 0; i < 4; i++)
            {

                LineSegment2D[] horiz = arry[channel, i, type];
                LineSegment2D dh = HistoLine(ref horiz);
                destiny[channel, i] = (LineSegment2D)dh;
            }
        }

    }
  

   

   

   
}
