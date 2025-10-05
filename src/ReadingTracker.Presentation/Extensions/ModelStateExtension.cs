using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ReadingTracker.Presentation.Extensions;

/// <summary>
/// Extension methods for ModelStateDictionary
/// </summary>
public static class ModelStateExtension
{
    /// <summary>
    /// Converts ModelState errors to a dictionary format
    /// </summary>
    /// <param name="modelState">The ModelStateDictionary instance</param>
    /// <returns>Dictionary with error messages grouped by key</returns>
    public static Dictionary<string, List<string>> GetErrorMessages(this ModelStateDictionary modelState)
    {
        return modelState
            .Where(ms => ms.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToList()
            );
    }
}

