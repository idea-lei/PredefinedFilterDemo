using System.Linq.Expressions;

namespace PredefinedFilterDemo.Filters;

/// <summary>
/// The base class of all filters. Can be also used for complex Predicate scenarios
/// </summary>
public class BaseFilter<TEntity> where TEntity : class
{
    /// <summary>
    /// If predicate is null, you must implement the Filter method by your self
    /// </summary>
    public required Expression<Func<TEntity, bool>>? Predicate { internal get; init; }

    /// <summary>
    /// If predicate is null, you must implement the Filter method by your self
    /// </summary>
    public virtual IQueryable<TEntity> Filter(IQueryable<TEntity> queryable)
    {
        if (Predicate == null)
            throw new InvalidOperationException("predicate is null and no override of Filter method");

        return queryable.Where(Predicate);
    }
}
