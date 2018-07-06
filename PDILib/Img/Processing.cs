using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace PDILib
{



    public partial class Img
    {

        public void Threshold(double d, double max, int channel)
        {
            Rgba r = PureColor(channel, d);
            Rgba g = PureColor(channel, max);
            Thres[channel] = RGB[channel].ThresholdToZeroInv(r);

            Rgba black = new Rgba(0, 0, 0, 0);
            Rgba set = new Rgba(255, 255, 255, 0);
            SwitchColor(ref Thres[channel], black, set);

          
        }
        public void Sobel(int channel, int xorder, int yorder, int aperture)
        {
          
            Soby[channel] = UIOne[channel].Sobel(xorder,yorder,aperture).Convert<Rgba,byte>();


        }

        public void Threshold(double d, double max)
        {
           
            for (int i = 0; i < 4; i++)
            {
                Threshold(d, max, i);
            }
        }
        public void ElementSubstraction()
        {
            for (int i = 0; i < 4; i++)
            {
                ElementSubstraction(i);
            }
        }

      

            public void ElementSubstraction( int channel)
        {

            Image<Rgba, byte>[] otherChannels = GetOtheChannels(channel,ref RGB);
            ElementSubs[channel] = Thres[channel];
            foreach (Image<Rgba, byte> item in otherChannels)
            {
                ElementSubs[channel] = ElementSubs[channel].Add(item);

            }
        }

        public void Divide()
        {
            for (int i = 0; i < 3; i++)
            {
                Divide(i);
            }

        }


        public void Divide(int channel)
        {
            Image<Rgba, byte> input2 = Thres[channel];
            Image<Rgba, byte> input = UIOne;
            Image<Rgba, byte> result = null;
            // Divs[channel] = input.Mul(input2.Pow(-1)).Convert<Rgb, byte>();

            result = input2.Sub(input).Not();

            SwitchColor(ref result,  imgUtil.pitchWhite, imgUtil.pitchBlack);

            Divs[channel] = result.Convert<Rgb, byte>();

        }
        public void FindRotation(double lastSum)
        {


            //   MessageBox.Show("rotacion");

            LineSegment2D pos = detect.avgDiagonalPos;
            LineSegment2D neg = detect.avgDiagonalNeg;

            double[] angle = new double[5];

            double refAngle = 90;
            double refAngle2 = refAngle - 90;

            string text;
            FindAngle(pos, neg, refAngle, out angle[0], out text);

            // richTextBox1.Clear();
            StringBuilder b = detect.msgBuilder;
            b.Clear();
            b.Append("Pos,Neg\t" + text);

            LineSegment2D refPos = detect.RefPos;
            LineSegment2D refNeg = detect.RefNeg;

            FindAngle(pos, refPos, refAngle2, out angle[1], out text);
            b.Append("Pos,refPos\t" + text);
            FindAngle(neg, refPos, refAngle, out angle[2], out text);
            b.Append("Neg,refPos\t" + text);

            FindAngle(pos, refNeg, -1 * refAngle, out angle[3], out text);
            b.Append("Pos,refNeg\t" + text);
            FindAngle(neg, refNeg, refAngle2, out angle[4], out text);
            b.Append("Neg,refNeg\t" + text);

            double avg1 = (angle[1] + angle[2]) / 2;
            double avg2 = (angle[3] + angle[4]) / 2;
            b.Append("avg1,avg2\t" + avg1.ToString() + "," + avg2.ToString());

            double sum = avg1 + avg2;
            double avg = (avg1 - avg2) / 2;
            b.Append("sum\t" + sum.ToString());



            if (sum <= Math.Abs(lastSum) && imgUtil.rotated != null)
            {
                imgUtil.rotated = UIOne;
                b.Append("\nSE HA ACABADO" + lastSum.ToString());

            }
            else if (!double.IsNaN(avg))
            {
                imgUtil.rotated = UIOne.Rotate(avg, imgUtil.pitchWhite,true);
                lastSum = sum;
                decimal angl = Decimal.Round(Convert.ToDecimal(avg1.ToString()), 2);
                b.Append("rotated " + angl);

            }
            else
            {
                imgUtil.rotated = UIOne;
                b.Append("Problem with avg ");

            }
        }
        public void ThresholdClassic(double d, double max, int channel)
        {
            Rgba r = PureColor(channel, d);
            Rgba g = PureColor(channel, max);
            Thres[channel] = RGB[channel].ThresholdBinary(r, g);
           // Image<Gray, byte> Imae = RGB[channel].ThresholdBinary(r, g).Convert<Gray, byte>();
           // Image<Gray, byte>[] Imae2 = new Image<Gray, byte>[] { Imae, Imae, Imae, Imae };
           // Thres[channel] = new Image<Rgba, byte>(Imae2);
            //  Thres[channel] = RGB[channel].ThresholdToZero(r);
            //Thres[channel] = RGB[channel].ThresholdToZeroInv(r);
        }

      

    }


}
