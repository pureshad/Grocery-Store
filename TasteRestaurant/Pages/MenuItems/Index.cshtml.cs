using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteRestaurant.Data;
using TasteRestaurant.Data.Models;
using TasteRestaurant.Utility;

namespace TasteRestaurant.Pages.MenuItems
{
    [Authorize(Policy = StaticDetails.AdminAndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<MenuItem> MenuItem { get; set; }

        public async Task OnGet()
        {
            MenuItem = await _dbContext.MenuItem
                .Include(m => m.CategoryType)
                .Include(m => m.FoodType)
                .ToListAsync();
        }
    }
}