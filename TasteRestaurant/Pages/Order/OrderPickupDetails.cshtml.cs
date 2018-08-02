using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteRestaurant.Data;
using TasteRestaurant.Data.Models;
using TasteRestaurant.Utility;
using TasteRestaurant.ViewModel;

namespace TasteRestaurant.Pages.Order
{
    public class OrderPickupDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderPickupDetailsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            OrderDetailsViewModel = new OrderDetailsViewModel();
        }

        [BindProperty]
        public OrderDetailsViewModel OrderDetailsViewModel { get; set; }

        public void OnGet(int orderId)
        {
            OrderDetailsViewModel.OrderHeader = _dbContext.OrderHeaders.Where(w => w.Id == orderId).FirstOrDefault();

            OrderDetailsViewModel.OrderHeader.ApplicationUser = _dbContext.Users
                .Where(w => w.Id == OrderDetailsViewModel.OrderHeader.UserId).FirstOrDefault();

            OrderDetailsViewModel.OrderDetails = _dbContext.OrderDetails.
                Where(w => w.OrderId == OrderDetailsViewModel.OrderHeader.Id).ToList();
        }

        public IActionResult OnPost(int orderId)
        {
            OrderHeader orderHeader = _dbContext.OrderHeaders.Find(orderId);
            orderHeader.Status = StaticDetails.StatusCompleted;
            _dbContext.SaveChanges();

            return RedirectToPage("/Order/ManageOrder");
        }
    }
}