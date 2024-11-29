using PredefinedFilterDemo.Dtos;
using PredefinedFilterDemo.Filters.Infrastructure;
using System.Linq.Expressions;

namespace PredefinedFilterDemo.Filters.SharedFilterCollections;

/// <summary>
/// To avoid duplication of filter collections with exact same filters.
/// </summary>
public sealed class DateTimeFilterCollection<TEntity> : IFilterCollection<TEntity> where TEntity : class
{
    /// <summary>
    /// create a filter collection with only one DateTimeFilter
    /// </summary>
    /// <remarks>
    /// <b>⚠️Warning⚠️</b> Must be called before other static members get called
    /// </remarks>
    public static void Init(
        Expression<Func<TEntity, DateTime>> propertyAccessor,
        string filterName,
        string? description = null)
    {
        _propertyAccessor = propertyAccessor;

        FilterDescriptors = [
            new FilterDescriptorDto{
                Name = filterName,
                Description = description,
                Pattern = "(DateTime)from|(DateTime)to",
                PatternExample = "2023-01-01|2024-12-31",
            }
        ];
        inited = true;
    }

    private static bool inited = false;

    private static Expression<Func<TEntity, DateTime>> _propertyAccessor;

    public static IEnumerable<FilterDescriptorDto> FilterDescriptors { get; private set; }

    private List<BaseFilter<TEntity>> _filterList = [];
    public IEnumerable<BaseFilter<TEntity>> FilterCollection => _filterList;

    public static IFilterCollection<TEntity> Parse(string[]? filters, params object?[]? args)
    {
        if (!inited) throw new InvalidOperationException("The shared filter collection is not initialized");

        var collection = new DateTimeFilterCollection<TEntity>();

        if (filters == null)
            return collection;

        foreach (var f in filters)
        {
            var parts = f.Split('|');

            if (!string.Equals(parts[0], FilterDescriptors.First().Name, StringComparison.OrdinalIgnoreCase))
                continue;

            collection._filterList.Add(DateTimeFilter<TEntity>.FromTo(f, _propertyAccessor));
            break;
        }

        return collection;
    }
}

