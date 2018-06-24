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





    public partial class Img
    {

        public void BeginDetection(ref Image<Rgb, byte> aux, int what, bool draw = false)
        {
            detect.Detect(ref aux, ref args, what);

            if (draw) DrawDetected(what);
        }

        private List<LineSegment2D[]> divideRoutine(int detectType, int step, double percent, double max, double valor, int channel, ref Image<Rgb, byte> result, double factor)
        {
            List<LineSegment2D[]> ls = new List<LineSegment2D[]>();

            for (double val = valor; val <= max; val += step)
            {
                Threshold(val, max, channel);
                ElementSubstraction(channel);
                Divide(channel);

                args[7] = val; //actualiza threshold

                result = Divs[channel].Convert<Rgb, byte>();

                BeginDetection(ref result, detectType);
                ls.Add(detect.lines);

            }


            return ls;
        }


        private List<LineSegment2D[]> elementSubsRoutine(int detectType, int step, double max, double valor, int channel, ref Image<Rgb, byte> result, double factorDiago)
        {

            List<LineSegment2D[]> ls = new List<LineSegment2D[]>();
            for (double val = valor; val <= max; val += step)
            {
                Threshold(val, max, channel);
                ElementSubstraction(channel);
                //   imagen.Divide(channel);
                args[7] = 220; //actualiza threshold
                result = ElementSubs[channel].Convert<Rgb, byte>();

                BeginDetection(ref result, detectType);
                ls.Add(detect.lines);

            }
            return ls;
        }
        public void DrawDetected(int what)
        {

            Rgb std = new Rgb(0, 255, 0);
            Rgb std2 = std;

            if (what == 1) std = new Rgb(200, 0, 255);
            else if (what == 3)
            {
                std = new Rgb(0, 0, 255);
                std2 = new Rgb(100, 200, 255);
            }

            if (what == 1) detect.DrawCircles(std);
            else if (what == 2) detect.DrawLines(std);
            else if (what == 3) detect.DrawTriRect(std, std2);

        }
        public void ElementSubsRoutine(int detectType, int step, double max, double resetvalor0, ref Image<Rgb, byte> final, double factorDiago)
        {
            for (int channel = 0; channel < 3; channel++)
            {

                double valor = resetvalor0;
                Image<Rgb, byte> result = null;
                List<LineSegment2D[]> ls = elementSubsRoutine(detectType, step, max, valor, channel, ref result, factorDiago);
                ///  MessageBox.Show("Last Subs");
                detect.SelectMany(channel, ref ls);
                ls.Clear();
                ls = null;
            }


        }
        public void DivideRoutine(int detectType, int step, double percent, double max, double resetvalor, ref Image<Rgb, byte> final, double factor)
        {

            for (int channel = 0; channel < 3; channel++)
            {
                double valor = resetvalor;
                Image<Rgb, byte> result = null;
                List<LineSegment2D[]> ls = divideRoutine(detectType, step, percent, max, valor, channel, ref result, factor);
                //faltaba esto
                detect.SelectMany(channel, ref ls);
                ls.Clear();
                ls = null;
                /// MessageBox.Show("UDLRs");
            }


        }

        public Image<Rgb, byte> GetDiagonalsRoutine(Image<Rgb, byte> final, double factorDiago)
        {
            for (int channel = 0; channel < 3; channel++)
            {
                detect.GetDiagonalsPosNegPerChannel(factorDiago, channel);
                detect.GetAvgDiagonalsPosNegPerChannel(channel);

                Image<Rgb, byte> result = null;
                //show last
                //segmentBox.Image = result.Bitmap;

                //print
                result = detect.raw.CopyBlank();
                detect.DrawDiagonals(ref result, channel);
                final = final.Add(result).Clone();
                //show last
                // segmentBox.Image = final.Bitmap;
                ///////////////////////////////////////////////////
                //  MessageBox.Show("Diagonals");

            }


            return final;
        }


        public Image<Rgb, byte> GetUDLRRoutine(double percent, Image<Rgb, byte> final, double factor)
        {
            for (int channel = 0; channel < 3; channel++)
            {
                detect.GetUDLR_HVOPerChannel(factor, channel, percent, true);
                detect.GetAvgUDLR_HVOPerChannel(channel);

                Image<Rgb, byte> result = null;
                result = detect.raw.CopyBlank();
                //i iterador es el segmento arriba abajo iz derecha
                detect.DrawUDLR(ref result, channel);
                final = final.Add(result).Clone();
                //   segmentBox.Image = result.Bitmap;
            }



            return final;
        }




    }

   


}
