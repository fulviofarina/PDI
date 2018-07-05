using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PDILib
{

   


   
    public partial class Img
    {




        public void BeginDetection(ref Image<Rgb, byte> aux, int what, bool draw = false)
        {
            detect.Detect(ref aux, ref args, what);

            if (draw) DrawDetected(what);
        }

        private List<LineSegment2D[]> divideRoutine(int detectType, int step, double percent, double max, double valor, int channel, ref Image<Rgb, byte> result)
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


        private List<LineSegment2D[]> elementSubsRoutine(int detectType, int step, double max, double valor, int channel, ref Image<Rgb, byte> result)
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
        public void ElementSubsRoutine(int detectType, int step, double max, double resetvalor0)
        {
            for (int channel = 0; channel < 3; channel++)
            {

                double valor = resetvalor0;
                Image<Rgb, byte> result = null;
                List<LineSegment2D[]> ls = elementSubsRoutine(detectType, step, max, valor, channel, ref result);
                ///  MessageBox.Show("Last Subs");
                detect.SelectMany(channel, ref ls);
                ls.Clear();
                ls = null;
            }


        }
        public void DivideRoutine(int detectType, int step, double percent, double max, double resetvalor)
        {

            for (int channel = 0; channel < 3; channel++)
            {
                double valor = resetvalor;
                Image<Rgb, byte> result = null;
                List<LineSegment2D[]> ls = divideRoutine(detectType, step, percent, max, valor, channel, ref result);
                //faltaba esto
                detect.SelectMany(channel, ref ls);
                ls.Clear();
                ls = null;
                /// MessageBox.Show("UDLRs");
            }


        }
        public void SobelRoutine(int detectType, int step, double threshold, int max, int resetvalor, int xorder = 1, int yorder = 1)
        {

            for (int channel = 0; channel < 3; channel++)
            {
                int valor = resetvalor;
                Image<Rgb, byte> result = null;
                List<LineSegment2D[]> ls = sobelRoutine(detectType, step, threshold, max, valor, channel, ref result, xorder, yorder);
                //faltaba esto
                detect.SelectMany(channel, ref ls);
                ls.Clear();
                ls = null;
                /// MessageBox.Show("UDLRs");
            }


        }
        public List<RotatedRect> GetAllBoxesRoutine(int channel, double thres=200, double max=255, double step =20)
        {
            List<RotatedRect> ls = new List<RotatedRect>();

            for (double d = thres; d <= max; d += step)
            {
                args[7] = d; //no recuerdo si lo estoy usando
                Threshold(d, max);
                ElementSubstraction();
                Divide();


                Image<Rgb, byte>[] others = Img.GetOtheChannels(channel, ref Divs);

                Image<Rgb, byte> o = others[0]; //red-green 
                if (channel == 2) o = others[1]; //blue - red
                Divs[channel] = Divs[channel].Sub(o).Dilate(2);

                BeginDetection(ref Divs[channel], 3);
                ls.AddRange(detect.boxList.ToArray());

            }


            detect.fullBoxes[channel] = ls.ToArray();
            return ls;
        }

        private List<LineSegment2D[]> sobelRoutine(int detectType, int step, double threshold, int max, int valor, int channel, ref Image<Rgb, byte> result, int xorder = 1, int yorder = 1)
        {
            List<LineSegment2D[]> ls = new List<LineSegment2D[]>();

            for (int aperture = valor; aperture <= max; aperture += step)
            {
                Sobel(channel, xorder, yorder,aperture);
               // ElementSubstraction(channel);
               // Divide(channel);

                args[7] = threshold; //actualiza threshold

                result = Soby[channel].Convert<Rgb, byte>();

                BeginDetection(ref result, detectType);
                ls.Add(detect.lines);

            }


            return ls;
        }

        public void  GetDiagonalsRoutine(double factorDiago)
        {
            for (int channel = 0; channel < 3; channel++)
            {
                detect.GetDiagonalsPosNegPerChannel(factorDiago, channel);
                detect.GetAvgDiagonalsPosNegPerChannel(channel);


            }

            
        }

        

        public void GetUDLRRoutine(double percent, double factor)
        {
            for (int channel = 0; channel < 3; channel++)
            {
                detect.GetUDLR_HVOPerChannel(factor, channel, percent, true);
                detect.GetAvgUDLR_HVOPerChannel(channel);

            }
         
        }

    
    }

   


}
