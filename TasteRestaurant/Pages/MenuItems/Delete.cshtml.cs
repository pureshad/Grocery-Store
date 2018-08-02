using System.IO;
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
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHostingEnvironment _hosting;

        public DeleteModel(ApplicationDbContext dbContext, IHostingEnvironment hosting)
        {
            _dbContext = dbContext;
            _hosting = hosting;
        }

        [BindProperty]
        public MenuItem MenuItem { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItem = await _dbContext.MenuItem.
                Include(m => m.CategoryType).
                Include(m => m.FoodType).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

            if (MenuItem == null)
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

            string webRootPath = _hosting.WebRootPath;

            MenuItem = await _dbContext.MenuItem.FindAsync(id);

            if (MenuItem != null)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extention = MenuItem.Image.Substring(MenuItem.Image.LastIndexOf("."), MenuItem.Image.Length - MenuItem.Image.LastIndexOf("."));

                var imagePath = Path.Combine(uploads, MenuItem.Id + extention);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _dbContext.MenuItem.Remove(MenuItem);
                await _dbContext.SaveChangesAsync();

            }
            return RedirectToPage("./Index");
        }
    }
}