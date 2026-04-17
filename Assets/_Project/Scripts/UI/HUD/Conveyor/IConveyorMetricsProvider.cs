namespace _Project.Core.Interfaces
{
    public interface IConveyorMetricsProvider
    {
        float GetExactStepDistance(float itemSpacing);
        int GetMaxItemsNeeded();
    }
}