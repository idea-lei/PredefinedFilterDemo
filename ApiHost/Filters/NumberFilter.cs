using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace PredefinedFilterDemo.Filters;

public sealed class NumberFilter<TEntity> : BaseFilter<TEntity> where TEntity : class
{
    private static readonly CultureInfo numberCulture = new("en-US");

    private NumberFilter() { }

    /// <summary>
    /// Filter property that within the from-to range (both inclusive)
    /// </summary>
    public static NumberFilter<TEntity> FromTo<TNumber>(string filterString, Expression<Func<TEntity, TNumber>> propertyAccessor)
        where TNumber : INumber<TNumber>, IMinMaxValue<TNumber>
    {
        string[] parts = filterString.Split("|");

        if (parts.Length != 3)
            throw new InvalidOperationException("Invalid filter string");

        if (!TNumber.TryParse(parts[1], NumberStyles.Number, numberCulture, out var from))
            from = TNumber.MinValue;
        if (!TNumber.TryParse(parts[2], NumberStyles.Number, numberCulture, out var to))
            to = TNumber.MaxValue;

        var parameter = propertyAccessor.Parameters[0];
        var property = propertyAccessor.Body;

        var fromConstant = Expression.Constant(from, typeof(TNumber));
        var toConstant = Expression.Constant(to, typeof(TNumber));

        var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, fromConstant);
        var lessThanOrEqual = Expression.LessThanOrEqual(property, toConstant);

        var andExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);

        var predicate = Expression.Lambda<Func<TEntity, bool>>(andExpression, parameter);

        return new NumberFilter<TEntity>() { Predicate = predicate };
    }

    /// <summary>
    /// Filters property that is one of the elements
    /// </summary>
    public static NumberFilter<TEntity> IsOneOf<TNumber>(string filterString, Expression<Func<TEntity, TNumber>> propertyAccessor)
         where TNumber : INumber<TNumber>, IMinMaxValue<TNumber>
    {
        string[] parts = filterString.Split("|");
        TNumber[] nums = [];
        nums = parts[1..].Select(p => TNumber.Parse(p, NumberStyles.Number, numberCulture)).ToArray();

        var containsMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TNumber));

        var arrayExpression = Expression.Constant(nums);
        var body = Expression.Call(containsMethod, arrayExpression, propertyAccessor.Body);

        var predicate = Expression.Lambda<Func<TEntity, bool>>(body, propertyAccessor.Parameters);

        return new NumberFilter<TEntity>() { Predicate = predicate };
    }

    public static NumberFilter<TEntity> GreaterThanOrEqual<TNumber>(string filterString, Expression<Func<TEntity, TNumber>> propertyAccessor)
        where TNumber : INumber<TNumber>, IMinMaxValue<TNumber>
    {
        return Compare(filterString, propertyAccessor, Expression.GreaterThanOrEqual);
    }

    public static NumberFilter<TEntity> LessThanOrEqual<TNumber>(string filterString, Expression<Func<TEntity, TNumber>> propertyAccessor)
        where TNumber : INumber<TNumber>, IMinMaxValue<TNumber>
    {
        return Compare(filterString, propertyAccessor, Expression.LessThanOrEqual);
    }

    public static NumberFilter<TEntity> Equal<TNumber>(string filterString, Expression<Func<TEntity, TNumber>> propertyAccessor)
        where TNumber : INumber<TNumber>, IMinMaxValue<TNumber>
    {
        return Compare(filterString, propertyAccessor, Expression.Equal);
    }

    private static NumberFilter<TEntity> Compare<TNumber>(
        string filterString,
        Expression<Func<TEntity, TNumber>> propertyAccessor,
        Func<Expression, Expression, BinaryExpression> compareFunc)
        where TNumber : INumber<TNumber>, IMinMaxValue<TNumber>
    {
        string[] parts = filterString.Split("|");

        if (parts.Length != 2)
            throw new InvalidOperationException("Invalid filter string");

        if (!TNumber.TryParse(parts[1], NumberStyles.Number, numberCulture, out var target))
            target = TNumber.MinValue;

        var parameter = propertyAccessor.Parameters[0];
        var property = propertyAccessor.Body;

        var targetConstant = Expression.Constant(target, typeof(TNumber));
        var expression = compareFunc(property, targetConstant);

        var predicate = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);

        return new NumberFilter<TEntity>() { Predicate = predicate };
    }
}
