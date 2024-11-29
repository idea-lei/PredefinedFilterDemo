using System.Linq.Expressions;

namespace PredefinedFilterDemo.Filters.Infrastructure;

public enum CombineLogic : byte
{
    Or = 0,
    And = 1,
}

/// <summary>
/// FilterCombinator is to combine multiple filters on an entity.
/// <para>
/// The AND logic is useless here since it is by default. If you use this class, you almost always use the OR logic.
/// </para>
/// </summary>
[Obsolete("Not ready for production yet, need more test")]
public static class FilterCombinator
{
    public static BaseFilter<TEntity> Combine<TEntity>(CombineLogic logic, params BaseFilter<TEntity>[] filters) where TEntity : class
    {
        if (filters.Length == 0)
            throw new ArgumentException("Filter combinator received no filter");

        Func<Expression, Expression, BinaryExpression> merge = logic switch
        {
            CombineLogic.And => Expression.AndAlso,
            CombineLogic.Or => Expression.OrElse,
            _ => throw new InvalidOperationException("Invalid combination logic")
        };

        Expression<Func<TEntity, bool>>? predicate = null;

        foreach (var filter in filters)
        {
            if (filter.Predicate == null)
                throw new ArgumentException("Filter combinator received a filter with no Predicate");

            if (predicate == null) // init
            {
                predicate = filter.Predicate;
                continue;
            }

            predicate = predicate.CombineWith(filter.Predicate!, merge);
        }

        return new BaseFilter<TEntity>() { Predicate = predicate };
    }

    private static Expression<Func<T, bool>> CombineWith<T>(
        this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second,
        Func<Expression, Expression, BinaryExpression> merge)
    {
        var parameter = Expression.Parameter(typeof(T), "x");

        var leftVisitor = new ReplaceParameterVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);

        var rightVisitor = new ReplaceParameterVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);

        var body = merge(left, right);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private class ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == oldParameter ? newParameter : base.VisitParameter(node);
        }
    }
}