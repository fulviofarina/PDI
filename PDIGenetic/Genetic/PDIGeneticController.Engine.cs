using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.Structure;
using GADB;
using GeneticSharp.Domain.Chromosomes;
using PDILib;

namespace PDIGenetic
{
    public partial class PDIGeneticController : ControllerBase, IController
    {
        private PDIGeneticUtil util;
        private Img imagen;
        private List<object> all;
   

    private bool checkGenes(ref double scalex, ref double scaley, ref int tx, ref int ty, ref int angle, ref double skX, ref double skY)
        {
            bool ok = angle <= this.util.FinalAngle;
            ok = ok && angle >= this.util.InitialAngle;
            ok = ok && tx >= this.util.MinTX;
            ok = ok && tx <= this.util.MaxTX;
            ok = ok && ty >= this.util.MinTY;
            ok = ok && ty <= this.util.MaxTY;
            ok = ok && scalex >= this.util.ScaleMin;
            ok = ok && scalex <= this.util.ScaleMax;
            ok = ok && scaley >= this.util.ScaleMin;
            ok = ok && scaley <= this.util.ScaleMax;
            ok = ok && skX >= this.util.ShearMin;
            ok = ok && skX <= this.util.ShearMax;
            ok = ok && skY >= this.util.ShearMin;
            ok = ok && skY <= this.util.ShearMax;
            return ok;
        }

        private static double fitnessCalculation(ref decimal[] countDecimals, ref double coeff)
        {
            double fitness = 0;
          
            for (int i = 0; i < countDecimals.Length; i++)
            {
                double weight = 1;
            //    if (i == 3) weight =1;
                fitness += (double)(countDecimals[i])*weight;
            }
                fitness *=coeff;
                fitness +=0;
                fitness = 1 / fitness;
          //    fitness = 1 - fitness;
         
         
            return fitness;
        }
        private static double fitnessCalculationOK(ref decimal[] countDecimals, ref double coeff)
        {
            double fitness = 0;

            for (int i = 0; i < countDecimals.Length; i++)
            {
                fitness += (double)countDecimals[i];
            }
            fitness *= coeff;
            fitness += 0;
            fitness = 1 / fitness;
            //    fitness = 1 - fitness;


            return fitness;
        }
        private static decimal[] convertCounts(ref int[] countsInts,ref int area, int roundCyph=6)
        {
         
            decimal[] newCounts = new decimal[countsInts.Length];
            for (int i = 0; i < countsInts.Length; i++)
            {
                decimal cnt = Convert.ToDecimal((double)countsInts[i] / (double)area);
                cnt = Decimal.Round(cnt, roundCyph);
                newCounts[i] = cnt;
            }

            return newCounts;
        }
        private static double fitnessCalculationOLD(ref int[] counts, ref double coeff)
        {
            double fitness = 0;
         
                fitness = (double)counts[0];
                fitness += (double)counts[1] ;
                fitness += (double)counts[2];
                fitness += (double)counts[3];

                fitness *= coeff;
                fitness += 1;
                fitness = 1 / fitness;
                // fitness = 1 - fitness;
          

            return fitness;
        }
    

        private static void extractGeneValues(ref int[] genes, out double scalex, out double scaley, out int tx, out int ty, out int angle, out double skX, out double skY)
        {
            scalex = 1;
            scaley = 1;
            tx = 0;
            ty = 0;
            skX = 0;
            skY = 0;

            if (genes.Count() > 1)
            {
                tx = (genes[1]);
               
            }
            if (genes.Count() > 2)
            {
                ty = (genes[2]);
            }
                if (genes.Count() > 3)
            {
                scalex = (double)(genes[3])/100;
              
            }
            if (genes.Count() > 4)
            {
               
                scaley = (double)(genes[4]) / 100;
            }
            if (genes.Count() > 5)
            {

                skX = (double)(genes[5]) / 100;
            }
            if (genes.Count() > 6)
            {

                skY = (double)(genes[6]) / 100;
            }
            angle = genes[0];
        }

        /// <summary>
        ///  Only keep the best fitness image
        /// </summary>
        /// <param name="isNullChromosome"></param>
        /// <param name="fitness"></param>
        /// <param name="latestBest"></param>
        /// <param name="matriz"></param>
        private static void keepMatrix(bool isNullChromosome,  double fitness, double? latestBest, ref Mat matriz)
        {
            bool killImage = false;

            if (latestBest != null)
            {

                if (latestBest >= fitness)
                {
                    if (latestBest == fitness)
                    {
                        if (!isNullChromosome) killImage = true;
                    }
                    else killImage = true;
                }
            }
           
            if (killImage)
            {
                matriz?.Dispose();
                matriz = null;
            }
           
        }

        
       
    }

   

}