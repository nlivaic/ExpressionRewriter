using System;

namespace ExpressionRewriter.Models
{
	public class Product
	{
		public Guid Id { get; set; }
		public string Name { get; internal set; }
		public string Description { get; internal set; }
		public string Category { get; internal set; }
		public decimal Price { get; internal set; }
	}
}