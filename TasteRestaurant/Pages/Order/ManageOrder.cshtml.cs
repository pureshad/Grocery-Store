using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TasteRestaurant.Data;
using TasteRestaurant.Data.Models;
using TasteRestaurant.Utility;
using TasteRestaurant.ViewModel;

namespace TasteRestaurant.Pages.Order
{
    public class ManageOrderModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public ManageOrderModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            OrderDetailsViewModels = new List<OrderDetailsViewModel>();
        }

        [BindProperty]
        public List<OrderDetailsViewModel> OrderDetailsViewModels { get; set; }

        public void OnGet()
        {
            List<OrderHeader> orderHeaders = _dbContext.OrderHeaders.Where(w => w.Status != StaticDetails.StatusCompleted
            && w.Status != StaticDetails.StatusReady
            && w.Status != StaticDetails.StatusCancelled).OrderByDescending(w => w.PickUpTime).ToList();

            foreach (OrderHeader item in orderHeaders)
            {
                OrderDetailsViewModel indevidual = new OrderDetailsViewModel()
                {
                    OrderDetails = _dbContext.OrderDetails.Where(o => o.OrderId == item.Id).ToList(),
                    OrderHeader = item
                };

                OrderDetailsViewModels.Add(indevidual);
            }
        }

        public IActionResult OnPostOrderPrepare(int orderId)
        {
            OrderHeader orderHeader = _dbContext.OrderHeaders.Find(orderId);
            orderHeader.Status = StaticDetails.StatusInProgress;
            _dbContext.SaveChanges();

            return RedirectToPage("/Order/ManageOrder");
        }

        public IActionResult OnPostOrderReady(int orderId)
        {
            OrderHeader orderHeader = _dbContext.OrderHeaders.Find(orderId);
            orderHeader.Status = StaticDetails.StatusReady;
            _dbContext.SaveChanges();

            return RedirectToPage("/Order/ManageOrder");

        }

        public IActionResult OnPostOrderCancel(int orderId)
        {
            OrderHeader orderHeader = _dbContext.OrderHeaders.Find(orderId);
            orderHeader.Status = StaticDetails.StatusCancelled;
            _dbContext.SaveChanges();

            return RedirectToPage("/Order/ManageOrder");

        }
    }
}