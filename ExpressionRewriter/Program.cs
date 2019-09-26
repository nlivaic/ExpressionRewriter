using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using ExpressionRewriter.Models;

namespace ExpressionRewriter
{
    class Program
    {
        static void Main(string[] args)
        {
            SeedDatabase.Seed();
            Execute();
            Console.ReadKey();
        }

        public static Expression<Func<int, bool>> isLargerThanTen() => number => number > 10;
        public static Expression<Func<int, int, bool>> isLargerThan() => (number1, number2) => number1 > number2;

        public static void Execute()
        {
            Expression<Func<int, int, bool>> expr = (n, m) => true && isLargerThan().Call(n, m);
            Expression<Func<int, int, bool>> rewrittenExpr = (Expression<Func<int, int, bool>>)new LambdaVisitor(expr).Visit("");
            Console.WriteLine($"Original expr: {expr}");
            Console.WriteLine($"Rewritten expr: {rewrittenExpr}");
            Console.WriteLine($"Rewritten expr compiled and executed: {rewrittenExpr.Compile()(3, 7)}");

            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                // var query = ctx.Orders.Where(o => o.Date > DateTime.Now.AddDays(-30));
                var query = ctx.Orders.Where(o => isLargerThan().Call(o.MyProperty, 4)).Select(o => new { o.Id }).Expand();

                var queryExpr = query.Expression as MethodCallExpression;
                MethodCallExpression methodExpr = null;
                UnaryExpression quoteExpr = null;
                LambdaExpression lambda = null;
                IQueryable<Order> rewrittenQuery = null;
                foreach (var expression in queryExpr.Arguments)
                {
                    switch (expression.NodeType)
                    {
                        case ExpressionType.Call:       // .Where
                            methodExpr = expression as MethodCallExpression;
                            quoteExpr = methodExpr.Arguments[1] as UnaryExpression;
                            lambda = quoteExpr.Operand as LambdaExpression;
                            var rewrittenLambda = Visitor.CreateFromExpression(lambda).Visit("") as LambdaExpression;
                            rewrittenQuery = ctx.Orders.Where((Expression<Func<Order, bool>>)rewrittenLambda);
                            rewrittenQuery = rewrittenQuery.Select(o => o);

                            break;
                        case ExpressionType.Quote:
                            quoteExpr = expression as UnaryExpression;
                            lambda = quoteExpr.Operand as LambdaExpression;
                            break;
                    }
                    //     var nodeType = expression.NodeType;
                }
                var result = rewrittenQuery.ToList();
            }
        }
    }




}
