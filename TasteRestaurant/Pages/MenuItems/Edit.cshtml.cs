using System;
using System.Collections.Generic;
using System.IO;
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
using TasteRestaurant.ViewModel;

namespace TasteRestaurant.Pages.MenuItems
{
    [Authorize(Policy = StaticDetails.AdminAndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHostingEnvironment _hosting;

        public EditModel(ApplicationDbContext dbContext, IHostingEnvironment hosting)
        {
            _dbContext = dbContext;
            _hosting = hosting;
        }

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM = new MenuItemViewModel()
            {
                MenuItem = _dbContext.MenuItem.Include(w => w.FoodType).SingleOrDefault(w => w.Id == id),
                CategoryType = _dbContext.CategoryType.ToList(),
                FoodType = _dbContext.FoodTypes.ToList()
            };

            if (MenuItemVM.MenuItem == null)
            {
                NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string webRootPath = _hosting.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = _dbContext.MenuItem.Where(m => m.Id == MenuItemVM.MenuItem.Id).FirstOrDefault();

            if (files[0]?.Length > 0)
            {
                var upload = Path.Combine(webRootPath, "images");

                var extention = menuItemFromDb.Image.Substring(files[0].FileName.LastIndexOf("."),
                    menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf("."));

                if (System.IO.File.Exists(Path.Combine(upload, MenuItemVM.MenuItem.Id + extention)))
                {
                    System.IO.File.Delete(Path.Combine(upload, MenuItemVM.MenuItem.Id + extention));
                }

                extention = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."),
                            files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                using (var fileStream = new FileStream(Path.Combine(upload, MenuItemVM.MenuItem.Id + extention), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extention;
            }


            if (MenuItemVM.MenuItem.Image != null)
            {
                menuItemFromDb.Image = MenuItemVM.MenuItem.Image;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuItemFromDb.FoodTypeId = MenuItemVM.MenuItem.FoodTypeId;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}