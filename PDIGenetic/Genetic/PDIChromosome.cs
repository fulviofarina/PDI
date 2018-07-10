using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PDIGenetic
{
    public sealed class PDIChromosome : ChromosomeBase
    {
    //    private int numberOfGenes;
        private int size;
        private PDIGeneticUtil config;
     


        //   BitArray chromosomeValue;
        public PDIGeneticUtil Config { get => config; set => config = value; }
       /// <summary>
        ///
        /// </summary>
        /// <param name="sizeOfChromosome"></param>
        /// <param name="numOfGenes"></param>
        public PDIChromosome(int sizeOfChromosome, int numOfGenes) : base(sizeOfChromosome)
        {
          //  numberOfGenes = numOfGenes; // do I need the values? nope I think, only indexes
            size = sizeOfChromosome;

           
          
          


        }

        /*
        public static int[] GenesAsIntegers( ref Gene[] genesRaw, int decodedSize)
        {
            int[] integers;

            BitArray array= GenesAsBitArray(genesRaw, out integers, decodedSize);
           
            array.CopyTo(integers, 0);
            return integers;
        }

        private static BitArray GenesAsBitArray(Gene[] genesRaw, out int[] integers, int decodedSize)
        {
            BitArray array;
            integers = new int[decodedSize];
            bool[] genes = genesRaw.Select(o => (bool)o.Value).ToArray();
            array = new BitArray(genes);
            return array;
        }
        */
         private  List<int> integers = new List<int>();

      


        public void FillGenes()
        {
            int rot = 0;
            int tx = 0;
            int ty = 0;
            int sx = 0;
            int sy = 0;
            int skX = 0;
            int skY = 0;
         

            integers.Clear();

             rot = RandomizationProvider.Current.GetInt(config.InitialAngle, config.FinalAngle);
         

            integers.Add(rot);
            if (size > 1)
            {
                tx = RandomizationProvider.Current.GetInt(config.MinTX, config.MaxTX);
                integers.Add(tx);
            }
            if (size > 2)
            {
                ty = RandomizationProvider.Current.GetInt(config.MinTY, config.MaxTY);

                integers.Add(ty);
            }
            int scaleMin = Convert.ToInt32(config.ScaleMin * 100);
            int scaleMax = Convert.ToInt32(config.ScaleMax * 100);

            if (size > 3)
            {
                sx = RandomizationProvider.Current.GetInt(scaleMin, scaleMax);
                integers.Add(sx);
            }
            if (size > 4)
            {
                sy = RandomizationProvider.Current.GetInt(scaleMin, scaleMax);
                integers.Add(sy);
            }

            int shearMin = Convert.ToInt32(config.ShearMin * 100);
            int shearMax = Convert.ToInt32(config.ShearMax * 100);

            if (size > 5)
            {
                skX = RandomizationProvider.Current.GetInt(shearMin, shearMax);
                integers.Add(skX);
            }
            if (size > 6)
            {
                skY= RandomizationProvider.Current.GetInt(shearMin, shearMax);
                integers.Add(skY);
            }

            //   chromosomeValue = new BitArray(integers.ToArray());

            //  base.Length = chromosomeValue.Length;
            //    this.Resize(chromosomeValue.Length);
            for (int i = 0; i < integers.Count; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }
        public override Gene GenerateGene(int geneIndex)
        {


                var valor = integers[geneIndex];


                return new Gene(valor);
          
        }

        public  Gene GenerateGeneOld(int geneIndex)
        {
            int  randIndex = 0;
            double sx = 0;

            if (geneIndex == 0)
            {
                randIndex = RandomizationProvider.Current.GetInt( config.InitialAngle, config.FinalAngle);
             
            }
          
            else if (geneIndex == 1)
            {
                randIndex = RandomizationProvider.Current.GetInt(config.MinTX, config.MaxTX);
              
            }
            else if (geneIndex == 2)
            {
                randIndex = RandomizationProvider.Current.GetInt(config.MinTY, config.MaxTY);
                
            }
            else if (geneIndex == 3)
            {
                sx = RandomizationProvider.Current.GetDouble(config.ScaleMin, config.ScaleMax);
             
            }
            else if (geneIndex == 4)
            {
                 sx = RandomizationProvider.Current.GetDouble(config.ScaleMin, config.ScaleMax);
               
            }


            if (geneIndex <= 2)
            {
                return new Gene(randIndex);
            }
            else return new Gene(sx); 
        }
        /*
        /// <summary>
        /// OPTION B
        /// </summary>
        /// <param name="geneIndex"></param>
        /// <returns></returns>
        public Gene GenerateGeneB(int geneIndex)
        {
            int randIndex = 0;
            int initial = 0;

            randIndex = RandomizationProvider.Current.GetInt(initial, numberOfGenes);

            if (randIndex == 0) randIndex = -1;

            return new Gene(randIndex);
        }

        /// <summary>
        /// OPTION A
        /// </summary>
        /// <param name="geneIndex"></param>
        /// <returns></returns>
        public Gene GenerateGeneA(int geneIndex)
        {
            int randIndex = 0;
            int initial = 0;

            //makes 1 gene with a random index from 0 to m_values as MAX
            initial = 1;

            randIndex = RandomizationProvider.Current.GetInt(initial, numberOfGenes);

            if (randIndex > numberOfGenes) randIndex = -1;

            return new Gene(randIndex);
        }
        */
        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public override IChromosome CreateNew()
        {
            PDIChromosome c = new PDIChromosome(size, 0);
            c.Config = config;
            c.FillGenes();
            return c;
        }

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>The chromosome clone.</returns>
        public override IChromosome Clone()
        {
            IChromosome clone = base.Clone();
            (clone as PDIChromosome).Config = config;
            return clone;
        }
    }
}