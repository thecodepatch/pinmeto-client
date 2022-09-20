namespace TheCodePatch.PinMeToClient.Exceptions;

/// <summary>
/// Guards for parameters.
/// </summary>
internal static class Guard
{
    /// <summary>
    /// Checks that a parameter is within the specified range
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="parameterValue">The value of the parameter.</param>
    /// <param name="min">The minimum boundry (inclusive).</param>
    /// <param name="max">The maximum boundry (inclusive).</param>
    /// <exception cref="InvalidInputException">When the parameter value is outside the specified boundries.</exception>
    public static void IsWithinRange(string parameterName, uint? parameterValue, int min, int max)
    {
        if (parameterValue < min || parameterValue > max)
        {
            throw new InvalidInputException(
                parameterName,
                $"{parameterName} must be a value between 0 and 250."
            );
        }
    }
}
