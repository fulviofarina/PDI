using Emgu.CV;
using Emgu.CV.Structure;
using PDILib;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PDIUI
{

    public partial class Form1 : Form
    {
        Bitmap lastBitMap = null;
        Img imagen = new Img();
      //  Emgu.CV.VideoWriter w;
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
            string filename =  first.Filename;


            imagen.GetImg(filename, 5);
            originalBox.Image = imagen.UIOne.Bitmap;
            imagen.GetBasicInfo();
            basicInfoBindingSource.DataSource = imagen.BInfo.table;

         //   if (w != null) w.Dispose();
           // int Codec = Emgu.Util.C('D', 'I', 'V', '3');
          
          //  w = new VideoWriter(filename + ".mp4",-1, 5, new Size(200,200), true);
           


            getRGB_Click(sender, e);
            threshold_Click(sender, e);

            lastSum = 0;
            imagen.imgUtil.rotated = null;

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

       

        private void diagonalsAndRotation_Click(object sender, EventArgs e)
        {

         //   factor = 0.6;
            factorDiago = 0.90;
            

            //    lastBitMap = imagen.escaledUI.CopyBlank().Bitmap;
           
           
            DiagonalsAndRotation2(0); //2 is Sobel way

            /*
            factor = 0.1;
            BorderIter(1);

            */

        }

        private void borders_Click(object sender, EventArgs e)
        {

            factor = 0.1;
          //  factorDiago = 0.90;


            //    lastBitMap = imagen.escaledUI.CopyBlank().Bitmap;


            UDLR(0);

            /*
            factor = 0.1;
            BorderIter(1);

            */

        }



        double lastSum = 0;




        private void UDLR(int type)
        {
            // return;

            imagen.GetChannels();

            extractArgs();

           
            if (type == 0)
            {
                factor = 0.6;
                factorDiago = 0.90;
            }
            else if (type == 1)
            {
                factor = 0.1;
            }
        


            imagen.detect = new Detector();

            Image<Rgb, byte> final = imagen.UIOne.Convert<Rgb, byte>().CopyBlank();

         
                double resetvalor = 1;
            int detectType = 2; //lines
            int step = 10;
            double percent = 0.25;
            double max = 250;
              imagen.DivideRoutine(detectType, step, percent, max, resetvalor);



            this.rgbBox.Image = final.Bitmap;
            MessageBox.Show("0");



            imagen.GetUDLRRoutine(percent, factor);
            imagen.detect.DrawUDLR(ref  final);
            imagen.detect.GetAvgUDLR(true);
            //
            /// MessageBox.Show("0");
            this.rgbBox.Image = final.Bitmap;




            Rgb r = new Rgb(255, 255, 255);
             final = final.InRange(r, r).Convert<Rgb, byte>().Clone();

           
            this.segmentBox.Image = final.Bitmap;
            //
            final = final.CopyBlank();

            Rgb[] color = new Rgb[3];
            imagen.detect.PickColorsAvg(type, ref color);
            Image<Rgb, byte>[] arr = imagen.detect.DrawDetectedAvg(ref final, ref color, false);

            //


            printSteps(ref final, ref arr);

           
        }
        private void DiagonalsAndRotation(int type)
        {
            imagen.GetChannels();

            extractArgs();

            int detectType = 2; //lines

            int step = 10;


            double max = 250;


            if (type == 0)
            {
              
                factorDiago = 0.90;
            }
        

            double resetvalor0 = 150;

            imagen.detect = new Detector();



            Image<Rgb, byte> final = imagen.UIOne.Convert<Rgb, byte>().CopyBlank();

            imagen.ElementSubsRoutine(detectType, step, max, resetvalor0);

            imagen.GetDiagonalsRoutine(factorDiago);
            imagen.detect.DrawDiagonals(ref final);


            imagen.detect.GetAvgDiagonalsPosNeg(true);

            this.rgbBox.Image = final.Bitmap;

            //  MessageBox.Show("1.1");

            Rgb r = new Rgb(255, 255, 255);
            final = final.InRange(r, r).Convert<Rgb, byte>().Clone();

            this.segmentBox.Image = final.Bitmap;

            //
            final = final.CopyBlank();
            //  MessageBox.Show("1.2");

            Rgb[] color = new Rgb[3];
            imagen.detect.PickColorsAvg(type, ref color);
            Image<Rgb, byte>[] arr = imagen.detect.DrawDetectedAvg(ref final, ref color, false);


            printSteps(ref final, ref arr);

            //  MessageBox.Show("1.2");

            imagen.FindRotation(lastSum);


            this.richTextBox1.Text = imagen.detect.msgBuilder.ToString();
            this.segmentBox.Image = rgbBox.Image;
            this.rgbBox.Image = imagen.imgUtil.rotated.Bitmap;


            imagen.UIOne = imagen.imgUtil.rotated;

            //  originalBox.Image = imagen.escaledUI.Bitmap;



        }
        private void DiagonalsAndRotation2(int type)
        {
            imagen.GetChannels();

            extractArgs();


            if (type == 0)
            {
              
                factorDiago = 0.90;
            }
        

         

            imagen.detect = new Detector();



            Image<Rgb, byte> final = imagen.UIOne.Convert<Rgb, byte>().CopyBlank();

            int resetvalor = 11;
            int detectType = 2; //lines
            int step = 2;
            double threshold = 250;
            int max = 19;
            int xorder = Convert.ToInt16(xorderbox.Text);
            int yorder = Convert.ToInt16(yorderbox.Text);

            imagen.SobelRoutine(detectType, step, threshold, max, resetvalor,xorder,yorder);

            imagen.GetDiagonalsRoutine( factorDiago);
            imagen.detect.DrawDiagonals(ref final);

         
            imagen.detect.GetAvgDiagonalsPosNeg(true);

            this.rgbBox.Image = final.Bitmap;

            //  MessageBox.Show("1.1");

            Rgb r = new Rgb(255, 255, 255);
            final = final.InRange(r, r).Convert<Rgb, byte>().Clone();
           
            this.segmentBox.Image = final.Bitmap;

            //
            final = final.CopyBlank();
          //  MessageBox.Show("1.2");

            Rgb[] color = new Rgb[3];
            imagen.detect.PickColorsAvg(type, ref color);
            Image<Rgb, byte>[] arr = imagen.detect.DrawDetectedAvg(ref final, ref color, false);


            printSteps(ref final, ref arr);

          //  MessageBox.Show("1.2");

            imagen.FindRotation(lastSum);


            this.richTextBox1.Text = imagen.detect.msgBuilder.ToString();
            this.segmentBox.Image = rgbBox.Image;
            this.rgbBox.Image = imagen.imgUtil.rotated.Bitmap;


            imagen.UIOne = imagen.imgUtil.rotated;

            //  originalBox.Image = imagen.escaledUI.Bitmap;



        }

        private void extractArgs()
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


            // int val = Convert.ToInt32(numericUpDown1.Value);
            imagen.args = new double[] { cth, cthlink,
                accum, dp,
                minDist, minRadio, rho,
                threshold, minWithLine,gap ,
                epsilonFact , minArea };
        }

        private void printSteps(ref Image<Rgb, byte> final, ref Image<Rgb, byte>[] arr)
        {
            this.rgbBox.Image = arr[0].Bitmap;
            //   MessageBox.Show(".0");
            this.rgbBox.Image = arr[1].Bitmap;
            //
            //      MessageBox.Show(".1"); 
            ///    
            this.rgbBox.Image = arr[2].Bitmap;

            // MessageBox.Show(".2");

            final = final.Add(arr[0].Add(arr[1]).Add(arr[2])).Clone();
            this.rgbBox.Image = final.Bitmap;
            
        }




        private void segmentBtn_Click(object sender, EventArgs e)
        {

            imagen.GetImg(imagen.curentfilename, 10);
            imagen.GetImgToCompare(imagen.curentfilename, 10);
            segmentBox.Image = imagen.imgUtil.expandedTwo[1].Bitmap;
          //  segmentBox.Image = imagen.expandedEscaledUI[1].Bitmap;
            /*
            LineSegment2D pos = new LineSegment2D(new Point(0, 0), new Point(imagen.escaledUI.Width*3/4, imagen.escaledUI.Height*1/4));

            LineSegment2D d = new LineSegment2D(new Point(imagen.escaledUI.Width, 0), new Point(0, imagen.escaledUI.Height));
            
            LineSegment2D[] araytest = new LineSegment2D[] { pos };

            segmentBox.Image = imagen.detect.DrawLines(new Rgb(Color.Yellow), ref araytest).Bitmap;
    */
            MessageBox.Show("finito");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int val = Convert.ToInt32(numericUpDown1.Value);
            imagen.Sobel(val,1,1,Convert.ToInt32(apertureBox.Text));
            lastBitMap = imagen.Soby[val].Bitmap;
            segmentBox.Image = lastBitMap;
        }
    }


  

}
