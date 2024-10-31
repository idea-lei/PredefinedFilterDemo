namespace PredefinedFilterDemo.Filters.Infrastructure;

[AttributeUsage(AttributeTargets.Field)]
public class ParameterPatternAttribute : Attribute
{
    public required string Pattern { get; init; }
    public string? Example { get; init; }
}
