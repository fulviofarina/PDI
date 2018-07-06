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
        private  PDIGeneticUtil util;
      

        public override void SetPreprocess()
        {

            util = new PDIGeneticUtil();


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

           double fitness = (double)fitnessMatrix[0];

                if (fitnessMatrix[2] != null)
                {
                    SystemException ex = fitnessMatrix[2] as SystemException;
                    r.Okays = ex.Message + " - InnerEx - " + ex.InnerException.Message;
                }
                else
                {
                    r.Okays = string.Concat((fitnessMatrix[3] as decimal[]).SelectMany(o => o.ToString() + ","));
                }


                r.Genotype = Aid.SetStrings(r.GenesAsInts);
                r.Fitness = fitness;

                double comparableFitness = this.GARow.Fitness;




                if (fitness > 0.505 && comparableFitness < fitness) 
                {
                    double readjustMin = 0.50;
                    double readjustMax = 1.50;
                  
                    if (fitness > 0.535)
                    {
                         readjustMin = 0.75;
                         readjustMax = 1.25;
                    }


                        if (fitness > 0.563)
                    {
                        readjustMin = 0.85;
                        readjustMax = 1.15;
                        this.util.InitialAngle = Convert.ToInt32((int)(genes[0].Value))-10;
                        this.util.FinalAngle = Convert.ToInt32((int)(genes[0].Value))+10;

                    }
                    if (fitness > 0.567)
                    {
                        readjustMin = 0.95;
                        readjustMax = 1.05;
                        this.util.InitialAngle = Convert.ToInt32((int)(genes[0].Value)) - 5;
                        this.util.FinalAngle = Convert.ToInt32((int)(genes[0].Value)) + 5;

                    }

                    this.util.MinTX = Convert.ToInt32((int)(genes[1].Value) *readjustMin);
                    this.util.MinTY = Convert.ToInt32((int)(genes[2].Value) * readjustMin);
                    this.util.MaxTX = Convert.ToInt32((int)(genes[1].Value) * readjustMax);
                    this.util.MaxTY = Convert.ToInt32((int)(genes[2].Value) * readjustMax);

                    this.util.ScaleMax = Convert.ToDouble((int)(genes[3].Value) * 0.01*readjustMax);
                    this.util.ScaleMin= Convert.ToDouble((int)(genes[3].Value) *0.01* readjustMin);

                   
                }


    

            }
            catch (Exception ex)
            {
                r.Okays = ex.Message + " - InnerEx - " + ex.InnerException.Message;

            }


        }

        public override void GAFinalize()
        {



            string title = "Resultado problema " + GARow.ProblemsRow.Label + " - " + GARow.ID;


            Img.Concatenate(title,ref all);

            //CvInvoke.WaitKey();
        }

        public override object[] FitnessRawEvaluator(bool isImgNull, ref Gene[] genesArray)
        {


            object[] result = new object[4];

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
                aux = imagen.PerformRotationCompare(angle, scalex, scaley, tx, ty, skX, skY);
            }
            int[] counts = null;
            //check if no exception
            if (aux!=null && aux[3] == null)
            {
                if (aux[0] != null) matriz = aux[0] as Mat;
            
                counts = (aux[1] as int[]);
               int area = imagen.UITwo.Width * imagen.UITwo.Height;
                //  int area = 250000;
              //   double coeff = 0.25e-5;
              double coeff = 1;
                decimal[] countsAsDeciamals = convertCounts(ref counts, ref area);
                //  fitness = fitnessCalculationOLD(ref counts, ref coeff);
               fitness = fitnessCalculation(ref countsAsDeciamals, ref coeff);
                if (matriz != null)
                {
                    double? lastBest = GA.BestChromosome?.Fitness;
                    keepMatrix(isImgNull, fitness, lastBest, ref matriz);
                }
                result[2] = aux[3]; //exception
                result[3] = countsAsDeciamals;
            }

            result[0] = fitness;
            result[1] = matriz;
          
            //new object[] { fitness, matriz, aux[3], counts }

            return result;
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

        private static double fitnessCalculation(ref decimal[] countDecimals, ref double coeff)
        {
            double fitness = 0;
          
            for (int i = 0; i < countDecimals.Length; i++)
            {
                fitness += (double)countDecimals[i];
            }
                fitness *=coeff;
                fitness +=0;
                fitness = 1 / fitness;
             //  fitness = 1 - fitness;
         
         
            return fitness;
        }
        private static decimal[] convertCounts(ref int[] countsInts,ref int area)
        {
         
            decimal[] newCounts = new decimal[countsInts.Length];
            for (int i = 0; i < countsInts.Length; i++)
            {
                decimal cnt = Convert.ToDecimal((double)countsInts[i] / (double)area);
                cnt = Decimal.Round(cnt, 3);
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