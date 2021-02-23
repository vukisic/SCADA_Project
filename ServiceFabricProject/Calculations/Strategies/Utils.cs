using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculations
{
    public class Utils
    {
        private float optimalFluidLevel;
        private float percentage;
        private float timeFactor;

        public Utils(float optimalFluidLevel, float percentage, float timeFactor)
        {
            this.optimalFluidLevel = optimalFluidLevel;
            this.percentage = percentage;
            this.timeFactor = timeFactor;
        }

        public Tuple<int, float> FindBestSolution(List<Tuple<int, float>> potentialSolutions)
        {
            Tuple<int, float> bestSolution;

            int indexSolution = potentialSolutions[0].Item1;
            float minSolution = potentialSolutions[0].Item2;

            foreach (var solution in potentialSolutions)
            {
                if (solution.Item2 < minSolution)
                {
                    indexSolution = solution.Item1;
                    minSolution = solution.Item2;
                }
            }

            bestSolution = new Tuple<int, float>(indexSolution, minSolution);

            return bestSolution;
        }

        public List<Tuple<int, float>> FindPotentialSolutions(List<float> results, List<Tuple<float, float, float>> times)
        {
            var solutions = new List<Tuple<int, float>>();
            for (int i = 0; i < results.Count(); i++)
            {
                if (IsSolutionCorrect(results[i], times[i]))
                    solutions.Add(new Tuple<int, float>(i, results[i]));
            }

            return solutions;
        }

        public List<Tuple<int, float>> FindPotentialSolutions(List<float> results, List<Tuple<float, float>> times)
        {
            var solutions = new List<Tuple<int, float>>();
            for (int i = 0; i < results.Count(); i++)
            {
                if (IsSolutionCorrect(results[i], times[i]))
                    solutions.Add(new Tuple<int, float>(i, results[i]));
            }
            return solutions;
        }

        public List<Tuple<int, float>> FindPotentialSolutions(List<float> results, List<Tuple<float>> times)
        {
            var solutions = new List<Tuple<int, float>>();
            for (int i = 0; i < results.Count(); i++)
            {
                if (IsSolutionCorrect(results[i], times[i]))
                    solutions.Add(new Tuple<int, float>(i, results[i]));
            }
            return solutions;
        }

        public bool IsSolutionCorrect(float solution, Tuple<float, float, float> times)
        {
            float lowerBound = optimalFluidLevel * (1.0f - (percentage / 100));
            float upperBound = optimalFluidLevel * (1.0f + (percentage / 100));
            bool criterium1 = (solution <= upperBound && solution >= lowerBound);
            bool criterium2 = (Math.Abs(times.Item1 - times.Item2) <= timeFactor && Math.Abs(times.Item1 - times.Item3) <= timeFactor && Math.Abs(times.Item2 - times.Item3) <= timeFactor);
            return criterium1 && criterium2;
        }

        public bool IsSolutionCorrect(float solution, Tuple<float, float> times)
        {
            float lowerBound = optimalFluidLevel * (1.0f - (percentage / 100));
            float upperBound = optimalFluidLevel * (1.0f + (percentage / 100));
            bool criterium1 = (solution <= upperBound && solution >= lowerBound);
            bool criterium2 = (Math.Abs(times.Item1 - times.Item2) <= timeFactor);
            return criterium1 && criterium2;
        }

        public bool IsSolutionCorrect(float solution, Tuple<float> times)
        {
            float lowerBound = optimalFluidLevel * (1.0f - (percentage / 100));
            float upperBound = optimalFluidLevel * (1.0f + (percentage / 100));
            bool criterium1 = (solution <= upperBound && solution >= lowerBound);
            return criterium1;
        }
    }
}
