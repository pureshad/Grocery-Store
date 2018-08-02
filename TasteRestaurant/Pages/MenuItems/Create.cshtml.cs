using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteRestaurant.Data;
using TasteRestaurant.Data.Models;
using TasteRestaurant.Utility;
using TasteRestaurant.ViewModel;

namespace TasteRestaurant.Pages.MenuItems
{
    [Authorize(Policy = StaticDetails.AdminAndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHostingEnvironment _hostingEnvironment; //For Uploading the image on the server

        public CreateModel(ApplicationDbContext dbContext, IHostingEnvironment hostingEnvironment)
        {
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public IActionResult OnGet()
        {
            MenuItemVM = new MenuItemViewModel
            {
                MenuItem = new MenuItem(),
                FoodType = _dbContext.FoodTypes.ToList(),
                CategoryType = _dbContext.CategoryType.ToList()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _dbContext.MenuItem.Add(MenuItemVM.MenuItem);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            //Image being saved

            string webRootPath = _hostingEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _dbContext.MenuItem.Find(MenuItemVM.MenuItem.Id);

            if (files[0]?.Length > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extention = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                using (var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extention), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extention;
            }
            else
            {
                var upload = Path.Combine(webRootPath, @"images\" + StaticDetails.DefaultFoodImage);
                System.IO.File.Copy(upload, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".jpg");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".jpg";
            }
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToPage("./Index");
        }
    }
}