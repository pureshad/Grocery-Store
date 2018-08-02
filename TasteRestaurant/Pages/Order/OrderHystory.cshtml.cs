using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TasteRestaurant.Data;
using TasteRestaurant.Data.Models;
using TasteRestaurant.ViewModel;

namespace TasteRestaurant.Pages.Order
{
    public class OrderHystoryModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderHystoryModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public List<OrderDetailsViewModel> OrderDetailsViewModel { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel = new List<OrderDetailsViewModel>();

            List<OrderHeader> orderHeadersList = await _dbContext.OrderHeaders.Where(w => w.UserId == claim.Value).ToListAsync().ConfigureAwait(false);

            if (id == 0 && orderHeadersList.Count > 4)
            {
                orderHeadersList = orderHeadersList.Take(5).ToList();
            }

            foreach (OrderHeader item in orderHeadersList)
            {
                OrderDetailsViewModel indevidual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _dbContext.OrderDetails.Where(w => w.Id == item.Id).ToListAsync().ConfigureAwait(false)
                };

                OrderDetailsViewModel.Add(indevidual);
            }

            return Page();
        }
    }
}