using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteRestaurant.Data;
using TasteRestaurant.Data.Models;
using TasteRestaurant.Utility;

namespace TasteRestaurant.Pages.CategoryTypes
{
    [Authorize(Policy = StaticDetails.AdminAndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _Context;

        public CreateModel(ApplicationDbContext context)
        {
            _Context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CategoryType CategoryType { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            _Context.CategoryType.Add(CategoryType);
            await _Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}