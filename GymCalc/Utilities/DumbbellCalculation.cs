using Galaxon.Core.Numbers;
using GymCalc.Data.Repositories;

namespace GymCalc.Utilities;

internal static class DumbbellCalculation
{
    internal static async Task<Dictionary<double, double>> CalculateResults(double maxWeight)
    {
        // Get the available dumbbell weights ordered from lightest to heaviest.
        var availableDumbbells = await DumbbellRepository.GetAll(true);
        var availableDumbbellWeights = availableDumbbells.Select(p => p.Weight).ToList();

        // Calculate the results.
        var results = new Dictionary<double, double>();

        // For now we'll hard code that we want 50%, 60% ... 100%.
        // Later, this might be configurable.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            var idealWeight = maxWeight * percent / 100.0;

            // Get the dumbbell that is closest to the ideal weight.
            results[percent] = FindBestSolution(idealWeight, availableDumbbellWeights);
        }

        return results;
    }

    /// <summary>
    /// Find the dumbbell closest to the ideal weight.
    /// </summary>
    /// <param name="idealWeight"></param>
    /// <param name="availableDumbbellWeights"></param>
    /// <returns>The closest dumbbell to the ideal weight.</returns>
    private static double FindBestSolution(double idealWeight,
        List<double> availableDumbbellWeights)
    {
        double dumbbellWeight = 0;

        for (var i = 0; i < availableDumbbellWeights.Count; i++)
        {
            dumbbellWeight = availableDumbbellWeights[i];

            // Check for exact match.
            if (dumbbellWeight.FuzzyEquals(idealWeight))
            {
                return dumbbellWeight;
            }

            // The current dumbbell is the smallest one with a weight greater than the ideal.
            if (dumbbellWeight > idealWeight)
            {
                // If it's the smallest dumbbell, this is the best we can do.
                if (i == 0)
                {
                    return dumbbellWeight;
                }

                // Compare the dumbbell weights above and below.
                var belowWeight = availableDumbbellWeights[i - 1];
                var diffBelow = idealWeight - belowWeight;
                var diffAbove = dumbbellWeight - idealWeight;
                return diffAbove < diffBelow ? dumbbellWeight : belowWeight;
            }
        }

        // We checked all available dumbbells and none have a weight equal to or greater then the
        // ideal. Therefore, the heaviest dumbbell is the best we can do.
        return dumbbellWeight;
    }
}
