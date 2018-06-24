using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;

namespace PDI
{

    public partial class Form1 : Form
    {
        Bitmap lastBitMap = null;
        Img imagen = new Img();
        Emgu.CV.VideoWriter w;
        private void GetFiles()
        {
            imagen.path = browseBox.Text; // "D:\\USBGDrive\\MAESTRIA\\LTER - Moorea";
            imagen.GetFiles();

            filesBS.DataSource = imagen.dir.table;

        }
        double factor = 0.7;
        double factorDiago = 0.98;

    
     
        public Form1()
        {
            InitializeComponent();

           // Emgu.CV.gpu
         
            GetFiles();

        }

   
        private void getRGB_Click(object sender, EventArgs e)
        {

            //  img = img.SmoothBlur(1,1);
            // img= img.Sobel(1, 1, 1).Convert<Rgba,Byte>();
            //  Image<Rgba, byte> redImg = imagen.GetRed();

            imagen.GetChannels();

            int val = Convert.ToInt32(numericUpDown1.Value);
            rgbBox.Image = imagen.RGB[val].Bitmap;
          

        }
        private void substract_Click(object sender, EventArgs e)
        {
            int val = Convert.ToInt32(numericUpDown1.Value);
            imagen.ElementSubstraction();
            lastBitMap = imagen.ElementSubs[val].Bitmap;
            segmentBox.Image = lastBitMap;

        }
            private void threshold_Click(object sender, EventArgs e)
        {

            double d = Convert.ToDouble(textBox1.Text);
            double max = Convert.ToDouble(textBox2.Text);
            int val = Convert.ToInt32(numericUpDown1.Value);

            imagen.Threshold(d, max, val);

            lastBitMap = imagen.Thres[val].Bitmap;

            rgbBox.Image = lastBitMap;

       
        }

        private void divide_Click(object sender, EventArgs e)
        {


            int val = Convert.ToInt32(numericUpDown1.Value);

            //   ConvolutionKernelF kern = new ConvolutionKernelF(new float[,] { { 0, -1, 0 }, { 0, -1, 0 }, {0, -1, 0}  });
            // clone = clone.Convolution(kern).Convert<Rgb,byte>();
            imagen.Divide(val);
            Image<Rgb, byte> redImg2;// = new Image<Rgb, Byte>(lastBitMap);

            redImg2 = imagen.Divs[val];//.Convolution(kern).Convert<Rgb,byte>();

            lastBitMap = redImg2.Bitmap;

            segmentBox.Image = lastBitMap;
          
        }
       
      


        private void figureClick(object sender, EventArgs e)
        {
            if (lastBitMap == null) return;
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(lastBitMap);
            Button btn = (Button)sender;
            int what = Convert.ToInt32(btn.Tag);
            bool draw = true;


            //line
            double cth = Convert.ToDouble(cannyThreshold.Text);
            double cthlink = Convert.ToDouble(cannyThreshLinking.Text);
            //circle
            double accum = Convert.ToDouble(cannyCircleAccum.Text);
            double dp = Convert.ToDouble(dpbox.Text);
            double minDist = Convert.ToDouble(minDistbox.Text);
            double minRadio = Convert.ToDouble(minRadiobox.Text);
            //lines
            double rho = Convert.ToDouble(rhoBox.Text);
            double threshold = Convert.ToDouble(thresholdbox.Text);
            double minWithLine = Convert.ToDouble(minWidthbox.Text);
            double gap = Convert.ToDouble(gapbox.Text);
            //trianRect
            double epsilonFact = Convert.ToDouble(epsilonBox.Text);
            double minArea = Convert.ToDouble(minAreabox.Text);

            imagen.args = new double[] { cth, cthlink,
                accum, dp,
                minDist, minRadio, rho,
                threshold, minWithLine,gap ,
                epsilonFact , minArea };

            imagen.detect = new Detector();

            imagen.BeginDetection(ref redImg2, what, draw);

            
            redImg2 = imagen.detect.figure[what];


            richTextBox1.Text = imagen.detect.msgBuilder.ToString();
            imagen.detect.msgBuilder.Clear();

            segmentBox.Image =redImg2.Bitmap;



        }



        private static void SlopeCutoof(ref Image<Bgr, byte> redImg2, LineSegment2D seg)
        {
            if (seg.P2.X - seg.P1.X != 0)
            {
                double slope = (seg.P2.Y - seg.P1.Y);
                slope /= (seg.P2.X - seg.P1.X);
                double cutoff = seg.P1.Y - (slope * seg.P1.X);
                double cutoff2 = seg.P2.Y - (slope * seg.P2.X);

                int x = seg.P1.X;


                while (x <= seg.P2.X)
                {
                    int y = Convert.ToInt32(slope * x + cutoff);
                    redImg2.Bitmap.SetPixel(x, y, Color.Black);
                    //redImg2.Bitmap.SetPixel(seg.P1.X, seg.P1.Y, Color.Black);
                    x++;
                }
            }
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
          DialogResult r =   this.folderBrowserDialog1.ShowDialog();
           if( r == DialogResult.OK)
            {
                browseBox.Text = this.folderBrowserDialog1.SelectedPath;
                GetFiles();
            }
        }

        private void filesBS_PositionChanged(object sender, EventArgs e)
        {
            DataRowView drv = (DataRowView)filesBS.Current;
            ImgDB.FilesRow first = (ImgDB.FilesRow)drv.Row;
            string filename = first.Path + first.Filename;

           

            originalBox.Image = imagen.GetBitMap(filename, 5);
            imagen.GetBasicInfo();
            basicInfoBindingSource.DataSource = imagen.BInfo.table;

         //   if (w != null) w.Dispose();
           // int Codec = Emgu.Util.C('D', 'I', 'V', '3');
          
          //  w = new VideoWriter(filename + ".mp4",-1, 5, new Size(200,200), true);
           


            getRGB_Click(sender, e);
            threshold_Click(sender, e);

            lastSum = 0;
            imagen.rotated = null;

        }

      
       

        private void erodeBtn_Click(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(segmentBox.Image);
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(bm);

            
            redImg2 = redImg2.Erode(Convert.ToInt32(erodeBox.Text));




            segmentBox.Image = redImg2.Bitmap;
        }

        private void dilateBtn_Click(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(segmentBox.Image);
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(bm);


            redImg2 = redImg2.Dilate(Convert.ToInt32(dilatebox.Text));




            segmentBox.Image = redImg2.Bitmap;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            getRGB_Click(sender, e);
           // threshold_Click(sender, e);
        }

       

        private void iterateBtn_Click(object sender, EventArgs e)
        {

            factor = 0.6;
            factorDiago = 0.90;
            

            //    lastBitMap = imagen.escaledUI.CopyBlank().Bitmap;
           
            BorderIter(0);


            /*
            factor = 0.1;
            BorderIter(1);

            */

        }

      
        private void  BorderIter(int type)
        {
            
            // Image<Rgb,byte> result = new Image<Rgb, byte>()
            IterateChannels(type);


         



        }
        double lastSum = 0;

        private void IterateChannels(int type)
        {


            imagen.GetChannels();


            //line
            double cth = Convert.ToDouble(cannyThreshold.Text);
            double cthlink = Convert.ToDouble(cannyThreshLinking.Text);
            //circle
            double accum = Convert.ToDouble(cannyCircleAccum.Text);
            double dp = Convert.ToDouble(dpbox.Text);
            double minDist = Convert.ToDouble(minDistbox.Text);
            double minRadio = Convert.ToDouble(minRadiobox.Text);
            //lines
            double rho = Convert.ToDouble(rhoBox.Text);
            double threshold = Convert.ToDouble(thresholdbox.Text);
            double minWithLine = Convert.ToDouble(minWidthbox.Text);
            double gap = Convert.ToDouble(gapbox.Text);
            //trianRect
            double epsilonFact = Convert.ToDouble(epsilonBox.Text);
            double minArea = Convert.ToDouble(minAreabox.Text);


            // int val = Convert.ToInt32(numericUpDown1.Value);
            imagen.args = new double[] { cth, cthlink,
                accum, dp,
                minDist, minRadio, rho,
                threshold, minWithLine,gap ,
                epsilonFact , minArea };

            int detectType = 2; //lines

            int step = 10;
            double percent = 0.25;

            double max = 250;
            double valor = 150;

            if (type == 0)
            {
                factor = 0.6;
                factorDiago = 0.90;
            }
            else if (type == 1)
            {
                factor = 0.1;
            }


            Image<Rgb, byte> final = imagen.escaledUI.Convert<Rgb, byte>().CopyBlank();


            imagen.detect = new Detector();

            for (int channel = 0; channel < 3; channel++)
            {

                valor = 150;

                Image<Rgb, byte> result=null;
              
                imagen.ElementSubsroutine(detectType, step, max, valor, channel, ref result, factorDiago);

                //show last
                //segmentBox.Image = result.Bitmap;

                //print
                result = imagen.detect.raw.CopyBlank();
                imagen.detect.DrawDiagonals(ref result, channel);
                final = final.Add(result).Clone();
                //show last
               // segmentBox.Image = final.Bitmap;
                ///////////////////////////////////////////////////
              //  MessageBox.Show("Diagonals");

            }


            segmentBox.Image = final.Bitmap;


            Rgb r = new Rgb(255, 255, 255);
            final = final.InRange(r, r).Convert<Rgb, byte>();
            this.rgbBox.Image = final.Bitmap;

            //
            ///   MessageBox.Show("1.2");

            Rgb[] color = new Rgb[3];
            imagen.detect.PickColorsAvg(type, ref color);
            Image<Rgb, byte>[] arr = imagen.detect.DrawDetectedAvg(ref final, ref color, false);




            //   MessageBox.Show("rotacion");

            LineSegment2D pos = imagen.detect.avgDiagonalPos;
            LineSegment2D neg = imagen.detect.avgDiagonalNeg;

            double[] angle = new double[5];
          
            double refAngle = 90;
            double refAngle2 = refAngle-90;

            string text;
            imagen.FindAngle(pos, neg, refAngle, out angle[0], out text);

            richTextBox1.Clear();
            richTextBox1.AppendText("Pos,Neg\t" + text);

            LineSegment2D refPos = imagen.detect.RefPos;
            LineSegment2D refNeg = imagen.detect.RefNeg;

            imagen.FindAngle(pos, refPos, refAngle2, out angle[1], out text);
            richTextBox1.AppendText("Pos,refPos\t" + text);
            imagen.FindAngle(neg, refPos, refAngle, out angle[2], out text);
            richTextBox1.AppendText("Neg,refPos\t" + text);

            imagen.FindAngle(pos, refNeg, -1* refAngle, out angle[3], out text);
            richTextBox1.AppendText("Pos,refNeg\t" + text);
            imagen.FindAngle(neg, refNeg, refAngle2, out angle[4], out text);
            richTextBox1.AppendText("Neg,refNeg\t" + text);

            double avg1 = (angle[1] + angle[2])/2;
            double avg2 = (angle[3] + angle[4])/2;
            richTextBox1.AppendText("avg1,avg2\t" + avg1.ToString()+","+avg2.ToString());

            double sum = avg1+avg2;
            double avg = (avg1 - avg2) / 2;
            richTextBox1.AppendText("sum\t" + sum.ToString());

       

            if (sum <= Math.Abs(lastSum) && imagen.rotated != null)
            {
                imagen.rotated = imagen.escaledUI;
                richTextBox1.AppendText("SE HA ACABAO" + lastSum.ToString());

            }
            else if (!double.IsNaN(avg))
            {
                imagen.rotated = imagen.escaledUI.Rotate(avg, new Rgba(255, 255, 255, 255));
                lastSum = sum;

                MessageBox.Show("rotated " + Decimal.Round(Convert.ToDecimal(avg1.ToString()), 2));

            }
            else 
            {
                MessageBox.Show("Problem with avg ");

            }

            segmentBox.Image = rgbBox.Image;
            rgbBox.Image = imagen.rotated.Bitmap;


            imagen.escaledUI = imagen.rotated;

          //  originalBox.Image = imagen.escaledUI.Bitmap;

            // return;

            /////////////////
            ///////////
            for (int channel = 0; channel < 3; channel++)
            {

                valor = 1;

                Image<Rgb, byte> result = null;

                imagen.DivideRoutine(detectType, step, percent, max, valor, channel, ref result, factor);

               // segmentBox.Image = result.Bitmap;


                result = imagen.detect.raw.CopyBlank();
                //i iterador es el segmento arriba abajo iz derecha
                imagen.detect.DrawUDLR(ref result, channel);
                final = final.Add(result).Clone();
                //   segmentBox.Image = result.Bitmap;

                /// MessageBox.Show("UDLRs");
            }

            //
            /// MessageBox.Show("0");

            segmentBox.Image = final.Bitmap;

            // MessageBox.Show("1.1");

            r = new Rgb(255, 255, 255);
            final = final.InRange(r, r).Convert<Rgb, byte>();
            this.rgbBox.Image = final.Bitmap;

            //
              MessageBox.Show("1.2");

             color = new Rgb[3];
            imagen.detect.PickColorsAvg(type, ref color);
           arr = imagen.detect.DrawDetectedAvg(ref final, ref color, false);

            //


            this.rgbBox.Image = arr[0].Bitmap;
            //   MessageBox.Show(".0");
            this.rgbBox.Image = arr[1].Bitmap;
            //
            //      MessageBox.Show(".1"); 
            ///    
            this.rgbBox.Image = arr[2].Bitmap;

            // MessageBox.Show(".2");

            final = final.Add(arr[0].Add(arr[1]).Add(arr[2]));
            this.rgbBox.Image = final.Bitmap;






            MessageBox.Show("finito");

        }

     



        private void segmentBtn_Click(object sender, EventArgs e)
        {
            LineSegment2D pos = new LineSegment2D(new Point(0, 0), new Point(imagen.escaledUI.Width*3/4, imagen.escaledUI.Height*1/4));

            LineSegment2D d = new LineSegment2D(new Point(imagen.escaledUI.Width, 0), new Point(0, imagen.escaledUI.Height));
            
            LineSegment2D[] araytest = new LineSegment2D[] { pos };

            segmentBox.Image = imagen.detect.DrawLines(new Rgb(Color.Yellow), ref araytest).Bitmap;
            MessageBox.Show("finito");
        }
    }


  

}
