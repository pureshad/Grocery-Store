using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasteRestaurant.Data.Models;

namespace TasteRestaurant.ViewModel
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> ShoppingCarts{ get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
