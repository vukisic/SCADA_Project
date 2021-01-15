using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE.Common.Proxies;
using SCADA.Common.DataModel;

namespace Calculations
{
    public class FluidLevelOptimization1 : IFitnessFunction
    {
        private List<Tuple<float>> workingTimes;
        private float[] results = new float[] { };
        private List<DNA<float>> population;
        private Dictionary<string, BasePoint> model;
        private Random random;
        private GeneticAlgorithm<float> ga;
        private int elitism = 1;
        private float mutationRate = 0.01f;
        private List<DNA<float>> hromozomes = new List<DNA<float>>();
        float[] firstGenes;
        int index = 0;
        float[] limits1 = new float[] { 0.0f, 1.0f };
        float[] limits2 = new float[] { 100.0f, 200.0f, 300.0f, 400.0f, 500.0f };
        float[] limits3 = new float[] { 1800.0f, 3600.0f, 5400.0f, 7200.0f, 9000.0f, 10800.0f, 12600.0f,
                                        14400.0f, 18000.0f, 19800.0f, 21600.0f, 23400.0f,
                                        25200.0f, 27000.0f, 28800.0f, 30600.0f, 32400.0f,
                                        34200.0f, 36000.0f, 37800.0f, 39600.0f, 41400.0f, 43200.0f};
        float percentage;
        float optimalFluidLevel;
        float timeFactor;

        AnalogPoint pump1flow = null;
        AnalogPoint tapChanger1 = null;

        AnalogPoint fluidLevel = null;

        public int isWorking1 = 0;

        public FluidLevelOptimization1()
        {
            Start();
            population = ga.Population;
            workingTimes = new List<Tuple<float>>();

            if (!float.TryParse(ConfigurationManager.AppSettings["Percetage"], out percentage))
            {
                percentage = 5;
            }
            if (!float.TryParse(ConfigurationManager.AppSettings["OptimalFluidLevel"], out optimalFluidLevel))
            {
                optimalFluidLevel = 1000;
            }
            if (!float.TryParse(ConfigurationManager.AppSettings["TimeFactor"], out timeFactor))
            {
                timeFactor = 1800;
            }


        }

        public float FitnessFunction(int index)
        {
            float ret = 0.0f;

            DNA<float> individual = population[index];

            for (int i = 0; i < individual.Genes.Count(); i++)
            {
                ret = individual.Genes[0] * individual.Genes[1] * individual.Genes[2];

                workingTimes.Add(new Tuple<float>(individual.Genes[2]));
            }

            return ret;
        }

        public float[] GetGene()
        {
            return firstGenes;
        }

        public float GetRandomGene()
        {
            float gene = 0.1f;
            random = new Random();

            if (index == 3)
                index = 0;

            if (index % 3 == 0)
                gene = limits1[random.Next(limits1.Length)];
            else if (index % 3 == 1)
                gene = limits2[random.Next(limits2.Length)];
            else if (index % 3 == 2)
                gene = limits3[random.Next(limits3.Length)];

            index++;
            return gene;
        }

        public void Start()
        {
            model = CeProxyFactory.Instance().ScadaExportProxy().GetData();
            random = new Random();

            foreach (var m in model)
            {
                if (m.Value.Mrid == "Flow_AM1")
                    pump1flow = m.Value as AnalogPoint;
                else if (m.Value.Mrid == "Discrete_Tap1")
                    tapChanger1 = m.Value as AnalogPoint;
                else if (m.Value.Mrid == "FluidLevel_Tank")
                    fluidLevel = m.Value as AnalogPoint;
            }

            if (pump1flow.Value > 0)
                isWorking1 = 0;
            else
                isWorking1 = 1;

            firstGenes = new float[]{ isWorking1, pump1flow.Value, 0.1f };

            DNA<float> firstHromozome = new DNA<float>(3, random, GetRandomGene, FitnessFunction, true, GetGene, false);

            hromozomes.Add(firstHromozome);
            ga = new GeneticAlgorithm<float>(1, 3, random, GetRandomGene, FitnessFunction, elitism, hromozomes, GetGene, mutationRate);

            Update();

            for (int i = 0; i < population.Count(); i++)
            {
                results[i] = FitnessFunction(i);
            }

            List<float> potentialSolutions = FindPotentialSolutions(results, workingTimes);
            float bestSolution = FindBestSolution(potentialSolutions);
        }

        private float FindBestSolution(List<float> potentialSolutions)
        {
            return potentialSolutions.Min();
        }

        private List<float> FindPotentialSolutions(float[] results, List<Tuple<float>> times)
        {
            var solutions = new List<float>();
            for (int i = 0; i < results.Count(); i++)
            {
                if (IsSolutionCorrect(results[i], times[i]))
                    solutions.Add(results[i]);
            }
            foreach (var item in results)
            {

            }

            return solutions;
        }

        private bool IsSolutionCorrect(float solution, Tuple<float> times)
        {
            float lowerBound = optimalFluidLevel * (1.0f - (percentage / 100));
            float upperBound = optimalFluidLevel * (1.0f + (percentage / 100));
            bool criterium1 = (solution <= upperBound && solution >= lowerBound);
            bool criterium2 = times.Item1 <= timeFactor;
            return criterium1 && criterium2;
        }

        public void Update()
        {
            ga.NewGeneration(4);
        }
    }
}
