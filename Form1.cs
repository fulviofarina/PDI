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

    public partial class Form1 
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

    }
    public partial class Form1 : Form
    {
     
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

            imagen.GetChannel(0);
            imagen.GetChannel(1);
            imagen.GetChannel(2);
            imagen.GetChannel(3);
            int val = Convert.ToInt32(numericUpDown1.Value);
            rgbBox.Image = imagen.RGB[val].Bitmap;
          

        }

        private void threshold_Click(object sender, EventArgs e)
        {
            //  Bitmap bm = new Bitmap(pictureBox2.Image);
            //= new Image<Rgba, Byte>(bm);

            double d = Convert.ToDouble(textBox1.Text);
            double max = Convert.ToDouble(textBox2.Text);

            imagen.Threshold(d, max);
            int val = Convert.ToInt32(numericUpDown1.Value);



            rgbBox.Image = imagen.Thres[val].Bitmap;


            // redImg2 = imagen.Thres[0].Convert<Rgb, byte>();
            // redImg2 = redImg2.Sobel(1, 1, 13).Convert<Rgb, byte>();
            lastBitMap = imagen.Thres[val].Bitmap;

            segmentBox.Image = lastBitMap;

       
        }

        private void divide_Click(object sender, EventArgs e)
        {

            Bitmap bm = new Bitmap(segmentBox.Image);
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(bm);

            imagen.Divide();
            int val = Convert.ToInt32(numericUpDown1.Value);
            redImg2 = imagen.Divs[val];

            lastBitMap = redImg2.Bitmap;

            segmentBox.Image = lastBitMap;
          
        }
       
        private void segment_Click(object sender, EventArgs e)
        {

           

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

        }

        private void figureClick(object sender, EventArgs e)
        {
            if (lastBitMap == null) return;
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(lastBitMap);
            Button btn = (Button) sender;
            int what = Convert.ToInt32(btn.Tag);
            imagen.GetDetection(ref redImg2,what);


            Rgb std = new Rgb(0, 255, 0);
            Rgb std2 = std;
          
            if (what == 1) std = new Rgb(200, 0, 255);
            else if (what == 3)
            {
                std = new Rgb(0, 0, 255);
                std2 = new Rgb(100, 200, 255);
            }

             if (what==1) imagen.detect.DrawCircles(std);
             else if (what ==2)  imagen.detect.DrawLines(std);
             else if (what ==3) imagen.detect.DrawTriRect(std,std2);

            // segmentBox.Image = imagen.detect.circleImage.Bitmap;
            richTextBox1.Text = imagen.detect.msgBuilder.ToString();
            segmentBox.Image = imagen.detect.figure[what].Bitmap;



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

            BorderIter(sender, e);

            factor = 0.1;


            BorderIter(sender, e);



        }

      
        private Image<Rgb, byte>  BorderIter(object sender, EventArgs e)
        {
            // Image<Rgb,byte> result = new Image<Rgb, byte>()
            IterateChannels(sender, e);
            //print
            Image<Rgb, byte> result = printAll();

            MessageBox.Show("no");

            imagen.detect.lines = imagen.detect.GetAvgUDLR(true);
            Image<Rgb, byte> nuevoResult = imagen.detect.DrawLines(new Rgb(Color.White), false);

            this.rgbBox.Image = nuevoResult.Bitmap;

            imagen.detect.lines = imagen.detect.GetAvgDiagonalsPosNeg(true);
            nuevoResult = imagen.detect.DrawLines(new Rgb(Color.White), true);

            this.rgbBox.Image = nuevoResult.Bitmap;


            MessageBox.Show("0");
            return result;
        }

    
        private void IterateChannels(object sender, EventArgs e)
        {
            double d = 10;
            double max = 250;
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
            //  double threshold = Convert.ToDouble(thresholdbox.Text);
            double minWithLine = Convert.ToDouble(minWidthbox.Text);
            double gap = Convert.ToDouble(gapbox.Text);
            //trianRect
            double epsilonFact = Convert.ToDouble(epsilonBox.Text);
            double minArea = Convert.ToDouble(minAreabox.Text);

            int detectType = 2; //lines

            int step = 20;
            double percent = 0.25;


            Image<Rgb, byte> result = null;

            for (int channel = 0; channel < 3; channel++)
            {
                List<LineSegment2D[]> ls = new List<LineSegment2D[]>();

                for (double val = d; val <= max; val += step)
                {
                    // textBox1.Text = val.ToString();
                    imagen.Threshold(val, max, channel);
                    // int val = Convert.ToInt32(numericUpDown1.Value);
                    imagen.args = new double[] { cth, cthlink,
                accum, dp,
                minDist, minRadio, rho,
                val, minWithLine,gap ,
                epsilonFact , minArea };
                    //  thresholdbox.Text = val.ToString();
                    //segment_Click(sender, e);
                    result = imagen.Thres[channel].Convert<Rgb, byte>();
                    imagen.GetDetection(ref result, detectType);
                    ls.Add(imagen.detect.lines);

                }
                imagen.detect.SelectMany(channel, ref ls);

                imagen.detect.GetUDLR_HVOPerChannel(factor, channel, percent, true);

                imagen.detect.GetDiagonalsPosNegPerChannel(factorDiago, channel);

            }


            for (int channel = 0; channel < 3; channel++)
            {
                imagen.detect.GetAvgUDLR_HVOPerChannel(channel);
                imagen.detect.GetAvgDiagonalsPosNegPerChannel(channel);
            }


        }

        private Image<Rgb, byte> printAll()
        {
            
            Image<Rgb, byte> result = null;
            result = imagen.detect.raw.CopyBlank();

            ///ARREGLAR ACA
            for (int channel = 0; channel < 3; channel++)
            {
                //channels, UDLR, horizontals verticals & others
                //i iterador es el segmento arriba abajo iz derecha

                for (int q = 0; q < 4; q++)
                {
                    for (int type = 0; type < 3; type++)
                    {
                        LineSegment2D[] aux = imagen.detect.chUDLR_HVO[channel, q, type];
                        Image<Rgb, byte> nuevoResult = imagen.detect.DrawLines(channel, ref aux);
                        result = result.Add(nuevoResult).Clone();
                        segmentBox.Image = result.Bitmap;

                        //  MessageBox.Show(channel.ToString() + " " +q.ToString() + " " + type.ToString());
                    }
                }
                for (int posNeg = 0; posNeg < 2; posNeg++)
                {
                    LineSegment2D[] aux = imagen.detect.Diagonals[channel, posNeg];
                    result = result.Add(imagen.detect.DrawLines(channel, ref aux)).Clone();
                    segmentBox.Image = result.Bitmap;

                }

            }

            return result;
        }

     
    }


  

}
