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
  

        public partial class BasicInfo
    {

        public List<float[]> histogramsRgba;
        public List<Gray> averagesRgba;
        public List<MCvScalar> sdsRgba;
        public ImgDB.BasicInfoDataTable table;

        public BasicInfo(ref Image<Rgba, byte> input)
        {
            table = new ImgDB.BasicInfoDataTable();

            int nBins = 256;
            RangeF range1 = new RangeF(0, 255);

            DenseHistogram hist = new DenseHistogram(nBins, range1);
            Image<Gray, byte>[] isg = input.Split();

            histogramsRgba = new List<float[]>();
            averagesRgba = new List<Gray>();
            sdsRgba = new List<MCvScalar>();

           
            for (short i = 0; i < isg.Count(); i++)
            {
                hist.Calculate(new Image<Gray, byte>[] { isg[i] }, false, null);
                float[] values = hist.GetBinValues();
                histogramsRgba.Add(values);

                Gray gr = new Gray();
                MCvScalar sd = new MCvScalar();
                isg[i].AvgSdv(out gr, out sd);

                averagesRgba.Add(gr);
                sdsRgba.Add(sd);

               
                ImgDB.BasicInfoRow row =  table.NewBasicInfoRow();
                row.Avg = gr.Intensity;
                row.Histogram = values;
                row.SD = sd.V0;
                row.Channel = i;
                if (i == 0) row.ChannelName = "Red";
                else if (i == 1) row.ChannelName = "Green";
                else if (i == 2) row.ChannelName = "Blue";
                else if (i == 3) row.ChannelName = "Alpha";

                table.AddBasicInfoRow(row);

            }

        }
    }

    

  

   
}
