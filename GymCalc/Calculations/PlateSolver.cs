using Galaxon.Core.Numbers;
using GymCalc.Data.Models;
using GymCalc.Graphics.Drawables;

namespace GymCalc.Calculations;

internal static class PlateSolver
{
    private static List<Plate> _availPlates;

    private static double _maxPlateWeight;

    private static List<Plate> _bestSolution;

    private static double _idealWeight;

    private static double _smallestDiff;

    internal static List<PlatesResult> CalculateResults(double maxWeight,
        double startingWeight, bool oneSideOnly, List<Plate> availPlates, string platesEachSideText)
    {
        var results = new List<PlatesResult>();

        // Sort the plates by decreasing weight.
        _availPlates = availPlates.OrderByDescending(p => p.Weight).ToList();
        _maxPlateWeight = _availPlates[0].Weight;

        // Get the best solution for each percentage fraction of the maxWeight we're interested in.
        // For now we'll hard code 50%, 60% ... 100%, but this might be configurable later.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            var idealTotal = maxWeight * percent / 100.0;
            var idealPlates = (idealTotal - startingWeight) / (oneSideOnly ? 2 : 1);

            // Get the set of plates that is closest to the ideal weight.
            var bestPlates = FindBestPlates(idealPlates);

            // Generate the drawables.
            var drawables = GetDrawables(bestPlates, _maxPlateWeight);

            var result = new PlatesResult(percent, maxWeight, startingWeight, platesEachSideText,
                bestPlates, drawables);
            results.Add(result);
        }

        return results;
    }

    private static List<PlateDrawable> GetDrawables(List<Plate> plates, double maxPlateWeight)
    {
        return plates.Select(plate => new PlateDrawable
            {
                Height = 30,
                MinWidth = 50,
                MaxWeight = maxPlateWeight,
                GymObject = plate
            })
            .ToList();
    }

    /// <summary>
    /// Find the stack of plates that will produce the closest total weight to the ideal weight.
    /// </summary>
    /// <returns></returns>
    private static List<Plate> FindBestPlates(double idealWeight)
    {
        // Initialize fields.
        _idealWeight = idealWeight;
        _smallestDiff = idealWeight;
        _bestSolution = new List<Plate>();

        // Search the solutions space.
        SearchSolutions(_bestSolution, _maxPlateWeight);

        // Sort by ascending plate weight.
        _bestSolution = _bestSolution.OrderBy(p => p.Weight).ToList();

        // Return the best solution found during the search.
        return _bestSolution;
    }

    /// <summary>
    /// Recursive function to generate and test new solutions.
    /// </summary>
    /// <param name="currentStack">The stack of plates so far.</param>
    /// <param name="maxPlateWeight">The largest next plate that can be added.</param>
    private static void SearchSolutions(List<Plate> currentStack, double maxPlateWeight)
    {
        foreach (var plate in _availPlates)
        {
            // Only add plates that are less than or equal to the largest plate added so far.
            // Ensuring plates are added in order of decreasing weight eliminates duplicate
            // solutions.
            if (plate.Weight > maxPlateWeight)
            {
                continue;
            }

            // Create a new stack from the current stack plus the new plate.
            var newStack = new List<Plate>();
            newStack.AddRange(currentStack);
            newStack.Add(plate);

            // Test the solution.
            var sum = newStack.Sum(p => p.Weight);
            var diff = double.Abs(sum - _idealWeight);

            // Check if this is a new best solution.
            if (diff < _smallestDiff
                || diff.FuzzyEquals(_smallestDiff) && newStack.Count < _bestSolution.Count)
            {
                // Update the best solution found so far.
                _bestSolution = newStack;
                _smallestDiff = diff;

                // If it's exact we can stop looking.
                if (diff == 0)
                {
                    break;
                }
            }

            // If we're below the ideal weight, continue adding plates and looking for the best
            // solution.
            if (sum < _idealWeight)
            {
                SearchSolutions(newStack, plate.Weight);
            }

            // If the remaining difference is greater than the weight we just added, don't test
            // adding any smaller plates. It's unnecessary and makes the algorithm take too long.
            if (diff > plate.Weight)
            {
                break;
            }
        }
    }
}
