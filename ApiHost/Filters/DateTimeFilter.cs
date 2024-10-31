using System.Data.SqlTypes;
using System.Linq.Expressions;

namespace PredefinedFilterDemo.Filters;

public sealed class DateTimeFilter<TEntity> : BaseFilter<TEntity> where TEntity : class
{
    private DateTimeFilter() { }
    /// <summary>
    /// Filter property that within the from-to range (both inclusive)
    /// </summary>
    public static DateTimeFilter<TEntity> FromTo(string filterString, Expression<Func<TEntity, DateTime>> propertyAccessor)
    {
        string[] parts = filterString.Split("|");

        if (parts.Length != 3)
            throw new InvalidOperationException("Invalid filter string");

        if (!DateTime.TryParse(parts[1], out var from))
            from = SqlDateTime.MinValue.Value;

        if (!DateTime.TryParse(parts[2], out var to))
            to = SqlDateTime.MaxValue.Value;

        if (to.TimeOfDay == TimeSpan.Zero)
            to = to.AddDays(1); // include the 'to' date

        var parameter = propertyAccessor.Parameters[0];
        var property = propertyAccessor.Body;

        // Capture 'from' and 'to' in a closure to force EF to generate parameters
        var closure = new { f = from, t = to };
        var fromExpr = Expression.Property(Expression.Constant(closure), "f");
        var toExpr = Expression.Property(Expression.Constant(closure), "t");

        var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, fromExpr);
        var lessThan = Expression.LessThan(property, toExpr);
        var andExpression = Expression.AndAlso(greaterThanOrEqual, lessThan);

        var predicate = Expression.Lambda<Func<TEntity, bool>>(andExpression, parameter);

        return new DateTimeFilter<TEntity>() { Predicate = predicate };
    }
}
