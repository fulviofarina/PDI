using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace PDILib
{
    public partial class ImgUtil
    {
        public ImgUtil()
        {
            expandedOne = new Image<Rgba, byte>[2];
            expandedTwo = new Image<Rgba, byte>[2];
           // pitchWhite = new Rgba(255, 255, 255, 0);
            pitchBlack = new Rgba(0, 0, 0, 0);
            pitchWhite = new Rgba(255, 255, 255, 0);

        }
        public int TX;
        public int TY;

       // public double radiusTestCanvas;
        public Image<Rgba, byte> rotated;
      //  public Image<Rgba, byte> resultado;
        public Image<Rgba, Byte> lastCanvas;


        public Image<Rgba, byte>[] expandedOne;
        public Image<Rgba, byte>[] expandedTwo;
        public Rgba pitchBlack;// = new Rgba(255, 255, 255, 255);
        public Rgba pitchWhite;
    }
}
