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

namespace TasteRestaurant.Pages.CategoryTypes
{
    [Authorize(Policy = StaticDetails.AdminAndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _Context;

        public IndexModel(ApplicationDbContext context)
        {
            _Context = context;
        }

        public IList<CategoryType> CategoryType { get; set; }

        public async Task OnGet()
        {
            CategoryType = await _Context.CategoryType.OrderBy(w => w.DisplayOrder).ToListAsync();
        }
    }
}