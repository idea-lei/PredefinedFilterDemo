using System.Linq.Expressions;

namespace PredefinedFilterDemo.Filters;

public class BoolFilter<TEntity> : BaseFilter<TEntity> where TEntity : class
{
    private BoolFilter() { }

    public static BoolFilter<TEntity> IsTrue(Expression<Func<TEntity, bool>> propertyAccessor)
        => Condition(propertyAccessor, true);

    public static BoolFilter<TEntity> IsFalse(Expression<Func<TEntity, bool>> propertyAccessor)
        => Condition(propertyAccessor, false);


    private static BoolFilter<TEntity> Condition(Expression<Func<TEntity, bool>> propertyAccessor, bool forTrue)
    {
        var parameter = propertyAccessor.Parameters[0];
        var property = propertyAccessor.Body;

        var targetConstant = Expression.Constant(forTrue, typeof(bool));
        var expression = Expression.Equal(property, targetConstant);

        var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);

        return new BoolFilter<TEntity>() { Predicate = predicate };
    }
}
