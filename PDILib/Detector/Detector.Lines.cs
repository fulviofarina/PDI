using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace PDILib
{
    public partial class Detector
    {
        public LineSegment2D LineResizeX(ref LineSegment2D s)
        {
            Point p1 = new Point(s.P1.X, 0);
            Point p2 = new Point(s.P2.X, raw.Height);
            LineSegment2D neo = new LineSegment2D(p1, p2);
            return neo;
        }

        public LineSegment2D LineResizeY(ref LineSegment2D s)
        {
            Point p1 = new Point(0, s.P1.Y);
            Point p2 = new Point(raw.Width, s.P2.Y);
            LineSegment2D neo = new LineSegment2D(p1, p2);
            return neo;
        }
        public LineSegment2D[] GetDiagonals(int channel, int heightOrWidth, double sizeFactor, float fracX = 1.02f, float fracY = 0.98f)
        {

            LineSegment2D[] arrGreen = GetLines(channel, heightOrWidth, sizeFactor, false)
                  .Where(s_isValid())
                .Where(s_lineDir(0, 0, 2))
                .ToArray();
            arrGreen = arrGreen
                .Where(s_lineDir(fracX, fracY, 1)).ToArray();
            return arrGreen;
        }

        public IEnumerable<LineSegment2D> GetLinesByDirection(int channel, float dirx, float diry)
        {
            IEnumerable<LineSegment2D> enumerable = fullLines[channel];
            return enumerable.Where(s_lineDir(dirx, diry, 0));

        }


        public IEnumerable<LineSegment2D> GetLines(int channel, int width, double sizeFactor, bool onlyStraight = true)
        {
            LineSegment2D[] arr = null;
            if (onlyStraight) arr = GetLinesByDirection(channel, 1, 0).Union(GetLinesByDirection(channel, 0, 1)).ToArray();
            else arr = fullLines[channel];

            return arr
                .Where(o => o.Length >= width * sizeFactor).OrderBy(o => o.Length).ToArray();
        }
        public IEnumerable<RotatedRect> GetBoxes(int channel, int width, double sizeFactor)
        {
            RotatedRect[] arr = null;
           // if (onlyStraight) arr = GetLinesByDirection(channel, 1, 0).Union(GetLinesByDirection(channel, 0, 1)).ToArray();
            arr = fullBoxes[channel];

            return arr
                .Where(o => o.Size.Width >= width * sizeFactor).OrderBy(o => o.Size.Width).ToArray();
        }


    }


   





}
