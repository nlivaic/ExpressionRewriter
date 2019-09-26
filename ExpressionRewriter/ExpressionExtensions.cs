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
            var queryExpr = query.Expression as MethodCallExpression;
            MethodCallExpression methodExpr = null;
            UnaryExpression quoteExpr = null;
            LambdaExpression lambda = null;
            IQueryable<T> rewrittenQuery = null;
            foreach (var expression in queryExpr.Arguments)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Call:       // .Where
                        methodExpr = expression as MethodCallExpression;
                        quoteExpr = methodExpr.Arguments[1] as UnaryExpression;
                        lambda = quoteExpr.Operand as LambdaExpression;
                        var rewrittenLambda = Visitor.CreateFromExpression(lambda).Visit("") as LambdaExpression;
                        rewrittenQuery = ctx.Orders.Where((Expression<Func<T, bool>>)rewrittenLambda);

                        break;
                    case ExpressionType.Quote:
                        quoteExpr = expression as UnaryExpression;
                        lambda = quoteExpr.Operand as LambdaExpression;
                        rewrittenQuery = rewrittenQuery.Select(o => o);
                        break;
                }
                //     var nodeType = expression.NodeType;
            }
            return rewrittenQuery;
        }
    }
}
