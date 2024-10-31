using System.ComponentModel;
using System.Reflection;

namespace PredefinedFilterDemo.Dtos;

/// <summary>
/// FilterDescriptor is instance level descriptor of a filter.
/// </summary>
public class FilterDescriptorDto
{
    /// <summary>
    /// The name of the filter in the specific case. like CreationTimeFilter, ReleaseTimeFilter
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// How is the pattern represented, multiple parameters separated by '|', e.g. <code>2024-01-01|2024-01-02</code>
    /// </summary>
    public required string Pattern { get; init; }

    public string? PatternExample { get; init; }

    /// <summary>
    /// extra description of the filter
    /// </summary>
    public string? Description { get; init; }
}

