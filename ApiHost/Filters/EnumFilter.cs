using System.Linq.Expressions;
using System.Reflection;

namespace PredefinedFilterDemo.Filters;

public sealed class EnumFilter<TEntity> : BaseFilter<TEntity> where TEntity : class
{
    private EnumFilter() { }

    /// <summary>
    /// Filters property that is one of the elements
    /// </summary>
    public static EnumFilter<TEntity> IsOneOf<TEnum>(string filterString, Expression<Func<TEntity, TEnum>> propertyAccessor) where TEnum : struct, Enum
    {
        string[] parts = filterString.Split("|");
        TEnum[] enums = null!;

        enums = parts[1..].Select(Enum.Parse<TEnum>).ToArray(); // throw on failure

        var containsMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TEnum));

        var arrayExpression = Expression.Constant(enums);
        var body = Expression.Call(containsMethod, arrayExpression, propertyAccessor.Body);

        var predicate = Expression.Lambda<Func<TEntity, bool>>(body, propertyAccessor.Parameters);

        return new EnumFilter<TEntity>() { Predicate = predicate };
    }

    /// <summary>
    /// Filters property that equals the parameter
    /// </summary>
    public static EnumFilter<TEntity> Equals<TEnum>(string filterString, Expression<Func<TEntity, TEnum>> propertyAccessor) where TEnum : struct, Enum
    {
        string[] parts = filterString.Split("|");

        if (parts.Length != 2)
            throw new InvalidOperationException("Invalid filter string");

        if (!Enum.TryParse<TEnum>(parts[1], out var target))
            throw new InvalidOperationException("Invalid filter string, can not parse Enum");

        var parameter = propertyAccessor.Parameters[0];
        var propertyExpression = propertyAccessor.Body;

        var targetConstant = Expression.Constant(target, typeof(TEnum));
        var equalExpression = Expression.Equal(propertyExpression, targetConstant);

        var predicate = Expression.Lambda<Func<TEntity, bool>>(equalExpression, parameter);

        return new EnumFilter<TEntity>() { Predicate = predicate };
    }
}
