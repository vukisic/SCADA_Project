namespace Calculations
{
    public interface IFitnessFunction
    {
        DNA<float> Start(float currentFluidLevel);
        void Update();
        float[] GetGene();
        float GetRandomGene(int index);
        float FitnessFunction(int index);
    }
}
