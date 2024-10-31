using PredefinedFilterDemo.Dtos;

namespace PredefinedFilterDemo.Filters.Infrastructure;

/// <summary>
/// Filter collection for an endpoint
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IFilterCollection<TEntity> where TEntity : class
{
    /// <param name="filters">If null, FilterCollection will be empty (not null). To bypass null check.</param>
    static abstract IFilterCollection<TEntity> Parse(string[]? filters, params object?[]? args);

    static abstract IEnumerable<FilterDescriptorDto> FilterDescriptors { get; }

    IEnumerable<BaseFilter<TEntity>> FilterCollection { get; }

    IQueryable<TEntity> Filter(IQueryable<TEntity> query)
    {
        foreach (var filter in FilterCollection)
            query = filter.Filter(query);
        return query;
    }
}
