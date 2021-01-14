﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculations
{
    public interface IFitnessFunction
    {
        void Start();
        void Update();
        float[] GetGene();
        float GetRandomGene();
        float FitnessFunction(int index);
    }
}