using System;

namespace Calculations
{
    public class DNA<T>
    {
        public float[] Genes { get; set; }
        public float Fitness { get; private set; }

        private Random random;
        private Func<int, float> getRandomGene;
        private Func<float[]> getGene;
        private Func<int, float> fitnessFunction;
        public static int index = 0;
        public DNA()
        {

        }

        public DNA(int size, Random random, Func<int, float> getRandomGene, Func<int, float> fitnessFunction,
             bool shouldInitGenes = true, bool isFirstGenes = false, Func<float[]> getGene = null)
        {
            Genes = new float[size];
            this.random = random;
            this.getRandomGene = getRandomGene;
            this.getGene = getGene;
            this.fitnessFunction = fitnessFunction;

            if (shouldInitGenes)
            {
                for (int i = 0; i < Genes.Length; i++)
                {
                    Genes[i] = CalculateGene();
                }
            }

            if (isFirstGenes)
            {
                Genes = getGene();
            }
        }
        public float CalculateGene()
        {
            float gene = getRandomGene(index);
            index++;
            return gene;
        }
        public float CalculateFitness(int index)
        {
            Fitness = fitnessFunction(index);
            return Fitness;
        }

        public DNA<T> Crossover(DNA<T> otherParent)
        {
            DNA<T> child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);

            if (otherParent != null)
            {
                for (int i = 0; i < Genes.Length; i++)
                {
                    child.Genes[i] = random.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i];
                }
            }

            return child;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                random = new Random();
                if (random.NextDouble() < mutationRate)
                {
                    Genes[i] = CalculateGene();
                }
            }
        }
    }
}
