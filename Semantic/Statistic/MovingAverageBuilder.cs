namespace Semantic.Statistic;

public static class MovingAverageBuilder
{
    public static IMovingAverage Create(int size) => new MovingAverage(size);

    public static IMovingAverage CreateCustomStep(int size, int sampleSize) => new MovingAverageCustomStep(size, sampleSize);
}