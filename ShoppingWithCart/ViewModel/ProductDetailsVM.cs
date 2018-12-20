using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingWithCart.ViewModel
{
    public class ProductDetailsVM
    {
        public int productId { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string productName { get; set; }
        public string description { get; set; }
        public string imagePath { get; set; }
        public int price { get; set; }
    }
}