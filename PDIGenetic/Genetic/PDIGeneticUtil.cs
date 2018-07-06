using System;
using System.Data;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using GADB;
using GeneticSharp.Domain.Chromosomes;
using PDILib;

namespace PDIGenetic
{
   

    public partial class PDIGeneticUtil
    {

        public PDIGeneticUtil()
        {
            SetStandard();
          //  SetAlternative();
        }

      
        public void SetStandard()
        {
            MinTX = 0;
            MinTY = 0;
            MaxTX = 300;
            MaxTY = 300;

            ScaleMin =0.1;
            ScaleMax = 3;


            //   ScaleMin = 0.3;
            //  ScaleMax = 3;
           // InitialAngle = 0;
          //  FinalAngle = 360;

            InitialAngle = -180;
            FinalAngle =180;
            ShearMax = 1;
            ShearMin = 0;
        }

        private int initial;// = 0;
        private int final;// = 360;
        private double scaleMax;// = 3;
        private double scaleMin;// = 3;
        private int minX;// = 0;
        private int minY;// = 0;

        private int maxX;//= 30;
        private int maxY;// = 30;
        private double shearMin;
        private double shearMax;

        // public double Initial { get => initial; set => initial = value; }
        public int InitialAngle { get => initial; set => initial = value; }
        public int FinalAngle { get => final; set => final = value; }
        public double ScaleMax { get => scaleMax; set => scaleMax = value; }
        public double ScaleMin { get => scaleMin; set => scaleMin = value; }
        public int MaxTX { get => maxX; set => maxX = value; }
        public int MaxTY { get => maxY; set => maxY = value; }
        public int MinTX { get => minX; set => minX = value; }
        public int MinTY { get => minY; set => minY = value; }
        public double ShearMin { get => shearMin; set => shearMin = value; }
        public double ShearMax { get => shearMax; set => shearMax = value; }
    }

}