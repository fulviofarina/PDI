﻿using System;
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
        private  PDIGeneticUtil util;
      

        public override void SetPreprocess()
        {

            if (util == null) util = new PDIGeneticUtil();


            if (imagen != null)
            {
                imagen.Dispose();
            }
            imagen = new Img();


            GADataSet.DataRow[] datas = (GADataSet.DataRow[])ProblemData;
            GADataSet.DataRow data = datas.FirstOrDefault(o => o.ProblemID == this.PROBLEMID);
            imagen.path = data.Label1;
            imagen.GetImg(data.Label2, 4);
            imagen.GetImgToCompare(data.Label3, 4);

            Img.DisposeArrayOfImages(ref all);
            all = new List<object>();

           // horizontalCounter = 1;

        }

       

        private Img imagen;


        private List<object> all;
      //  private   int horizontalCounter;
     

        public override void GACompleted<T>(ref GADataSet.SolutionsRow r, ref T s)
        {


            try
            {

              
           
            GADataSet.StringsRow str = s as GADataSet.StringsRow;

            // r.Chromosome //= s.Chromosome = System.IO.File.ReadAllBytes(scriptFile + ".gif"); 

            GADataSet.DataRow aux = r.DataAxuliar.FirstOrDefault();

           
                //  Bitmap b = mat.Bitmap;
                //  escaledUI = original.Resize(w / scale, h / scale, Emgu.CV.CvEnum.Inter.LinearExact, true);

                bool ok = r.IsChromosomeNull();
                ok = ok && aux.ExternalDataObject != null;
                if (ok)
            {

                    object o = aux.ExternalDataObject;
                    string texto = r.Genotype + " F=" + Decimal.Round(Convert.ToDecimal(r.Fitness), 2).ToString();
                    r.Chromosome = Img.ExtractImage(ref o, texto);

                    all.Add(o);

                 //   im.Dispose();
            }
          
                //mat.ToImage<Rgba,byte>().ToJpegData();
            }
            catch (Exception ex)
            {

              
            }


            //   r.Chromosome = imagen.expandedEscaledUIToCompare.Resize(mat.Width / 5, mat.Height / 5, Emgu.CV.CvEnum.Inter.LinearExact).ToJpegData();


        }
        /// <summary>
        /// BASIC CALCULATION NECESSARY FOR FITNESS
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        public override void GenerationRan(ref GADataSet.SolutionsRow r)
        {

            GADataSet.DataRow d = r.DataAxuliar.NewDataRow();
            r.DataAxuliar.AddDataRow(d);
            try
            {

          
            Gene[] genes = r.Genes;
            object[] fitnessMatrix = FitnessRawEvaluator(r.IsChromosomeNull(), ref genes);

            d.ExternalDataObject = fitnessMatrix[1];

            r.Fitness = (double)fitnessMatrix[0];

            r.Genotype = Aid.SetStrings(r.GenesAsInts);

            }
            catch (Exception ex)
            {

              
            }


        }

        public override void GAFinalize()
        {



            string title = "Resultado problema " + GARow.ProblemsRow.Label;


            Img.Concatenate(title,ref all);

            //CvInvoke.WaitKey();
        }

        public override object[] FitnessRawEvaluator(bool isImgNull, ref Gene[] genesArray)
        {
            double? lastBest = GA.BestChromosome?.Fitness;

            int[] genes = genesArray.Select(o=> int.Parse(o.Value.ToString())).ToArray();
            double scalex, scaley;
            int tx, ty, angle;
            double skX, skY;

            extractGeneValues(ref genes, out scalex, out scaley, out tx, out ty, out angle,out skX, out skY);

            double fitness = 0;
            Mat matriz = null;

            object[] aux = null;
            bool ok = checkGenes(ref scalex, ref scaley, ref tx, ref ty, ref angle, ref skX, ref skY);

            if (ok)
            {
                aux = (object[])imagen.PerformRotationCompare(angle, scalex, scaley, tx, ty, skX, skY);
            }

            if (aux != null)
            {
                int[] counts = (aux[1] as int[]);
                double coeff = 1;
          //      counts[3] = (int)1e5;
               // counts[3] = ((Mat)imagen.UIOne.Mat).Height* ((Mat)imagen.UIOne.Mat).Width;
                fitness = fitnessCalculation(ref counts, ref coeff);
                if (aux[0] != null)
                {
                    matriz = aux[0] as Mat;
                    keepMatrix(isImgNull, fitness, lastBest, ref matriz);
                }
            }

            return new object[] { fitness, matriz};
        }

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

        private static double fitnessCalculation(ref int[] counts, ref double coeff)
        {
            double fitness = 0;
            coeff = 1e-5;
            if (counts[0] != 0 && counts[1] != 0 && counts[2] != 0)
            {
                fitness = (double)counts[0];
                fitness += (double)counts[1];
                fitness += (double)counts[2];
                fitness += (double)counts[3];
                fitness *= 0.25*coeff;
                fitness += 1;
                fitness = 1 / fitness;
             // fitness = 1 - fitness;
            }
         
            return fitness;
        }
        private static double fitnessCalculationOLD(ref int[] counts, ref double coeff)
        {
            double fitness = 0;
            if (counts[0] != 0 && counts[1] != 0 && counts[2] != 0)
            {
                fitness = (double)counts[0] / (double)counts[3];
                fitness += (double)counts[1] / (double)counts[3];
                fitness += (double)counts[2] / (double)counts[3];

                fitness *= coeff;
                fitness += 1;
                fitness = 1 / fitness;
                // fitness = 1 - fitness;
            }

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

        /// <summary>
        /// POST CALCULATION TO DECODE
        /// </summary>
        /// <param name="r"></param>

        /// <summary>
        /// INITIALIZER
        /// </summary>
        /// <param name="dt"></param>
        public PDIGeneticController() : base()
        {
        }

        /// <summary>
        /// NATURAL FUNCTION, COMPULSORY
        /// </summary>
        /// <returns></returns>
        public override IChromosome CreateChromosome()
        {
            PDIChromosome c = new PDIChromosome(SIZE, ProblemData.Length);
            c.Config = util;
            c.FillGenes();
            return c;
        }

        
       
    }

   

}