using GymCalc.Drawables;
using GymCalc.Models;
using GymCalc.Repositories;

namespace GymCalc.Services;

internal static class PlateSolver
{
    private static List<Plate>? _availPlates;

    private static decimal _maxPlateWeight;

    private static List<Plate>? _bestPlates;

    private static decimal _idealWeight;

    private static decimal _smallestDiff;

    internal static async Task<List<PlatesResult>> CalculateResults(decimal maxTotalWeight,
        decimal totalStartingWeight, int nStacks, string eachSideText, PlateRepository plateRepo)
    {
        var results = new List<PlatesResult>();

        // Get the plates in order of decreasing weight.
        _availPlates = await plateRepo.LoadSome(true, false);

        // Get the heaviest plate.
        _maxPlateWeight = _availPlates.First().Weight;

        // Get the best solution for each percentage fraction of the maxWeight we're interested in.
        // For now we'll hard code 50%, 60% ... 100%, but this might be configurable later.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            decimal idealTotal = maxTotalWeight * percent / 100m;
            decimal idealPlates = (idealTotal - totalStartingWeight) / nStacks;

            // Get the set of plates that is closest to the ideal weight.
            List<Plate> closestPlates = FindBestPlates(idealPlates);

            // Create the drawable.
            var drawable = new PlateStackDrawable
            {
                Plates = closestPlates,
                MaxWeight = _maxPlateWeight
            };

            // Create the result object.
            var result = new PlatesResult(percent, maxTotalWeight, totalStartingWeight, nStacks,
                eachSideText, closestPlates, drawable);
            results.Add(result);
        }

        return results;
    }

    /// <summary>
    /// Find the stack of plates that will produce the closest total weight to the ideal weight.
    /// </summary>
    /// <returns></returns>
    private static List<Plate> FindBestPlates(decimal idealWeight)
    {
        // Initialize fields.
        _idealWeight = idealWeight;
        _smallestDiff = idealWeight;
        _bestPlates = new List<Plate>();

        // Search the solutions space.
        SearchSolutions(_bestPlates, _maxPlateWeight);

        // Sort by ascending plate weight.
        _bestPlates = _bestPlates.OrderBy(p => p.Weight).ToList();

        // Return the best solution found during the search.
        return _bestPlates;
    }

    /// <summary>
    /// Recursive function to generate and test new solutions.
    /// </summary>
    /// <param name="currentPlates">The stack of plates so far.</param>
    /// <param name="maxPlateWeight">The largest next plate that can be added.</param>
    private static void SearchSolutions(IReadOnlyCollection<Plate> currentPlates,
        decimal maxPlateWeight)
    {
        foreach (Plate plate in _availPlates!)
        {
            // Only add plates that are less than or equal to the largest plate added so far.
            // Ensuring plates are added in order of decreasing weight eliminates duplicate
            // solutions.
            if (plate.Weight > maxPlateWeight)
            {
                continue;
            }

            // Create a new stack from the current stack plus the new plate.
            var newPlates = new List<Plate>();
            newPlates.AddRange(currentPlates);
            newPlates.Add(plate);

            // Test the solution.
            decimal newPlatesWeight = newPlates.Sum(p => p.Weight);
            var newDiff = decimal.Abs(newPlatesWeight - _idealWeight);

            // Check if this is a new best solution.
            var updateSolution = false;
            if (newDiff < _smallestDiff)
            {
                updateSolution = true;
            }
            else if (newDiff == _smallestDiff)
            {
                // Check and see if the ideal is midway between 2 solutions, in which case, take
                // the heavier.
                decimal bestPlatesWeight = _bestPlates!.Sum(p => p.Weight);
                if (bestPlatesWeight < _idealWeight && newPlatesWeight > _idealWeight)
                {
                    // The total weight of the new stack is above the ideal wright by exactly the
                    // same amount as the current best solution is below it.
                    // We'll update the best solution because of the rule:
                    // "If there are 2 solutions equally close to the ideal, one lighter than it and
                    // one heavier than it, choose the heavier."
                    updateSolution = true;
                }
                else if (newPlatesWeight == bestPlatesWeight && newPlates.Count < _bestPlates!.Count)
                {
                    // The new set of plates has the same weight as the current best solution,
                    // but with fewer plates, so we'll update it. This is because of the rule:
                    // "Fewer plates is better."
                    updateSolution = true;
                }
            }

            // If we should, update the best solution found so far.
            if (updateSolution)
            {
                _bestPlates = newPlates;
                _smallestDiff = newDiff;

                // If it's exact we can stop looking.
                if (newDiff == 0)
                {
                    break;
                }
            }

            // If we're below the ideal weight, keep looking.
            if (newPlatesWeight < _idealWeight)
            {
                SearchSolutions(newPlates, plate.Weight);
            }

            // If the remaining difference is greater than the weight we just added, don't test
            // adding any smaller plates. It's unnecessary and makes the algorithm take too long.
            if (newDiff > plate.Weight)
            {
                break;
            }
        }
    }
}
