using GymCalc.Drawables;
using GymCalc.Models;
using GymCalc.Repositories;

namespace GymCalc.Services;

internal static class SingleWeightSolver
{
    private static GymObject[]? _availWeights;

    internal static async Task<List<SingleWeightResult>> CalculateResults<T>(decimal maxWeight,
        GymObjectRepository<T> repo) where T : GymObject, new()
    {
        var results = new List<SingleWeightResult>();

        // Get the available weights as an array.
        IEnumerable<GymObject> listAvailWeights = await repo.LoadSome();
        _availWeights = listAvailWeights.ToArray();

        // For now we'll hard code that we want 50%, 60% ... 100%.
        // Later, this might be configurable.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            decimal idealWeight = maxWeight * percent / 100m;

            // Get the weight that is closest to the ideal weight.
            GymObject closest = FindClosest(idealWeight);

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
        GymObject current = _availWeights![0];

        for (var i = 0; i < _availWeights.Length; i++)
        {
            current = _availWeights[i];

            // Check for exact match.
            if (current.Weight == idealWeight)
            {
                return current;
            }

            // Stop when we find the first weight heavier than the ideal.
            if (current.Weight > idealWeight)
            {
                // If it's the lightest weight, this is the best we can do.
                if (i == 0)
                {
                    return current;
                }

                // Compare the weights above and below.
                GymObject lowerWeightObject = _availWeights[i - 1];
                decimal diffBelow = idealWeight - lowerWeightObject.Weight;
                decimal diffAbove = current.Weight - idealWeight;

                // If the ideal weight is exactly halfway between the lower and higher weights,
                // choose the higher.
                return diffBelow < diffAbove ? lowerWeightObject : current;
            }
        }

        // The loop completed, which means we checked all available weight and none have a weight
        // equal to or greater then the ideal. Therefore, the heaviest weight, which will be the
        // last one we looked at, is the best we can do.
        return current;
    }
}
