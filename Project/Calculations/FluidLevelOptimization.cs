using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE.Common.Proxies;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common.DataModel;

namespace Calculations
{
    public class FluidLevelOptimization : IFitnessFunction
    {
        private Dictionary<string, BasePoint> model;
        private Random random;
        private GeneticAlgorithm<float> ga;
        private int elitism = 1;
        private float mutationRate = 0.01f;

        AnalogPoint pump1flow = null;
        AnalogPoint tapChanger1 = null;

        AnalogPoint pump2flow = null;
        AnalogPoint tapChanger2 = null;

        AnalogPoint pump3flow = null;
        AnalogPoint tapChanger3 = null;
        AnalogPoint fluidLevel = null;

        private int isWorking1;
        private int isWorking2;
        private int isWorking3;

        public float FitnessFunction(int index)
        {

            throw new NotImplementedException();
        }

        public float GetRandomGene()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            ScadaExportProxy proxy = new ScadaExportProxy();
            model = proxy.GetData();
            random = new Random();

            ga = new GeneticAlgorithm<float>(1, 9, random, GetRandomGene, FitnessFunction, elitism, mutationRate);
            
            
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

            float[] array = { isWorking1, pump1flow.Value, 0.1f,
                              isWorking2, pump2flow.Value, 0.1f,
                              isWorking3, pump3flow.Value, 0.1f
                            };
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
