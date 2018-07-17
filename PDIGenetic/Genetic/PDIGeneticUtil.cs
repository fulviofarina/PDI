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

        public void readjustSearchSpace(ref Gene[] genes, double fitness, double comparableFitnes)
        {
            // ..  0.505
            if (fitness >= 0.485 && comparableFitnes < fitness)
            {
                double readjustFactor;

                bool readJustAngleToo = false;

                double a1 = 59.097;
                double a2 = -73.995;
                double a3 = 23.008;

             //   if (genes.Count()>=7)
                {
                    a1 = 62.808;
                    a2 = -76.242;
                }

                readjustFactor = a1 * fitness * fitness + a2 * fitness + a3;

                if (fitness > 0.567)
             //       if (fitness > 0.567)
                {
                    //     readjustFactor = 0.05;
              //      readJustAngleToo = true;
                }
                else if (fitness > 0.563)
               // else if (fitness > 0.563)
                {
                    //  readjustFactor = 0.15;
//readJustAngleToo = true;
                }


                //    this.util.Sum90 = sum90;


                if (readJustAngleToo)
                {
                    int readjustAngle = Convert.ToInt32(readjustFactor * 100);
                    this.InitialAngle = Convert.ToInt32((int)(genes[0].Value)) - readjustAngle;
                    this.FinalAngle = Convert.ToInt32((int)(genes[0].Value)) + readjustAngle;

                }


                double readjustMin = 1 - readjustFactor;
                double readjustMax = 1 + readjustFactor;

                if (genes.Count() > 1)
                {

                    this.MinTX = Convert.ToInt32((int)(genes[1].Value) * readjustMin);
                    this.MinTY = Convert.ToInt32((int)(genes[2].Value) * readjustMin);
                    this.MaxTX = Convert.ToInt32((int)(genes[1].Value) * readjustMax);
                    this.MaxTY = Convert.ToInt32((int)(genes[2].Value) * readjustMax);
                }
                if (genes.Count() > 3)
                {
                    int skx = (int)(genes[3].Value);
                    int sky = (int)(genes[4].Value);

                    int scale = skx;
                    if (sky > skx) scale = sky;
                    this.ScaleMax = Convert.ToDouble(scale * 0.01 * readjustMax);
                    this.ScaleMin = Convert.ToDouble(scale * 0.01 * readjustMin);

                }
                if (genes.Count() > 5)
                {
                    int skx2 = (int)(genes[5].Value);
                    int sky2 = (int)(genes[6].Value);

                    int shear = skx2;
                    if (sky2 > skx2) shear = sky2;

                    this.ShearMax = Convert.ToDouble(shear * 0.01 * readjustMax);
                    this.ShearMin = Convert.ToDouble(shear * 0.01 * readjustMin);

                }
            }


        }

        public void SetStandard()
        {
            MinTX = 0;
            MinTY = 0;
            MaxTX = 300;
            MaxTY = 300;

            ScaleMin =0.1;
            ScaleMax = 3;

           // Sum90 = 0;
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