using Emgu.CV.Structure;
using System;

namespace PDILib
{


    public partial class Detector
    {
        //SELECTORS

        private Func<LineSegment2D, bool> s_lineDir(float dirx, float diry, int type)
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
        private Func<RotatedRect, bool> b_quadrant(int quadrant, int widthHeight, double percent1 = 0.25)
        {
            Func<RotatedRect, bool> func;


            double percent2 = 1 - percent1;
            int factor1 = Convert.ToInt32(widthHeight * percent1);
            int factor2 = Convert.ToInt32(widthHeight * percent2);
            func = o =>
            {

                return (o.Center.Y > factor2 && o.Center.Y + o.Size.Height > factor2);
            };

            if (quadrant == 0)
            {
                func = o =>
                {
                    return (o.Center.Y < factor1 && o.Center.Y + o.Size.Height < factor1);
                  
                };
            }
            else if (quadrant == 3)
            {
                func = o =>
                {

                    return (o.Center.X > factor2 && o.Center.X + o.Size.Width > factor2);

                   
                };
            }
            else if (quadrant == 2)
            {
                func = o =>
                {

                    return (o.Center.X < factor1 && o.Center.X + o.Size.Width < factor1);

                  //  return (o.P1.X < factor1 && o.P2.X < factor1);
                };
            }

            return func;
        }


        private Func<LineSegment2D, bool> s_quadrant(int quadrant, int widthHeight, double percent1 = 0.25)
        {
            Func<LineSegment2D, bool> func;

          
            double percent2 = 1- percent1;
            int factor1 = Convert.ToInt32(widthHeight * percent1);
            int factor2 = Convert.ToInt32(widthHeight * percent2);
            func = o =>
            {
              
                return (o.P1.Y > factor2 && o.P2.Y > factor2);
            };

            if (quadrant == 0)
            {
                func = o =>
                {
                 
                    return (o.P1.Y < factor1 && o.P2.Y < factor1);
                };
            }
            else if (quadrant == 3)
            {
                func = o =>
                {
               

                    return (o.P1.X > factor2 && o.P2.X > factor2);
                };
            }
            else if (quadrant == 2)
            {
                func = o =>
                {
                 

                    return (o.P1.X < factor1 && o.P2.X < factor1);
                };
            }

            return func;
        }
        private Func<LineSegment2D, bool> s_isValid()
        {
            Func<LineSegment2D, bool> g = o =>
            {
                return !Double.IsNaN(o.Direction.X);
            };
            return g;
        }
    }

   

   
}
