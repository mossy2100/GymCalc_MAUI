namespace GymCalc.Calculations;

internal class SingleWeightSolver
{
    private readonly List<double> _availWeights;

    public SingleWeightSolver(List<double> availWeights)
    {
        _availWeights = availWeights;
    }

    internal Dictionary<double, double> CalculateResults(double maxWeight)
    {
        var results = new Dictionary<double, double>();

        // For now we'll hard code that we want 50%, 60% ... 100%.
        // Later, this might be configurable.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            var idealWeight = maxWeight * percent / 100.0;

            // Get the weight that is closest to the ideal weight.
            results[percent] = FindClosest(idealWeight);
        }

        return results;
    }

    /// <summary>
    /// Find the weight closest to the ideal weight.
    /// </summary>
    /// <param name="idealWeight"></param>
    /// <returns>The closest weight to the ideal weight.</returns>
    private double FindClosest(double idealWeight)
    {
        double closestWeight = 0;

        for (var i = 0; i < _availWeights.Count; i++)
        {
            closestWeight = _availWeights[i];

            // Check for exact match.
            if (closestWeight == idealWeight)
            {
                return closestWeight;
            }

            // The current weight is the smallest one with a weight greater than the ideal.
            if (closestWeight > idealWeight)
            {
                // If it's the smallest weight, this is the best we can do.
                if (i == 0)
                {
                    return closestWeight;
                }

                // Compare the weights above and below.
                var belowWeight = _availWeights[i - 1];
                var diffBelow = idealWeight - belowWeight;
                var diffAbove = closestWeight - idealWeight;
                return diffAbove < diffBelow ? closestWeight : belowWeight;
            }
        }

        // We checked all available weight and none have a weight equal to or greater then the
        // ideal. Therefore, the heaviest is the closest.
        return closestWeight;
    }
}
