using System.Linq.Expressions;

namespace PredefinedFilterDemo.Filters;

/// <remarks>
/// Works on both reference type and Nullable<T> where T : struct <br/>
/// </remarks>
public class NullFilter<TEntity> : BaseFilter<TEntity> where TEntity : class
{
    public static NullFilter<TEntity> NotNull<TProperty>(Expression<Func<TEntity, TProperty>> propertyAccessor)
        => Create(propertyAccessor, Expression.NotEqual);

    public static NullFilter<TEntity> Null<TProperty>(Expression<Func<TEntity, TProperty>> propertyAccessor)
        => Create(propertyAccessor, Expression.Equal);

    private static NullFilter<TEntity> Create<TProperty>(
        Expression<Func<TEntity, TProperty>> propertyAccessor,
        Func<Expression, Expression, BinaryExpression> checkFunc)
    {
        var parameter = propertyAccessor.Parameters[0];
        var property = propertyAccessor.Body;
        var nullCheckExpression = checkFunc(property, Expression.Constant(null, typeof(TProperty)));

        var predicate = Expression.Lambda<Func<TEntity, bool>>(nullCheckExpression, parameter);
        return new NullFilter<TEntity>() { Predicate = predicate };
    }
}
