using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TasteRestaurant.Data;
using TasteRestaurant.Data.Models;

namespace TasteRestaurant.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public DetailsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public ShoppingCart CartObj { get; set; }

        public void OnGet(int id)
        {
            var menuItemFromdb = _dbContext.MenuItem
                .Include(c => c.CategoryType)
                .Include(f => f.FoodType)
                .Where(m => m.Id == id).FirstOrDefault();

            CartObj = new ShoppingCart()
            {
                MenuItemId = menuItemFromdb.Id,
                MenuItem = menuItemFromdb
            };
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObj.ApplicationUserId = claim.Value;

                ShoppingCart shoppingCartDb = await _dbContext.ShoppingCarts.
                    Where(c => c.ApplicationUserId == CartObj.ApplicationUserId
                             && c.MenuItemId == CartObj.MenuItemId).FirstOrDefaultAsync().ConfigureAwait(false);

                if (shoppingCartDb == null)
                {
                    _dbContext.ShoppingCarts.Add(CartObj);
                }
                else
                {
                    shoppingCartDb.Count += CartObj.Count;
                }

                await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                //Add Setion
                var count = _dbContext.ShoppingCarts.Where(c => c.ApplicationUserId == CartObj.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32("CartCount", count);
                StatusMessage = "Item added to cart!";

                return RedirectToPage("Index");
            }
            else
            {
                var menuItemFromdb = _dbContext.MenuItem.Include(c => c.CategoryType).Include(f => f.FoodType).FirstOrDefault();

                CartObj = new ShoppingCart()
                {
                    MenuItemId = menuItemFromdb.Id,
                    MenuItem = menuItemFromdb
                };

                return Page();
            }
        }
    }
}