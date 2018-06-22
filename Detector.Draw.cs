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

      
        public void DrawLines(Rgb color, bool append = false)
        {
            if (!append) figure[2] = raw.CopyBlank();
            lines = lines.Where(notANumber()).ToArray();
            foreach (LineSegment2D line in lines)
            {
                figure[2].Draw(line,color, 1);
            }
        }

       
        public void DrawCircles(Rgb color, bool append=false)
        {
            if (!append) figure[1] = raw.CopyBlank();
            foreach (CircleF circle in circles)
                figure[1].Draw(circle, color, 1);
        }




    }

   

   
}
