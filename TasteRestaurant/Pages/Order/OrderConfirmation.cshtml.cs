using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteRestaurant.Data;
using TasteRestaurant.ViewModel;

namespace TasteRestaurant.Pages.Order
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderConfirmationModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public OrderDetailsViewModel OrderDetailsViewModel { get; set; }

        public void OnGet(int id)
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = _dbContext.OrderHeaders.Where(o => o.Id == id).FirstOrDefault(),
                OrderDetails = _dbContext.OrderDetails.Where(w => w.OrderId == id).ToList()
            };
        }
    }
}