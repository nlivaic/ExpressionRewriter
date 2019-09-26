using System;
using System.Collections.Generic;

namespace ExpressionRewriter.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public List<Product> Products { get; set; }
        public int MyProperty { get; set; }
    }
}