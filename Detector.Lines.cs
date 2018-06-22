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
       
      

        public LineSegment2D[] GetAvgRGBLines(bool extendeLenght = false)
        {
           
            List<LineSegment2D> ls = new List<LineSegment2D>();
          
            LineSegment2D s;
           
            avgUDLRLines_H = GetAvgUDLRAllChannels(ref UDLRLines_H);

            for (int i = 0; i < 4; i++)
            {
                if (extendeLenght)
                {
                    s= avgUDLRLines_H[i];
                    Point p1 = new Point(0, s.P1.Y);
                    Point p2 = new Point(raw.Width, s.P2.Y);
                    LineSegment2D neo = new LineSegment2D(p1,p2);
                    avgUDLRLines_H[i] = neo;
                }
            }

            avgUDLRLines_V = GetAvgUDLRAllChannels(ref UDLRLines_V);
            for (int i = 0; i < 4; i++)
            {
                if (extendeLenght)
                {
                    s = avgUDLRLines_V[i];
                    Point p1 = new Point(s.P1.X, 0);
                    Point p2 = new Point(s.P2.X, raw.Height);
                    LineSegment2D neo = new LineSegment2D(p1, p2);
                    avgUDLRLines_V[i] = neo;
                }
            }

          
            avgUDLRLines = GetAvgUDLRAllChannels(ref UDLRLines);

            ls.AddRange(avgUDLRLines_H);
            ls.AddRange(avgUDLRLines_V);
            ls.AddRange(avgUDLRLines);
            //////////////
            LineSegment2D[] input = null;
            input = DiagonalPos;
            avgDiagonalPos = GetAvgLine(ref input);
            ls.Add(avgDiagonalPos);
            ///////////
            input = DiagonalNeg;
            avgDiagonalNeg = GetAvgLine(ref input);
            ls.Add(avgDiagonalNeg);

            return ls.ToArray();
        }

        private LineSegment2D[] GetAvgUDLRAllChannels(ref LineSegment2D[,] aux)
        {
            List<LineSegment2D> ls = new List<LineSegment2D>();
            List<LineSegment2D> ls2 = new List<LineSegment2D>();
          
            for (int i = 0; i < 4; i++)
            {
                ls2.Clear();
                LineSegment2D s;
                for (int channel = 0; channel < 3; channel++)
                {
                    s = aux[channel, i];
                  
                    ls2.Add(s);
                }
                LineSegment2D[]  input = ls2.ToArray(); //horizontales
                s = GetAvgLine(ref input);
                ls.Add(s);
            }
            return ls.ToArray();
        }

        public LineSegment2D[] ExtractAvgLines(double factor, int channel, bool onlyStraight = true)
        {
            List<LineSegment2D> ls = new List<LineSegment2D>();
         //i iterador es el segmento arriba abajo iz derecha
            for (int i = 0; i < 4; i++)
            {
                LineSegment2D[] ul = GetUDLRLines(channel, raw.Height, factor, i, onlyStraight);
                LineSegment2D[] horiz = ul.Where(lineDirSelector(1, 0, 0)).ToArray();
                LineSegment2D[] verti = ul.Where(lineDirSelector(0, 1, 0)).ToArray();
                ul = ul.Except(horiz).ToArray();
                ul = ul.Except(verti).ToArray();

                LineSegment2D dh = HistoLine(ref horiz);
                UDLRLines_H[channel,i] = (LineSegment2D)dh;
                LineSegment2D dv = HistoLine(ref verti);
                UDLRLines_V[channel,i] = (LineSegment2D)dv;
                LineSegment2D d = HistoLine(ref ul); //the rest
                UDLRLines[channel,i] = (LineSegment2D)d;
                ls.Add((LineSegment2D)dh);
                ls.Add((LineSegment2D)dv);
                ls.Add((LineSegment2D)d);

            }
            
            return ls.ToArray();
        }
        public LineSegment2D[] ExtractAvgDiagonalLines(double factor, int channels)
        {
            LineSegment2D[] diag = GetDiagonals(channels, raw.Height, factor);

            LineSegment2D[] da = diag.Where(o => o.Direction.Y / o.Direction.X > 0).ToArray();
            LineSegment2D d1 = HistoLine(ref  da );
            DiagonalPos[channels] = (LineSegment2D)d1;

            LineSegment2D[] db = diag.Where(o => o.Direction.Y / o.Direction.X < 0).ToArray();
            LineSegment2D d2 = HistoLine(ref db);
            DiagonalNeg[channels] = (LineSegment2D)d2;
            LineSegment2D[] newArr = new LineSegment2D[] { (LineSegment2D)d1, (LineSegment2D)d2 };
            return newArr;
        }
        public LineSegment2D GetAvgLine(ref LineSegment2D[] arrRed)
        {
            if (arrRed!=null && arrRed.Count() == 0) return new LineSegment2D();

            arrRed = arrRed.Where(notANumber()).ToArray();
           
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
        public IEnumerable<LineSegment2D> GetLinesByDirection(int channel, float dirx, float diry)
        {
            IEnumerable<LineSegment2D> enumerable = fullLines[channel];
            return enumerable.Where(lineDirSelector(dirx, diry, 0));

        }


        public LineSegment2D[] GetLines(int channel, int width, double sizeFactor, bool onlyStraight=true)
        {
            LineSegment2D[] arr = null;
            if (onlyStraight) arr = GetLinesByDirection(channel, 1, 0).Union(GetLinesByDirection(channel, 0, 1)).ToArray();
            else arr = fullLines[channel];

            return arr
                .Where(o => o.Length >= width * sizeFactor).OrderBy(o=> o.Length).ToArray();
        }

      
        
    }

   

   
}
