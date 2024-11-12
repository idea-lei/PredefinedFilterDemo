using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PredefinedFilterDemo.Filters;

public sealed class StringFilter<TEntity> : BaseFilter<TEntity> where TEntity : class
{
    private StringFilter() { }

    /// <summary>
    /// Filters the property that contains the target string (case insensitive) using SQL <c>LIKE</c> method
    /// </summary>
    public static StringFilter<TEntity> Contains(string filterString, Expression<Func<TEntity, string>> propertyAccessor)
    {
        int pipeIndex = filterString.IndexOf('|');
        var stringType = typeof(string);
        string arg = filterString[(pipeIndex + 1)..].Trim();

        var parameter = propertyAccessor.Parameters[0];
        var propertyExpression = propertyAccessor.Body;

        // Ensure the property is of type string
        var strProperty = propertyExpression;

        // Build the pattern for case-insensitive search
        var likePattern = $"%{arg}%";
        var likePatternExpression = Expression.Constant(likePattern);

        var efFunctionsProperty = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        // Get the EF.Functions.Like method
        var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
            nameof(DbFunctionsExtensions.Like),
            [typeof(DbFunctions), stringType, stringType]
        )!;

        // Build the method call expression for EF.Functions.Like
        var likeCallExpression = Expression.Call(
            likeMethod,
            efFunctionsProperty,
            strProperty,
            likePatternExpression
        );

        var predicate = Expression.Lambda<Func<TEntity, bool>>(likeCallExpression, parameter);

        return new StringFilter<TEntity>() { Predicate = predicate };
    }

    public static StringFilter<TEntity> Equal(string filterString, Expression<Func<TEntity, string>> propertyAccessor, bool ignoreCase = true)
    {
        int pipeIndex = filterString.IndexOf('|');
        string arg = filterString[(pipeIndex + 1)..].Trim();

        var parameter = propertyAccessor.Parameters[0];
        var propertyExpression = propertyAccessor.Body;

        if (ignoreCase)
        {
            var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
            propertyExpression = Expression.Call(propertyExpression, toLowerMethod!);
            arg = arg.ToLower();
        }

        var argExpression = Expression.Constant(arg);
        var equalExpression = Expression.Equal(propertyExpression, argExpression);
        var predicate = Expression.Lambda<Func<TEntity, bool>>(equalExpression, parameter);

        return new StringFilter<TEntity> { Predicate = predicate };
    }
}
