using System.Linq.Expressions;

namespace PredefinedFilterDemo.Filters;

/// <remarks>
/// Works on both reference type and Nullable<T> where T : struct <br/>
/// </remarks>
public class NullFilter<TEntity> : BaseFilter<TEntity> where TEntity : class
{
    /// <summary>
    /// This method is generally not needed, null value will by default be ruled out
    /// </summary>
    public static NullFilter<TEntity> NotNull<TProperty>(Expression<Func<TEntity, TProperty>> propertyAccessor)
        => Create(propertyAccessor, ExpressionType.NotEqual);

    /// <summary>
    /// To only include null values
    /// </summary>
    public static NullFilter<TEntity> Null<TProperty>(Expression<Func<TEntity, TProperty>> propertyAccessor)
        => Create(propertyAccessor, ExpressionType.Equal);

    private static NullFilter<TEntity> Create<TProperty>(
        Expression<Func<TEntity, TProperty>> propertyAccessor,
        ExpressionType expressionType)
    {
        var parameter = propertyAccessor.Parameters[0];
        var property = propertyAccessor.Body;

        // Determine if TProperty is nullable (either a reference type or Nullable<T>)
        var isNullable = !typeof(TProperty).IsValueType || Nullable.GetUnderlyingType(typeof(TProperty)) != null;

        Expression predicateBody;

        if (isNullable)
        {
            // For nullable types, compare the property to null
            var nullConstant = Expression.Constant(null, property.Type);
            predicateBody = Expression.MakeBinary(expressionType, property, nullConstant);
        }
        else
            predicateBody = Expression.Constant(expressionType != ExpressionType.Equal);


        var predicate = Expression.Lambda<Func<TEntity, bool>>(predicateBody, parameter);
        return new NullFilter<TEntity> { Predicate = predicate };
    }
}
