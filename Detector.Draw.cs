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

        public void DrawUDLR(ref Image<Rgb, byte> result, int channel)
        {
            for (int q = 0; q < 4; q++)
            {
                for (int type = 0; type < 3; type++)
                {
                    LineSegment2D[] aux = chUDLR_HVO[channel, q, type];
                    Image<Rgb, byte> nuevoResult = DrawLines(channel, ref aux);
                    result = result.Add(nuevoResult).Clone();
                    // segmentBox.Image = result.Bitmap;

                    //  MessageBox.Show(channel.ToString() + " " +q.ToString() + " " + type.ToString());
                }
            }

        }
        public void DrawDiagonals(ref Image<Rgb, byte> result, int channel)
        {
            // List<Image<Rgb, byte>> ls = new List<Image<Rgb, byte>>();
            for (int posNeg = 0; posNeg < 2; posNeg++)
            {
                LineSegment2D[] aux = Diagonals[channel, posNeg];
                result = result.Add(DrawLines(channel, ref aux)).Clone();
                //   segmentBox.Image = result.Bitmap;
                // ls.Add(result);

            }


        }
        public Image<Rgb, byte>[] DrawDetectedAvg(ref Image<Rgb, byte> final, ref Rgb[] color, bool append = true)
        {
            figure[2] = final.Clone();


            lines = makeavgUDLRArray();
            Image<Rgb, byte> nuevoResult0 = DrawLines(color[0], append);
            ////////////////////////////////////////////////////////////
          
            ///print red DIAGONALSS //////////////
           // imagen.detect.lines = new LineSegment2D[] { imagen.detect.avgDiagonalPosCh[0] };
            lines = new LineSegment2D[] { avgDiagonalPos };
            Image<Rgb, byte> nuevoResult1 = DrawLines(color[1], append);
            //
            // imagen.detect.lines = new LineSegment2D[] { imagen.detect.avgDiagonalPosCh[1] };
            lines = new LineSegment2D[] { avgDiagonalNeg };
            Image<Rgb, byte> nuevoResult2 = DrawLines(color[2], append);


            return new Image<Rgb, byte>[] { nuevoResult0, nuevoResult1, nuevoResult2 };
        }
        public void PickColorsAvg(int type, ref Rgb[] color)
        {
            color[0] = new Rgb(Color.Green);
            color[1] = new Rgb(Color.Red);
            color[2] = new Rgb(Color.Maroon);

            if (type == 0)
            {
                //ok, previous by default at 0
            }
            else if (type == 1)
            {
                color[0] = new Rgb(Color.Yellow);
                color[1] = new Rgb(Color.Cyan);
                color[2] = new Rgb(Color.Fuchsia);
            }
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

      
        public Image<Rgb, byte> DrawLines(Rgb color, bool append = false)
        {
            if (!append) figure[2] = raw.CopyBlank();
            lines = lines.Where(s_isValid()).ToArray();
            foreach (LineSegment2D line in lines)
            {
                figure[2].Draw(line,color, 1);
            }

            return figure[2];
        }

       
        public void DrawCircles(Rgb color, bool append=false)
        {
            if (!append) figure[1] = raw.CopyBlank();
            foreach (CircleF circle in circles)
                figure[1].Draw(circle, color, 1);
        }




    }

   

   
}
