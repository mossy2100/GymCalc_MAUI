using GymCalc.Drawables;
using GymCalc.Models;

namespace GymCalc.Solvers;

internal static class SingleWeightSolver
{
    private static IEnumerable<GymObject> _availWeights;

    internal static List<SingleWeightResult> CalculateResults(decimal maxWeight,
        IEnumerable<GymObject> availWeights)
    {
        var results = new List<SingleWeightResult>();

        _availWeights = availWeights;

        // For now we'll hard code that we want 50%, 60% ... 100%.
        // Later, this might be configurable.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            var idealWeight = maxWeight * percent / 100m;

            // Get the weight that is closest to the ideal weight.
            var closest = FindClosest(idealWeight);

            // Construct the drawable.
            var drawable = GymObjectDrawable.Create(closest);

            // Add the result to the result set.
            var result = new SingleWeightResult(percent, idealWeight, closest, drawable);
            results.Add(result);
        }

        return results;
    }

    /// <summary>
    /// Find the weight closest to the ideal weight.
    /// </summary>
    /// <param name="idealWeight"></param>
    /// <returns>The closest weight to the ideal weight.</returns>
    private static GymObject FindClosest(decimal idealWeight)
    {
        var gymObjects = _availWeights.ToArray();
        GymObject current = null;
        for (var i = 0; i < gymObjects.Length; i++)
        {
            current = gymObjects[i];

            // Check for exact match.
            if (current.Weight == idealWeight)
            {
                return current;
            }

            // The current weight is the smallest one with a weight greater than the ideal.
            if (current.Weight > idealWeight)
            {
                // If it's the smallest weight, this is the best we can do.
                if (i == 0)
                {
                    return current;
                }

                // Compare the weights above and below.
                var previous = gymObjects[i - 1];
                var diffBelow = idealWeight - previous.Weight;
                var diffAbove = current.Weight - idealWeight;
                return diffAbove < diffBelow ? current : previous;
            }
        }

        // We checked all available weight and none have a weight equal to or greater then the
        // ideal. Therefore, the heaviest is the closest.
        return current;
    }
}
