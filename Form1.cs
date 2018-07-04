using Emgu.CV;
using Emgu.CV.Structure;
using PDILib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
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
         
           

        }

   
        private void getRGB_Click(object sender, EventArgs e)
        {

            //  img = img.SmoothBlur(1,1);
            // img= img.Sobel(1, 1, 1).Convert<Rgba,Byte>();
            //  Image<Rgba, byte> redImg = imagen.GetRed();

            imagen.GetChannels();

            int val = Convert.ToInt32(numericUpDown1.Value);
            rgbbox.Image = imagen.RGB[val].Bitmap;
          

        }
        private void substract_Click(object sender, EventArgs e)
        {
            int val = Convert.ToInt32(numericUpDown1.Value);
            imagen.ElementSubstraction(val);
            lastBitMap = imagen.ElementSubs[val].Bitmap;
            segmentBox.Image = lastBitMap;

        }
            private void threshold_Click(object sender, EventArgs e)
        {

            double d = Convert.ToDouble(thresholdBinarybox.Text);
            double max = Convert.ToDouble(thresholdBinaryMAxbox.Text);
            int val = Convert.ToInt32(numericUpDown1.Value);

            imagen.Threshold(d, max, val);

            lastBitMap = imagen.Thres[val].Bitmap;

            rgbbox.Image = lastBitMap;

       
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
            Bitmap bm = new Bitmap(segmentBox.Image);
           // Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(bm);
            Image<Rgb, byte> redImg2 = new Image<Rgb, Byte>(bm);
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
            this.basicInfoDGV.AutoGenerateColumns = true;
            this.basicInfoDGV.DataSource = this.basicInfoBindingSource;

           
                
           // histogramBox.ZedGraphControl.SaveFileDialog.OverwritePrompt = true;
           // histogramBox.ZedGraphControl.SaveFileDialog.CreatePrompt = false;
           // histogramBox.ZedGraphControl.SaveAs(imagen.path + "\\" + imagen.curentfilename.Replace(".jpg", ".Histo.jpg"));

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



            this.rgbbox.Image = final.Bitmap;
            MessageBox.Show("0");



            imagen.GetUDLRRoutine(percent, factor);
            imagen.detect.DrawUDLR(ref  final);
            imagen.detect.GetAvgUDLR(true);
            //
            /// MessageBox.Show("0");
            this.rgbbox.Image = final.Bitmap;




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

            this.rgbbox.Image = final.Bitmap;

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
            this.segmentBox.Image = rgbbox.Image;
            this.rgbbox.Image = imagen.imgUtil.rotated.Bitmap;


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
            double threshold = 220;
            int max = 19;
            int xorder = Convert.ToInt16(xorderbox.Text);
            int yorder = Convert.ToInt16(yorderbox.Text);

            imagen.SobelRoutine(detectType, step, threshold, max, resetvalor,xorder,yorder);

            imagen.GetDiagonalsRoutine( factorDiago);
            imagen.detect.DrawDiagonals(ref final);

         
            imagen.detect.GetAvgDiagonalsPosNeg(true);

            this.rgbbox.Image = final.Bitmap;

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
            this.segmentBox.Image = rgbbox.Image;


            //factores

            Image<Rgba, byte> other = imagen.imgUtil.rotated.Clone();


            ImgDB.BasicInfoRow red = imagen.BInfo.table.FirstOrDefault( o=> o.Channel==0);
            ImgDB.BasicInfoRow green = imagen.BInfo.table.FirstOrDefault(o => o.Channel == 1);
            ImgDB.BasicInfoRow blue = imagen.BInfo.table.FirstOrDefault(o => o.Channel == 2);
            ImgDB.BasicInfoRow alpha = imagen.BInfo.table.FirstOrDefault(o => o.Channel == 3);

          

            //imagen.imgUtil.rotated._EqualizeHist() ;
            Img.ChangeColor(ref imagen.imgUtil.rotated, (float)red.Factor, (float)green.Factor, (float)blue.Factor, (float)alpha.Factor);
            //imagen.UIOne = imagen.imgUtil.rotated;
            this.rgbbox.Image = imagen.imgUtil.rotated.Bitmap;
           // this.matrixBox.Matrix = imagen.imgUtil.rotated.Mat.Data;

            imagen.imgUtil.rotated.Save(imagen.path+ "\\" + imagen.curentfilename.Replace(".jpg", ".out.jpg"));
            //  originalBox.Image = imagen.escaledUI.Bitmap;


            float sum = (float)(red.Avg + green.Avg + blue.Avg);
            sum /= 3;
            float rF = 1;
            float gF = 1;
            float bF = 1;
            float AF = 1;
            rF = (float)(sum / red.Avg);
            gF = (float)(sum / green.Avg);
            bF = (float)(sum / blue.Avg);
           

            Img.ChangeColor(ref other, rF, gF, bF, AF);


            // CvInvoke.CLAHE(other, 1, new Size(10, 10), other);
            double radius = Img.CalculateDiagonalLenght(other.Width, other.Height);
            radius /= 2;
             Point p = imagen.GetMiddlePointCanvas(radius, ref other);
            Rectangle rec = new Rectangle(p.X , p.Y , 2, 2);
          //  Rectangle rec = new Rectangle(p.X-(p.X*1/5), p.Y+ (p.Y * 3 / 2), 150,150);
          //  other.ROI = rec;
           Image<Rgba,byte> pantone  = other.GetSubRect(rec);
          //  other.ROI = rec;
            Point ul = new Point(rec.X, rec.Y);
            Point bl = new Point(rec.Left, rec.Bottom);
            Point ur = new Point(rec.Right, rec.Top);
            Point br = new Point(rec.Right, rec.Bottom);
            other.DrawPolyline(new Point[] { ul, bl, br, ur }, true, new Rgba(255, 0, 0, 255), 2, Emgu.CV.CvEnum.LineType.FourConnected);
          //  source.CopyTo(destiny);
         //   other.ROI = Rectangle.Empty;



           // this.segmentBox.Image = pantone.Bitmap;
            this.segmentBox.Image = other.Bitmap;

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
            this.rgbbox.Image = arr[0].Bitmap;
            //   MessageBox.Show(".0");
            this.rgbbox.Image = arr[1].Bitmap;
            //
            //      MessageBox.Show(".1"); 
            ///    
            this.rgbbox.Image = arr[2].Bitmap;

            // MessageBox.Show(".2");

            final = final.Add(arr[0].Add(arr[1]).Add(arr[2])).Clone();
            this.rgbbox.Image = final.Bitmap;
            
        }




        private void segmentBtn_Click(object sender, EventArgs e)
        {

            imagen.GetChannels();



            //experimento();


            // int channel = Convert.ToInt32(numericUpDown1.Value);
            double thres = 10;
            double max = 300;

            extractArgs();
            for (int channel = 0; channel < 3; channel++)
            {
                imagen.GetAllBoxesRoutine(channel, thres, max);

            }
            for (int channel = 0; channel < 3; channel++)
            {

                imagen.detect.GetUDLRBoxes_PerChannel(0.05, channel, 0.25);

                // segmentBox.Image = imagen.Divs[channel].Bitmap;
            }

            Image<Rgb, byte> blank = imagen.Divs[0].CopyBlank();

            for (int channel = 0; channel < 3; channel++)
            {
                for (int j = 0; j < 4; j++)
                {
                    foreach (var item in imagen.detect.UDLRBoxes[channel, j])
                    {

                        blank.Draw(item, new Rgb(Color.White), 2);



                    }

                    segmentBox.Image = blank.Bitmap;


                    // MessageBox.Show(j.ToString());

                }
                //  imagen.detect.GetUDLRBoxes_PerChannel(0.05, channel, 0.25);


            }




            //    imagen.GetImg(imagen.curentfilename, 10);
            //   imagen.GetImgToCompare(imagen.curentfilename, 10);
            //  segmentBox.Image = imagen.imgUtil.expandedTwo[1].Bitmap;



            //  segmentBox.Image = imagen.expandedEscaledUI[1].Bitmap;
            /*
            LineSegment2D pos = new LineSegment2D(new Point(0, 0), new Point(imagen.escaledUI.Width*3/4, imagen.escaledUI.Height*1/4));

            LineSegment2D d = new LineSegment2D(new Point(imagen.escaledUI.Width, 0), new Point(0, imagen.escaledUI.Height));
            
            LineSegment2D[] araytest = new LineSegment2D[] { pos };

            segmentBox.Image = imagen.detect.DrawLines(new Rgb(Color.Yellow), ref araytest).Bitmap;
    */
            //   MessageBox.Show("finito");
        }

        private void experimento()
        {
            Matrix<float> m = new Matrix<float>(2, 3, 1);
            m.SetIdentity();

            CvInvoke.NamedWindow("lolo", Emgu.CV.CvEnum.NamedWindowType.AutoSize);

            CvInvoke.Imshow("yaya", imagen.RGB[0]);


            Mat n = new Mat();

            PointF[] inputQuad = new PointF[4];
            PointF[] outputQuad = new PointF[4];
            inputQuad[0] = new PointF(-30, -60);
            inputQuad[1] = new PointF(+50, -50);
            inputQuad[2] = new PointF(+100, +50);
            inputQuad[3] = new PointF(-50, +50);
            // The 4 points where the mapping is to be done , from top-left in clockwise order
            outputQuad[0] = new PointF(0, 0);
            outputQuad[1] = new PointF(-1, 0);
            outputQuad[2] = new PointF(-1, -1);
            outputQuad[3] = new PointF(0, -1);



            n = CvInvoke.GetAffineTransform(inputQuad, outputQuad);

            CvInvoke.PerspectiveTransform(imagen.RGB[0], imagen.RGB[1], n);
        }

        private void sobel_Click(object sender, EventArgs e)
        {
            int val = Convert.ToInt32(numericUpDown1.Value);
            int xorder = Convert.ToInt16(xorderbox.Text);
            int yorder = Convert.ToInt16(yorderbox.Text);
            int aperture = Convert.ToInt32(apertureBox.Text);
            imagen.Sobel(val,xorder,yorder,aperture);
            lastBitMap = imagen.Soby[val].Bitmap;
            segmentBox.Image = lastBitMap;
        }

     

        private void Form1_Load(object sender, EventArgs e)
        {
            GetFiles();


            Form m, m2;
            Emgu.CV.UI.HistogramBox hbox;
            Emgu.CV.UI.HistogramBox hbox2;


            m = new Form();
             hbox = new Emgu.CV.UI.HistogramBox();
            m.Controls.Add(hbox);
            hbox.Dock = DockStyle.Fill;
            hbox.ZedGraphControl.Dock = DockStyle.Fill;

            m.Show();
            m.Visible = false;
            m.FormClosing += M_FormClosing;

             m2 = new Form();
            hbox2 = new Emgu.CV.UI.HistogramBox();
            m2.Controls.Add(hbox2);
            hbox2.Dock = DockStyle.Fill;
            hbox2.ZedGraphControl.Dock = DockStyle.Fill;

            m2.Show();
            m2.Visible = false;
            m2.FormClosing += M_FormClosing;
           

            string extToAdd = ".Histo.jpg";
            string savePath = imagen.path + "\\" + imagen.curentfilename.Replace(".jpg", extToAdd);

          

            histoBtnRaw.Click += delegate
            {
                Image<Rgba, byte> aux = imagen.UIOne;
                SetHistogram(ref aux, savePath,ref hbox);
                m.Visible = true;
            };
            extToAdd = ".HistoCorrected.jpg";
            savePath = imagen.path + "\\" + imagen.curentfilename.Replace(".jpg", extToAdd);
          
         
            this.histoBtnCorr.Click += delegate
            {
                Image<Rgba, byte> aux = imagen.imgUtil.rotated;
                SetHistogram(ref aux, savePath, ref hbox2);
                m2.Visible = true;
            };

        }

        private static void SetHistogram(ref Image<Rgba, byte> aux, string savePath, ref Emgu.CV.UI.HistogramBox h)
        {
            h.ClearHistogram();
            h.GenerateHistograms(aux, 256);
            h.ZedGraphControl.AxisChange();
            h.ZedGraphControl.GetImage()
                .Save(savePath);
        }

        private void M_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            (sender as Form).Visible = false;
        }
    }


  

}
