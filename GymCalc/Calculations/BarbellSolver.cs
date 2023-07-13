using GymCalc.Data.Repositories;

namespace GymCalc.Calculations;

internal class BarbellSolver
{
    private List<double> _availablePlateWeights;

    private List<double> _bestSolution;

    private double _idealWeight;

    private double _smallestDiff;

    internal async Task<Dictionary<double, List<double>>> CalculateResults(double maxWeight,
        double barWeight)
    {
        // Get the available plate weights ordered from heaviest to lightest.
        var availablePlates = await PlateRepository.GetAll(true, false);
        _availablePlateWeights = availablePlates.Select(p => p.Weight).ToList();

        var results = new Dictionary<double, List<double>>();

        // Get the best solution for each percentage fraction of the maxWeight we're interested in.
        // For now we'll hard code 50%, 60% ... 100%, but this might be configurable later.
        for (var percent = 100; percent >= 50; percent -= 10)
        {
            var idealTotal = maxWeight * percent / 100.0;
            var idealPlates = (idealTotal - barWeight) / 2.0;

            // Get the set of plates that is closest to the ideal weight.
            results[percent] = GetBestPlates(idealPlates);
        }

        return results;
    }

    /// <summary>
    /// Find the stack of plates that will produce the closest total weight to the ideal weight.
    /// </summary>
    /// <returns></returns>
    private List<double> GetBestPlates(double idealWeight)
    {
        // Initialize fields.
        _idealWeight = idealWeight;
        _smallestDiff = idealWeight;
        _bestSolution = new List<double>();

        // Search the solutions space.
        FindSolutions(_availablePlateWeights[0], _bestSolution);

        // Return the best solution found during the search.
        return _bestSolution;
    }

    /// <summary>
    /// Recursive function to generate and test new solutions.
    /// </summary>
    /// <param name="maxPlateWeight">The largest next plate that can be added.</param>
    /// <param name="currentStack">The stack of plates so far.</param>
    private void FindSolutions(double maxPlateWeight, List<double> currentStack)
    {
        foreach (var newPlateWeight in _availablePlateWeights)
        {
            // Only add plates that are less than or equal to the largest plate added so far.
            // Ensuring plates are added in order of decreasing weight eliminates duplicate
            // solutions.
            if (newPlateWeight > maxPlateWeight)
            {
                continue;
            }

            // Create a new stack from the current stack plus the new plate.
            var newStack = new List<double>();
            newStack.AddRange(currentStack);
            newStack.Add(newPlateWeight);

            // Test the solution.
            var sum = newStack.Sum();
            var diff = double.Abs(sum - _idealWeight);

            // Check if this is a new best solution.
            if (diff < _smallestDiff)
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
                FindSolutions(newPlateWeight, newStack);
            }
        }
    }
}
