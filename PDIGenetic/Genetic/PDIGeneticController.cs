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
      
        public override void SetPreprocess()
        {

            util = new PDIGeneticUtil();


            if (imagen != null) //imagen = new Img();
            {
                      imagen.Dispose();
            }
                 imagen = new Img();

            repairCounter = 0;

            GADataSet.DataRow[] datas = (GADataSet.DataRow[])ProblemData;
            GADataSet.DataRow data = datas.FirstOrDefault(o => o.ProblemID == this.PROBLEMID);
            imagen.path = data.Label1;

            int scale = 6;
            imagen.GetImg(data.Label2, scale);
            imagen.GetImgToCompare(data.Label3, scale);


            //     CvInvoke.DetailEnhance(imagen.UIOne.Mat, imagen.UIOne.Mat);
            //    CvInvoke.DetailEnhance(imagen.UITwo.Mat, imagen.UITwo.Mat);
            imagen.UITwo.CountNonzero();
            Image<Rgba, byte> uno = imagen.UIOne;
            Image<Rgba, byte> dos = imagen.UITwo;


            CvInvoke.Imshow("ONE", uno);
            CvInvoke.Imshow("TWO", dos);
 
            double cm =   imagen.OneMoments[0].GetCentralMoment(1,1);
            double sm = imagen.OneMoments[0].GetSpatialMoment(1, 1);



            Img.DisposeArrayOfImages(ref all);
            all = new List<object>();

            // horizontalCounter = 1;

        }

     

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

        //  private double lastBest;
        //   private int StagnationCounter = 0;
        public IChromosome specificAdam;

        public override IChromosome GenerateAdamFromStrings(ref string[] genes)
        {
            specificAdam = CreateChromosome();

            if (genes != null)
            {
                List<Gene> genesInts = new List<Gene>();
                foreach (var item in genes)
                {
                    genesInts.Add(new Gene(Convert.ToInt32(item)));
                }


                specificAdam.ReplaceGenes(0, genesInts.ToArray());
            }
            return specificAdam;
        }
        /// <summary>
        /// BASIC CALCULATION NECESSARY FOR FITNESS
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        public override void GenerationRan(ref GADataSet.SolutionsRow r, ref IChromosome chromosome)
        {

            GADataSet.DataRow d = r.DataAxuliar.NewDataRow();
            r.DataAxuliar.AddDataRow(d);
            try
            {

             

                Gene[] genes = chromosome.GetGenes();
                object[] fitnessMatrixCounts = FitnessRawEvaluator(r.IsChromosomeNull(),ref chromosome);
                //matrix aqui
              
                d.ExternalDataObject = fitnessMatrixCounts[1];
                //fitness aqui
                double fitness = (double)fitnessMatrixCounts[0];
                r.Fitness = fitness;
                if (fitnessMatrixCounts[2] != null)
                {
                    SystemException ex = fitnessMatrixCounts[2] as SystemException;
                    r.Okays = ex.Message + " - InnerEx - " + ex.InnerException.Message;
                }
                else
                {
                    decimal[] counts = fitnessMatrixCounts[3] as decimal[];
                    if (counts != null)
                    {
                        r.Okays = string.Concat(counts.SelectMany(o => Decimal.Round(o, 2).ToString() + ","));
                    }
                }
                r.Genotype = Aid.SetStrings(r.GenesAsInts, " | ", 0, "0");
                double comparableFitness = this.GARow.Fitness;
               this.util.readjustSearchSpace(ref genes, fitness, comparableFitness);


               if (GA.GenerationsNumber == 20)
               {
                  // GA.Population.CurrentGeneration.Chromosomes.RemoveAt(GA.Population.CurrentGeneration.Chromosomes.Count() - 1);
                    //
                   GA.Population.CurrentGeneration.Chromosomes.Add(specificAdam);

                   // GA.Population.CreateNewGeneration(GA.Population.CurrentGeneration.Chromosomes);
                    
                    //   GA.Crossover.Cross(GA.Population.CurrentGeneration.Chromosomes);
                  //  GA.BestChromosome.Fitness = specificAdam.Fitness;
                  //  GA.Population.CurrentGeneration.BestChromosome.ReplaceGenes(0,specificAdam.GetGenes());

                    //
                    //   GA.Population.CurrentGeneration.Chromosomes.RemoveAt(GA.Population.CurrentGeneration.Chromosomes.Count() - 1);
                    //Add(specificAdam);
                    //    GA.Population.CurrentGeneration.Chromosomes.Add(specificAdam);
                    //  chromosome.ReplaceGenes(0, specificAdam.GetGenes());
                    //     chromosome.Fitness = specificAdam.Fitness;
                    //   chromosome = specificAdam;
                }

            }
            catch (Exception ex)
            {
                r.Okays = ex.Message + " - InnerEx - ";// + ex.InnerException?.Message;

            }


        }

       public override void GAFinalize()
        {
            string title = "Resultado problema " + GARow.ProblemsRow.Label + " - " + GARow.ID;
            Img.Concatenate(title,ref all, imagen.path + "\\");
            //CvInvoke.WaitKey();
        }


        private int repairCounter = 0;

        public override object[] FitnessRawEvaluator(bool isImgNull, ref IChromosome chromosome)
        {

             Gene[] genesArray = chromosome?.GetGenes();
            /*
         
            double? lastnown = GA?.BestChromosome?.Fitness;
            if (lastnown != null && lastnown <= lastBest)
            {
                StagnationCounter++;
               
            }
            else if (lastnown != null)
            {
                lastBest = (double)lastnown;
                StagnationCounter = 0;
            }



            int rotNew = (int)genesArray[0].Value;
            int rotOld = rotNew;
            bool changeGene = false;


            if (StagnationCounter > 200)
            {
                if (rotNew < 0) rotNew += 180;
                else rotNew -= 180;
                changeGene = true;
            }
            else if (StagnationCounter > 100)
            {
                if (rotNew < 0) rotNew += 90;
                else rotNew -= 90;
                changeGene = true;
            }

            if (changeGene)
            {
                genesArray[0] = new Gene(rotNew);
            }

          */
          /*

            int rotNew = (int)genesArray[0].Value;
            int rotOld = rotNew;
           
            if ((double)rotNew == lastBest)
            {
                StagnationCounter++;

            }
            else 
            {
                lastBest = rotNew;
                StagnationCounter = 0;
            }


            if (StagnationCounter >20)
            {
                GA.Mutation.Mutate(chromosome, 0.5f);
           
               
            }
            else if (StagnationCounter > 10)
            {
                GA.Mutation.Mutate(chromosome, 0.5f);
            
            }

          */



        
            double fitness = 0;
            Mat matriz = null;
            object[] result = null;
            if (genesArray == null)
            {
                result = new object[] { fitness,matriz};
                return result;
            }

            result = new object[4];
            int[] genes = genesArray.Select(o=> int.Parse(o.Value.ToString())).ToArray();
            double scalex, scaley;
            int tx, ty, angle;
            double skX, skY;
            double repairFactor = 0.95;

            extractGeneValues(ref genes, out scalex, out scaley, out tx, out ty, out angle,out skX, out skY);

          

            object[] aux = null;
            bool ok = checkGenes(ref scalex, ref scaley, ref tx, ref ty, ref angle, ref skX, ref skY);

            if (ok)
            {
                aux = imagen.PerformRotationCompare(angle, scalex, scaley, ref tx,ref ty, skX, skY, repairFactor);
            }
            int[] counts = null;
            //check if no exception
            if (aux!=null && aux[3] == null)
            {
                if (aux[0] != null) matriz = aux[0] as Mat;
                bool repairTx = (bool)aux[4];
                bool repairTy = (bool)aux[5];

                if (repairTx)
                {
                    chromosome.ReplaceGene(1, new Gene(tx));
                    repairCounter++;
                    if (repairCounter == 5)
                    {
                      //  this.util.MaxTX = Convert.ToInt32(this.util.MaxTX * repairFactor);
                        repairCounter = 0;
                    }
                }

                if (repairTy)
                {
                    chromosome.ReplaceGene(2, new Gene(ty));
                    repairCounter++;
                    if (repairCounter == 5)
                    {
                       // this.util.MaxTY = Convert.ToInt32(this.util.MaxTY * repairFactor);
                        repairCounter = 0;
                    }
                }

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