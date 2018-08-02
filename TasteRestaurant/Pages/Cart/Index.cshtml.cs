using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteRestaurant.Data;
using TasteRestaurant.Data.Models;
using TasteRestaurant.Utility;
using TasteRestaurant.ViewModel;

namespace TasteRestaurant.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public OrderDetailsCart DetailsCart { get; set; }

        public void OnGet()
        {
            DetailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };

            DetailsCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _dbContext.ShoppingCarts.Where(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                DetailsCart.ShoppingCarts = cart.ToList();
            }

            foreach (var item in DetailsCart.ShoppingCarts)
            {
                item.MenuItem = _dbContext.MenuItem.FirstOrDefault(m => m.Id == item.MenuItemId);
                DetailsCart.OrderHeader.OrderTotal += (item.MenuItem.Price * item.Count);

                if (item.MenuItem.Description.Length > 100)
                {
                    item.MenuItem.Description = item.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            DetailsCart.OrderHeader.PickUpTime = DateTime.Now;
        }

        public async Task<IActionResult> OnPostPlus(int cartId)
        {
            var cart = _dbContext.ShoppingCarts.Where(c => c.Id == cartId).FirstOrDefault();
            cart.Count++;

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToPage("/Cart/Index");
        }

        public async Task<IActionResult> OnPostMinusAsync(int cartId)
        {
            var cart = _dbContext.ShoppingCarts.Where(c => c.Id == cartId).FirstOrDefault();

            if (cart.Count == 1)
            {
                _dbContext.Remove(cart);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                var count = _dbContext.ShoppingCarts.Where(c => c.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32("CartCount", count);
            }
            else
            {
                cart.Count--;
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }

            return RedirectToPage("/Cart/Index");
        }

        //btnPlaceOrder
        public IActionResult OnPost()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            DetailsCart.ShoppingCarts = _dbContext.ShoppingCarts.Where(c => c.ApplicationUserId == claim.Value).ToList();

            OrderHeader orderHeader = DetailsCart.OrderHeader;
            DetailsCart.OrderHeader.OrderDate = DateTime.Now;
            DetailsCart.OrderHeader.UserId = claim.Value;
            DetailsCart.OrderHeader.Status = StaticDetails.StatusSubmited;
            _dbContext.OrderHeaders.Add(orderHeader);
            _dbContext.SaveChanges();

            foreach (var item in DetailsCart.ShoppingCarts)
            {
                item.MenuItem = _dbContext.MenuItem.FirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetail orderDetail = new OrderDetail()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = orderHeader.Id,
                    Name = item.MenuItem.Name,
                    Description = item.MenuItem.Description,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                _dbContext.OrderDetails.Add(orderDetail);
            }
            _dbContext.ShoppingCarts.RemoveRange(DetailsCart.ShoppingCarts);

            HttpContext.Session.SetInt32("CartCount", 0);
            _dbContext.SaveChanges();

            return RedirectToPage("../Order/OrderConfirmation", new { id = orderHeader.Id });
        }
    }
}