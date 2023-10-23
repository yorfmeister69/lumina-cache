using System.Linq.Expressions;

namespace LuminaCache.Core;

public static class LambdaExtensions
{
    public static string GetMemberName<T>(this Expression<Func<T, object>> expression)
    {
        var name = expression.Body switch
        {
            UnaryExpression { Operand: MemberExpression memberExpression } => memberExpression.Member.Name,
            MemberExpression member => member.Member.Name,
            _ => throw new ArgumentException("The expression is not a member access expression.",
                nameof(expression))
        };

        return name;
    }
    
    public static string GetMemberName<T, TMember>(this Expression<Func<T, TMember>> expression)
    {
        var name = expression.Body switch
        {
            UnaryExpression { Operand: MemberExpression memberExpression } => memberExpression.Member.Name,
            MemberExpression member => member.Member.Name,
            _ => throw new ArgumentException("The expression is not a member access expression.",
                nameof(expression))
        };

        return name;
    }
}