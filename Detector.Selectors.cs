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

        private Func<LineSegment2D, bool> lineDirSelector(float dirx, float diry, int type)
        {
            Func<LineSegment2D, bool> func;

            //for paralell or perpendicular lines or exact values
            func = o =>
            {
                return Math.Abs(o.Direction.X) == dirx && Math.Abs(o.Direction.Y) == diry;
            };
            //for absolute direction or ratios
            if (type == 1)
            {
                func = o =>
                {
                    return Math.Abs(o.Direction.Y / o.Direction.X) <= dirx && Math.Abs(o.Direction.Y / o.Direction.X) >= diry;
                };
            }
            //for not equal = differences
            else if (type == 2)
            {
                func = o =>
                {
                    return o.Direction.X != dirx && o.Direction.Y != diry;
                };
            }
            return func;
        }


        private Func<LineSegment2D, bool> quadrantSelector(int type, int widthHeight)
        {
            Func<LineSegment2D, bool> func;
            int halfwidth = widthHeight / 2;
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
            else if (type == 2)
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
        private Func<LineSegment2D, bool> notANumber()
        {
            Func<LineSegment2D, bool> g = o =>
            {
                return !Double.IsNaN(o.Direction.X);
            };
            return g;
        }
    }

   

   
}
