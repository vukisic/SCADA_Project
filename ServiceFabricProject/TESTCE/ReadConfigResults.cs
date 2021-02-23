namespace CE
{
    public class ReadConfigResults
    {
        public float Percetage { get; set; }
        public float OptimalFluidLevel { get; set; }
        public float TimeFactor { get; set; }
        public int Iterations { get; set; }

        public ReadConfigResults(float percetage, float optimalFluidLevel, float timeFactor, int iterations)
        {
            Percetage = percetage;
            OptimalFluidLevel = optimalFluidLevel;
            TimeFactor = timeFactor;
            Iterations = iterations;
        }
    }
}
