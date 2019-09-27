using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using ExpressionRewriter.Models;
using Microsoft.EntityFrameworkCore;

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
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var rewrittenQuery = ctx.Orders
									.Where(o => isLargerThan().Call(o.MyProperty, 2))
									.Include(o => o.Products)
									.Select(o => o).Expand();
                var result = rewrittenQuery.ToList();
            }
        }
    }
}
