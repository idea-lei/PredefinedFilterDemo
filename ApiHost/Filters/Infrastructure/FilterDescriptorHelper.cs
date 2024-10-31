using PredefinedFilterDemo.Dtos;
using System.ComponentModel;
using System.Reflection;

namespace PredefinedFilterDemo.Filters.Infrastructure;

public static class FilterDescriptorHelper
{
    /// <summary>
    /// Get the usecase-specific name and description for the filter
    /// </summary>
    public static FilterDescriptorDto GetDescriptorDto<TFilterEnum>(this TFilterEnum e) where TFilterEnum : struct, Enum
    {
        Type filterType = typeof(TFilterEnum);
        MemberInfo memberInfo = filterType.GetMember(e.ToString())[0];

        var patternAttribute = memberInfo.GetCustomAttribute<ParameterPatternAttribute>()
            ?? throw new Exception($"FilterPatternAttribute not found on {e}");

        var descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();

        return new FilterDescriptorDto
        {
            Name = memberInfo.Name,
            Pattern = $"{memberInfo.Name}|{patternAttribute.Pattern}",
            PatternExample = $"{memberInfo.Name}|{patternAttribute.Example}",
            Description = descriptionAttribute?.Description
        };
    }

    public static IEnumerable<FilterDescriptorDto> GetDescriptorDtos<TFilterEnum>() where TFilterEnum : struct, Enum
    {
        return Enum.GetValues<TFilterEnum>().Select(e => GetDescriptorDto(e));
    }
}
