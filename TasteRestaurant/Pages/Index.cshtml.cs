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
using TasteRestaurant.ViewModel;

namespace TasteRestaurant.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            CartObj = new ShoppingCart();

        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public IndexViewModel IndexViewModel { get; set; }


        [BindProperty]
        public ShoppingCart CartObj { get; set; }


        public async Task OnGet()
        {
            IndexViewModel = new IndexViewModel()
            {
                MenuItems = await _dbContext.MenuItem
                .Include(m => m.CategoryType)
                .Include(m => m.FoodType).ToListAsync(),

                CategoryTypes = _dbContext.CategoryType.OrderBy(c => c.DisplayOrder)
            };
        }
    }
}
