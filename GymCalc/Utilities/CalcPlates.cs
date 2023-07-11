using GymCalc.Data.Models;
using GymCalc.Data.Repositories;

namespace GymCalc.Utilities;

internal static class CalcPlates
{
    internal static async Task<Dictionary<double, List<double>>> CalculateResults(double maxWeight,
        double barWeight)
    {
        // Make sure we have some plates.
        await PlateRepository.InitializeTable();

        // Get the available plates.
        var plates = PlateRepository.GetAllAvailable();

        // Calculate the results.
        var results = new Dictionary<double, List<double>>();

        // For now we'll hard code that we want 50%, 60% ... 100%.
        // Later, this might be configurable.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            var idealTotal = maxWeight * percent / 100.0;
            var idealPlates = (idealTotal - barWeight) / 2;

            // Get the set of plates that is closest to the ideal weight.
            results[percent] = FindBestSolution(idealPlates, plates);
        }

        return results;
    }

    private static List<List<double>> GetSolutions(double weight, List<Plate> plates,
        double maxPlateWeight, bool aboveOnly = false)
    {
        var solutions = new List<List<double>>();
        if (weight <= 0)
        {
            return solutions;
        }

        // Go through all the plate weights looking for possible solutions.
        foreach (var plate in plates)
        {
            var plateWeight = plate.Weight;

            // Check if the current plate is valid for this solution.
            // We don't want to exceed the weight of the previous plate in the stack (represented by
            // maxPlateWeight) otherwise we'll get duplicate solutions.
            if (plateWeight <= maxPlateWeight && (aboveOnly || plateWeight <= weight))
            {
                var remWeight = weight - plateWeight;

                // If there's no remainder or we've gone over, we're done.
                if (remWeight <= 0)
                {
                    // If the remainder is 0 but we're only looking for solutions that exceed the
                    // ideal weight (i.e. aboveOnly is true), then the current plate is not a
                    // solution, but we still need to terminate the iteration.
                    // If we're only looking for solutions that are less than or equal to the ideal
                    // weight, the if the remainder is 0 the current plate is the solution.
                    if (aboveOnly && remWeight < 0 || !aboveOnly && remWeight == 0)
                    {
                        // The current plate is the only one in the solution.
                        var plateResult = new List<double> { plateWeight };
                        solutions.Add(plateResult);
                    }
                    continue;
                }

                // Get all possible solutions for the remaining weight.
                var remSolutions = GetSolutions(remWeight, plates, plateWeight, aboveOnly);

                // If there were no solutions, we're done.
                if (remSolutions.Count == 0)
                {
                    // The current plate is the only one in the solution.
                    var plateResult = new List<double> { plateWeight };
                    solutions.Add(plateResult);
                    continue;
                }

                // Append the current plate to each solution.
                foreach (var remSolution in remSolutions)
                {
                    var plateResult = new List<double> { plateWeight };
                    foreach (var remPlateWeight in remSolution)
                    {
                        plateResult.Add(remPlateWeight);
                    }
                    solutions.Add(plateResult);
                }
            }
        }

        return solutions;
    }

    /// <summary>
    /// Find the set of plates closest to the ideal weight.
    /// </summary>
    /// <param name="idealWeight"></param>
    /// <param name="plates"></param>
    /// <returns>A list of double values representing the plates in the best solution.</returns>
    private static List<double> FindBestSolution(double idealWeight, List<Plate> plates)
    {
        List<double> bestSolution = null;

        // Get all solutions equal to or less than the ideal weight.
        var maxPlateWeight = plates.Select(p => p.Weight).Max();
        var solutionsBelow = GetSolutions(idealWeight, plates, maxPlateWeight);

        if (solutionsBelow.Count > 0)
        {
            var bestSolutionBelow = solutionsBelow
                .OrderBy(s => double.Abs(idealWeight - s.Sum()))
                .ThenBy(s => s.Count)
                .First();
            var bestSolutionBelowSum = bestSolutionBelow.Sum();
            bestSolution = bestSolutionBelow;

            // If the solution is not exact, look for the best solution above and compare.
            if (bestSolutionBelowSum != idealWeight)
            {
                var solutionsAbove = GetSolutions(idealWeight, plates, maxPlateWeight, true);
                if (solutionsAbove.Count > 0)
                {
                    var bestSolutionAbove = solutionsAbove
                        .OrderBy(s => double.Abs(idealWeight - s.Sum()))
                        .ThenBy(s => s.Count)
                        .First();
                    var bestSolutionAboveSum = bestSolutionAbove.Sum();
                    var diffBelow = idealWeight - bestSolutionBelowSum;
                    var diffAbove = bestSolutionAboveSum - idealWeight;
                    if (diffAbove < diffBelow)
                    {
                        bestSolution = bestSolutionAbove;
                    }
                }
            }
        }

        return bestSolution;
    }
}
