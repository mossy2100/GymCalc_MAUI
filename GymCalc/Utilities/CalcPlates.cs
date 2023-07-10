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

        // Get the weight of the smallest plate.
        var smallestPlate = plates.Min(p => p.Weight);

        // Calculate the results.
        var results = new Dictionary<double, PlatesResult>();

        // For now we'll hard code that we want 50%, 60% ... 100%.
        // Later, this might be configurable.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            var targetWeightTotal = maxWeight * percent / 100.0;
            var targetWeightEachEnd = (targetWeightTotal - barWeight) / 2;
            var platesResult = new PlatesResult { TargetWeight = targetWeightEachEnd };

            // Get as close as we can without going over.
            var closestBelow = GetPlatesClosestBelow(targetWeightEachEnd, plates);
            var closestBelowTotal = closestBelow.Sum(p => p.Weight);

            // See if we're done.
            if (closestBelowTotal == targetWeightEachEnd)
            {
                // Exact.
                platesResult.ActualWeight = closestBelowTotal;
                platesResult.Plates = closestBelow;
            }
            else
            {
                // Get as close as we can, just going over.
                var closestAbove = GetPlatesClosestBelow(closestBelowTotal + smallestPlate, plates);
                var closestAboveTotal = closestAbove.Sum(p => p.Weight);

                // Which is closer?
                var diffBelow = targetWeightEachEnd - closestBelowTotal;
                var diffAbove = closestAboveTotal - targetWeightEachEnd;
                if (diffBelow < diffAbove)
                {
                    // If diffBelow is less than diffAbove, we'll take the lower weight.
                    platesResult.ActualWeight = closestBelowTotal;
                    platesResult.Plates = closestBelow;
                }
                else
                {
                    // If diffBelow is greater than or equal to diffAbove, we'll take the higher
                    // weight.
                    platesResult.ActualWeight = closestAboveTotal;
                    platesResult.Plates = closestAbove;
                }
            }

            results[percent] = platesResult;
        }

        return results;
    }

    private static List<Plate> GetPlatesClosestBelow(double targetWeight, List<Plate> plates)
    {
        var closestBelow = new List<Plate>();
        var rem = targetWeight;
        var smallestPlate = plates.Min(p => p.Weight);
        while (rem >= smallestPlate)
        {
            // Look through the plates for the next one to add to the stack.
            foreach (var plate in plates)
            {
                if (plate.Weight <= rem)
                {
                    closestBelow.Add(plate);
                    rem -= plate.Weight;
                    break;
                }
            }
        }
        return closestBelow;
    }
}
