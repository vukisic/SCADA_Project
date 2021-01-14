using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;

namespace Calculations
{
    public class GeneticAlgorithm<T>
    {

        public List<DNA<T>> Population { get; private set; }
        public int Generation { get; private set; }
        public float BestFitness { get; private set; }
        public T[] BestGenes { get; private set; }

        public int Elitism;
        public float MutationRate;

        private List<DNA<T>> newPopulation;
        private Random random;
        private float fitnessSum;
        private int dnaSize;
        private Func<T> getRandomGene;
        private Func<T[]> getGene;
        private Func<int, float> fitnessFunction;


        public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction,
            int elitism, List<DNA<T>> hromozomes, Func<T[]> getGene, float mutationRate = 0.01f)
        {
            Generation = 1;
            Elitism = elitism;
            MutationRate = mutationRate;
            Population = new List<DNA<T>>(populationSize);
            newPopulation = new List<DNA<T>>(populationSize);
            this.random = random;
            this.dnaSize = dnaSize;
            this.getRandomGene = getRandomGene;
            this.getGene = getGene;
            this.fitnessFunction = fitnessFunction;

            BestGenes = new T[dnaSize];

            if (hromozomes.Count > 0)
            {
                foreach (var h in hromozomes)
                    Population.Add(h);
            }
            else
            {
                for (int i = 0; i < populationSize; i++)
                {
                    Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, false, getGene, shouldInitGenes: true));
                }
            }
        }
        //1. a1b1c1X1 + a2b2c2X2 + a3b3c3X3 <= 1000
        //2. a - 0 ili 1 - da li pumpa radi
        //3. b - pozicija tap changera - 1-5
        //4. c - vrijeme rada
        //5. POCETNA --- 1*2*2*X1 + 0 + 0 <= 1000
        //6. NOVA GENERACIJA...
        /*
                1*2*2*X1 + 0*0*0*X2 + 0*0*0*X3 <= 1000
                0*2*2*X1 + 1*2*0.1*X2 + 0*0*0*X3 <= 1000
                1*1*2*X1 + 1*1*0.1*X2 + 0*0*0*X3 <= 1000

                (1, 2, 2, 0, 0, 0, 0, 0, 0)
                (0, 2, 2, 1, 2, 0.1, 0, 0, 0)
                (1, 1, 2, 1, 1, 0.1, 0, 0, 0)
        */
        //7. Bira se najbolja jedinka od ove nove generacije
        public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
        {
 
            int finalCount = Population.Count + numNewDNA;

            if (finalCount <= 0)
            {
                return;
            }

            if (Population.Count > 0)
            {
                CalculateFitness();
                Population.Sort(CompareDNA);
            }
            newPopulation.Clear();

            for (int i = 0; i < Population.Count; i++)
            {
                if (i < Elitism && i < Population.Count)
                {
                    newPopulation.Add(Population[i]);
                }
                else if ((i < Population.Count || crossoverNewDNA) && Population.Count > 1)
                {
                    DNA<T> parent1 = ChooseParent();
                    DNA<T> parent2 = ChooseParent();

                    DNA<T> child = parent1.Crossover(parent2);

                    child.Mutate(MutationRate);

                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, false, getGene, shouldInitGenes: true));
                }
            }

            List<DNA<T>> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;

            Generation++;
        }

        private int CompareDNA(DNA<T> a, DNA<T> b)
        {
            if (a.Fitness > b.Fitness)
            {
                return -1;
            }
            else if (a.Fitness < b.Fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private void CalculateFitness()
        {
            fitnessSum = 0;
            DNA<T> best = Population[0];

            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].CalculateFitness(i);

                if (Population[i].Fitness > best.Fitness)
                {
                    best = Population[i];
                }
            }

            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }

        private DNA<T> ChooseParent()
        {
            double randomNumber = random.NextDouble() * fitnessSum;

            for (int i = 0; i < Population.Count; i++)
            {
                if (randomNumber < Population[i].Fitness)
                {
                    return Population[i];
                }

                randomNumber -= Population[i].Fitness;
            }

            return null;
        }
    }
}
