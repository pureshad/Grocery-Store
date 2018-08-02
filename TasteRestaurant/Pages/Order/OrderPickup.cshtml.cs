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
    public class OrderPickupModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderPickupModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            OrderDetailsViewModels = new List<OrderDetailsViewModel>();
        }

        [BindProperty]
        public List<OrderDetailsViewModel> OrderDetailsViewModels { get; set; }

        public void OnGet(string option = null, string search = null)
        {
            if (search != null)
            {
                var user = new ApplicationUser();

                List<OrderHeader> orderHeadersList = _dbContext.OrderHeaders.Where(w => w.UserId == user.Id).OrderByDescending(w => w.PickUpTime).ToList();

                if (option == "order")
                {
                    orderHeadersList = _dbContext.OrderHeaders.Where(o => o.Id == Convert.ToInt32(search)).ToList();
                }
                else
                {
                    if (option == "email")
                    {
                        user = _dbContext.Users.Where(u => u.Email.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).FirstOrDefault();
                    }
                    else if (option == "phonenumber")
                    {
                        user = _dbContext.Users.Where(u => u.PhoneNumber.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).FirstOrDefault();
                    }
                    else if (option == "name")
                    {
                        user = _dbContext.Users.Where(u => u.FirstName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                        || u.LastName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).FirstOrDefault();
                    }
                }
                if (user != null || orderHeadersList.Count > 0)
                {
                    if (orderHeadersList.Count == 0)
                    {
                        orderHeadersList = _dbContext.OrderHeaders.Where(w => w.UserId == user.Id).OrderByDescending(w => w.PickUpTime).ToList();
                    }

                    foreach (OrderHeader item in _dbContext.OrderHeaders.Where(w => w.Status != StaticDetails.StatusReady).ToList())
                    {
                        OrderDetailsViewModel indevidual = new OrderDetailsViewModel()
                        {
                            OrderDetails = _dbContext.OrderDetails.Where(o => o.OrderId == item.Id).ToList(),
                            OrderHeader = item
                        };

                        OrderDetailsViewModels.Add(indevidual);
                    }
                }
            }
            else
            {
                List<OrderHeader> OrderHeaderList = _dbContext.OrderHeaders.Where(w => w.Status == StaticDetails.StatusReady).
                OrderByDescending(w => w.PickUpTime).ToList();

                foreach (OrderHeader item in OrderHeaderList)
                {
                    OrderDetailsViewModel indevidual = new OrderDetailsViewModel()
                    {
                        OrderDetails = _dbContext.OrderDetails.Where(o => o.OrderId == item.Id).ToList(),
                        OrderHeader = item
                    };

                    OrderDetailsViewModels.Add(indevidual);
                }
            }
        }
    }
}