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
       


        public void GetDiagonalsPosNegPerChannel(double factor, int channels)
        {
            LineSegment2D[] diag = GetDiagonals(channels, raw.Height, factor);

            Diagonals[channels, 0] = diag.Where(o => o.Direction.Y / o.Direction.X > 0).ToArray();
            Diagonals[channels, 1] = diag.Where(o => o.Direction.Y / o.Direction.X < 0).ToArray();
        }


   
        public LineSegment2D[,,][] GetUDLR_HVOPerChannel(double factor, int channel, double percentFrame = 0.25, bool onlyStraight = true)
        {
            List<LineSegment2D> ls = new List<LineSegment2D>();

            //i iterador es el segmento arriba abajo iz derecha
            for (int i = 0; i < 4; i++)
            {
                LineSegment2D[] ul = getUDLRLines(channel, raw.Height, factor, i, percentFrame, onlyStraight);
                LineSegment2D[] horiz = ul.Where(s_lineDir(1, 0, 0)).ToArray();
                LineSegment2D[] verti = ul.Where(s_lineDir(0, 1, 0)).ToArray();
                ul = ul.Except(horiz).ToArray();
                ul = ul.Except(verti).ToArray();

                chUDLR_HVO[channel, i, 0] = horiz;
                chUDLR_HVO[channel, i, 1] = verti;
                chUDLR_HVO[channel, i, 2] = ul;

            }

            return chUDLR_HVO;
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
        public LineSegment2D[] getUDLRLines(int channel, int heightOrWidth, double sizeFactor, int UDLR, double percentFrame = 0.25, bool onlyStraight = true)
        {

            LineSegment2D[] arrGreen = GetLines(channel, heightOrWidth, sizeFactor, onlyStraight)
                .Where(s_quadrant(UDLR, heightOrWidth, percentFrame))
                .ToArray();
            return arrGreen;
        }


    }

   

   

   
}
