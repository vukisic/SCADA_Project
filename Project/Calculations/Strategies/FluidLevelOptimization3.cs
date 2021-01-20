using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE.Common.Proxies;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common.DataModel;

namespace Calculations
{
    public class FluidLevelOptimization3 : IFitnessFunction
    {
        private Utils utils;
        private List<Tuple<float, float, float>> workingTimes;
        private List<float> results = new List<float>();
        private List<DNA<float>> population = new List<DNA<float>>();
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
        float[] limits3 = new float[] { 30.0f, 60.0f, 90.0f, 120.0f, 150.0f, 180.0f, 210.0f,
                                        240.0f, 270.0f, 300.0f, 330.0f, 360.0f,
                                        390.0f, 420.0f, 450.0f, 480.0f, 510.0f,
                                        540.0f, 570.0f, 600.0f, 630.0f, 660.0f, 690.0f};
        float percentage;
        float optimalFluidLevel;
        float timeFactor;

        int countIteration = 0;
        int iterations;
        float lastBestSolution = 20000.0f;
        int bestSolutionIndex;
        DNA<float> bestIndividual;

        AnalogPoint pump1flow = null;
        AnalogPoint tapChanger1 = null;

        AnalogPoint pump2flow = null;
        AnalogPoint tapChanger2 = null;

        AnalogPoint pump3flow = null;
        AnalogPoint tapChanger3 = null;
        AnalogPoint fluidLevel = null;

        public int isWorking1 = 0;
        public int isWorking2 = 0;
        public int isWorking3 = 0;

        public FluidLevelOptimization3(float optimalFluidLevel, float percentage, float timeFactor, int iterations)
        {
            
            workingTimes = new List<Tuple<float, float, float>>();
            this.percentage = percentage;
            this.optimalFluidLevel = optimalFluidLevel;
            this.timeFactor = timeFactor;
            this.iterations = iterations;
            utils = new Utils(optimalFluidLevel,percentage,timeFactor);
        }

        public float FitnessFunction(int index)
        {
            population = ga.Population;
            float ret = 0.0f;

            DNA<float> individual = population[index];

            ret = individual.Genes[0] * individual.Genes[1] * individual.Genes[2] +
                        +individual.Genes[3] * individual.Genes[4] * individual.Genes[5]
                        + individual.Genes[6] * individual.Genes[7] * individual.Genes[8];

            workingTimes.Add(new Tuple<float, float, float>(individual.Genes[2],
                                                            individual.Genes[5],
                                                            individual.Genes[8]
                                                           ));

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

            if (index == 9)
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

        public DNA<float> Start(float currentFluidLevel)
        {
            model = CeProxyFactory.Instance().ScadaExportProxy().GetData();
            random = new Random();
            
            foreach(var m in model)
            {
                if (m.Value.Mrid == "Flow_AM1")
                    pump1flow = m.Value as AnalogPoint;
                else if (m.Value.Mrid == "Flow_AM2")
                    pump2flow = m.Value as AnalogPoint;
                else if (m.Value.Mrid == "Flow_AM3")
                    pump3flow = m.Value as AnalogPoint;
                else if (m.Value.Mrid == "Discrete_Tap1")
                    tapChanger1 = m.Value as AnalogPoint;
                else if (m.Value.Mrid == "Discrete_Tap2")
                    tapChanger2 = m.Value as AnalogPoint;
                else if (m.Value.Mrid == "Discrete_Tap3")
                    tapChanger3 = m.Value as AnalogPoint;
                else if (m.Value.Mrid == "FluidLevel_Tank")
                    fluidLevel = m.Value as AnalogPoint;
            }

            if (pump1flow.Value > 0)
                isWorking1 = 0;
            else
                isWorking1 = 1;
            if (pump2flow.Value > 0)
                isWorking2 = 0;
            else
                isWorking2 = 1;
            if (pump3flow.Value > 0)
                isWorking3 = 0;
            else
                isWorking3 = 1;

            //PRVA GENERACIJA IMA JEDNU JEDINKU

            firstGenes = new float[]{ isWorking1, pump1flow.Value, 0.1f,
                              isWorking2, pump2flow.Value, 0.1f,
                              isWorking3, pump3flow.Value, 0.1f
            };

            DNA<float> firstHromozome = new DNA<float>(9, random, GetRandomGene, FitnessFunction, false, true, GetGene);

            hromozomes.Add(firstHromozome);

            ga = new GeneticAlgorithm<float>(1, 9, random, GetRandomGene, FitnessFunction, elitism, mutationRate, hromozomes, GetGene);
            
            do
            {
                Update();

                population = ga.Population;

                results = new List<float>();
                for (int i = 0; i < population.Count(); i++)
                {
                    results.Add(currentFluidLevel - FitnessFunction(i));
                }

                List<Tuple<int, float>> potentialSolutions = utils.FindPotentialSolutions(results, workingTimes);

                if (potentialSolutions.Count() > 0)
                {
                    Tuple<int, float> bestSolution = utils.FindBestSolution(potentialSolutions);

                    if (bestSolution.Item2 < lastBestSolution)
                    {
                        lastBestSolution = bestSolution.Item2;
                        bestSolutionIndex = bestSolution.Item1;
                        bestIndividual = population[bestSolution.Item1];
                    }
                }

                countIteration++;

            } while (countIteration != iterations || !utils.IsSolutionCorrect(lastBestSolution, workingTimes[bestSolutionIndex]));

            return bestIndividual;

        }

        public void Update()
        {
            ga.NewGeneration(4);
        }
    }
}
