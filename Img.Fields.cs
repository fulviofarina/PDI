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
        public double[] args;

        public Detector detect;

        public Image<Rgba, Byte> original;
        public Image<Rgba, Byte> escaledUI;
        public Image<Rgba, Byte>[] RGB;
        //  public Image<Rgba, Byte> blue;
        //  public Image<Rgba, Byte> green;
        //  public Image<Rgba, Byte> alpha;

        public Image<Rgba, Byte>[] Thres;
        public Image<Rgba, Byte>[] Soby;


        public Image<Rgba, Byte>[] ElementSubs;
        //public Image<Rgba, Byte> blueThres;
        // public Image<Rgba, Byte> greenThres;

        public Image<Rgb, Byte>[] Divs;
        // public Image<Rgb, Byte> blueDiv;
        // public Image<Rgb, Byte> greenDiv;

        public BasicInfo BInfo;
        public Directory dir;
        public string path;
        public Image<Rgba, byte> rotated;


        public Img()
        {
            Thres = new Image<Rgba, byte>[4];
            Soby = new Image<Rgba, byte>[4];
            ElementSubs = new Image<Rgba, byte>[4];
            Divs = new Image<Rgb, byte>[3];
            RGB = new Image<Rgba, byte>[4];
            detect = new Detector();

        }



    }

   


}
