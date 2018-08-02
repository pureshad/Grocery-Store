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
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteModel(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        [BindProperty]
        public CategoryType CategoryType { get; set; }

        public async Task <IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CategoryType = await _dbContext.CategoryType.SingleOrDefaultAsync(c => c.Id == id);

            if (CategoryType == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CategoryType = await _dbContext.CategoryType.FindAsync(id);

            if (CategoryType != null)
            {
                _dbContext.CategoryType.Remove(CategoryType);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

    }
}