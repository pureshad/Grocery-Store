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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _Context;

        public EditModel(ApplicationDbContext context)
        {
            _Context = context;
        }

        [BindProperty]
        public CategoryType CategoryType{ get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CategoryType = await _Context.CategoryType.SingleOrDefaultAsync(c => c.Id == id);

            if (CategoryType == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _Context.Attach(CategoryType).State = EntityState.Modified;

            await _Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}