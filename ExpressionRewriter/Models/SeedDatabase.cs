using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionRewriter.Models
{
	class SeedDatabase
	{
		public static void Seed()
		{
			ApplicationDbContext context = new ApplicationDbContext();
			Random rnd = new Random();
			var products = new List<Product> {
				new Product
				{
					Id = Guid.NewGuid(),
					Name = "Kayak",
					Description = "A boat for one person",
					Category = "Watersports",
					Price = 275
				},
				new Product
				{
					Id = Guid.NewGuid(),
					Name = "Lifejacket",
					Description = "Protective and fashionable",
					Category = "Watersports",
					Price = 48.95m
				},
				new Product
				{
					Id = Guid.NewGuid(),
					Name = "Soccer Ball",
					Description = "FIFA-approved size and weight",
					Category = "Soccer",
					Price = 19.50m
				},
				new Product
				{
					Id = Guid.NewGuid(),
					Name = "Corner Flags",
					Description = "Give your playing field a professional touch",
					Category = "Soccer",
					Price = 34.95m
				},
				new Product
				{
					Id = Guid.NewGuid(),
					Name = "Stadium",
					Description = "Flat-packed 35,000-seat stadium",
					Category = "Soccer",
					Price = 79500
				},
				new Product
				{
					Id = Guid.NewGuid(),
					Name = "Thinking Cap",
					Description = "Improve brain efficiency by 75%",
					Category = "Chess",
					Price = 16
				},
				new Product
				{
					Id = Guid.NewGuid(),
					Name = "Unsteady Chair",
					Description = "Secretly give your opponent a disadvantage",
					Category = "Chess",
					Price = 29.95m
				},
				new Product
				{
					Id = Guid.NewGuid(),
					Name = "Human Chess Board",
					Description = "A fun game for the family",
					Category = "Chess",
					Price = 75
				},
				new Product
				{
					Id = Guid.NewGuid(),
					Name = "Bling-Bling King",
					Description = "Gold-plated, diamond-studded King",
					Category = "Chess",
					Price = 1200
				}
			};
			var orders = new List<Order>
			{
				new Order
				{
					Id = Guid.NewGuid(),
					Date = DateTime.Now.AddDays(-10),
					Products = new List<Product>
					{
						products[0],
						products[1]
					}
				},
				new Order
				{
					Id = Guid.NewGuid(),
					Date = DateTime.Now.AddDays(-20),
					Products = new List<Product>
					{
						products[2],
						products[3],
						products[4],
						products[5],
						products[6]
					}
				},
				new Order
				{
					Id = Guid.NewGuid(),
					Date = DateTime.Now.AddDays(-30),
					Products = new List<Product>
					{
						products[7],
						products[8],
						products[0],
						products[1]
					}
				},
				new Order
				{
					Id = Guid.NewGuid(),
					Date = DateTime.Now.AddDays(-40),
					Products = new List<Product>
					{
						products[2],
						products[3],
						products[4]
					}
				},
				new Order
				{
					Id = Guid.NewGuid(),
					Date = DateTime.Now.AddDays(-50),
					Products = new List<Product>
					{
						products[5]
					}
				}
			};
			if (!context.Orders.Any())
			{
				context.Orders.AddRange(
						orders
				);

				context.SaveChanges();
			}
		}
	}
}
