using System;
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
				var orders = ctx.Orders.Where(o => o.Date > DateTime.Now.AddDays(-30)).ToList();
			}
		}
	}
}
