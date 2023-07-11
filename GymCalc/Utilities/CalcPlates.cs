using GymCalc.Data;
using GymCalc.Data.Models;
using GymCalc.Data.Repositories;

namespace GymCalc.Utilities;

internal static class CalcPlates
{
    internal static async Task<Dictionary<double, PlatesResult>> CalculateResults(double maxWeight,
        double barWeight)
    {
        // Make sure we have some plates.
        await PlateRepository.InitializeTable();

        // Get the available plates.
        var db = Database.GetConnection();
        var plates = db.Table<Plate>()
            .Where(p => p.Enabled)
            .OrderByDescending(p => p.Weight)
            .ToListAsync()
            .Result;

        // Calculate the results.
        var results = new Dictionary<double, PlatesResult>();

        // For now we'll hard code that we want 50%, 60% ... 100%.
        // Later, this might be configurable.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            var idealTotal = maxWeight * percent / 100.0;
            var idealPlates = (idealTotal - barWeight) / 2;

            // Initialise the result.
            var platesResult = new PlatesResult
            {
                IdealWeight = idealPlates,
                ClosestWeight = 0
            };

            // Get the set of plates that is closest to the ideal weight.
            var bestSolution = FindBestSolution(idealPlates, plates);
            if (bestSolution != null)
            {
                platesResult.ClosestWeight = bestSolution.Sum();

                // Get the plates.
                foreach (var plateWeight in bestSolution)
                {
                    var plate = plates.Where(p => p.Weight == plateWeight).First();
                    platesResult.Plates.Add(plate);
                }
            }

            results[percent] = platesResult;
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

        foreach (var plate in plates)
        {
            var plateWeight = plate.Weight;

            if (plateWeight <= maxPlateWeight && (aboveOnly || plateWeight <= weight))
            {
                var remWeight = weight - plateWeight;

                // If there's no remainder or we've gone over, we're done.
                if (remWeight <= 0)
                {
                    if (aboveOnly && remWeight < 0 || !aboveOnly && remWeight == 0)
                    {
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

    public static List<double> FindBestSolution(double idealWeight, List<Plate> plates)
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
