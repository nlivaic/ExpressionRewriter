using System;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionRewriter
{
    public static class ExpressionExtensions
    {
        public static R Call<T, R>(this Expression<Func<T, R>> expr, T param1)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }
        public static U Call<T, R, U>(this Expression<Func<T, R, U>> expr, T param1, R param2)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static IQueryable<T> Expand<T>(this IQueryable<T> query)
        {
						var rewrittenExpression = Visitor.CreateFromExpression(query.Expression).Visit("");
						return query.Provider.CreateQuery<T>(rewrittenExpression);
        }
    }
}
