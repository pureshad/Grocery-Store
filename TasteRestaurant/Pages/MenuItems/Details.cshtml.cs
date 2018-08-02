using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TasteRestaurant.Data;
using TasteRestaurant.Data.Models;
using TasteRestaurant.Utility;

namespace TasteRestaurant.Pages.MenuItems
{
    [Authorize(Policy = StaticDetails.AdminAndUser)]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHostingEnvironment _hosting;

        public DetailsModel(ApplicationDbContext dbContext, IHostingEnvironment hosting)
        {
            _dbContext = dbContext;
            _hosting = hosting;
        }

        public MenuItem MenuItem { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItem = await _dbContext.MenuItem.Include(m => m.CategoryType).Include(m => m.FoodType).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

            if (MenuItem == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}