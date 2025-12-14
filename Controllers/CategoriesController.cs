using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser>
        userManager, RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly ApplicationDbContext db = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            ViewBag.Categories = categories;
            return View();
        }

        public IActionResult Show(int id)
        {
            Category? category = db.Categories.Find(id);

            if (category is null)
            {
                TempData["message"] = "Categoria nu există!";
                return RedirectToAction("Index");            
            }

            return View(category);
        }

        public IActionResult New()
        {
            Category category = new Category();
            
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> New(Category cat)
        {
            bool categoryExists = await db.Categories.AnyAsync(c => c.CategoryName == cat.CategoryName);

            if (categoryExists)
            {
                ModelState.AddModelError("CategoryName", "Această denumire de categorie există deja.");
            }

            if (ModelState.IsValid)
            {
                db.Categories.Add(cat);
                await db.SaveChangesAsync();
                TempData["message"] = "Categoria a fost adăugată";
                return RedirectToAction("Index");
            }

            return View(cat);
        }

        public IActionResult Edit(int id)
        {
            Category? category = db.Categories.Find(id);

            if (category is null)
            {
                TempData["message"] = "Categoria nu există!";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category requestCategory)
        {
            Category? category = db.Categories.Find(id);

            if (category is null)
            {
                TempData["message"] = "Categoria nu există!";
                return RedirectToAction("Index");
            }

            bool categoryExists = await db.Categories.AnyAsync(c => c.CategoryName == requestCategory.CategoryName);

            if (categoryExists)
            {
                ModelState.AddModelError("CategoryName", "Această denumire de categorie există deja.");
            }

            if (ModelState.IsValid)
            {
                category.CategoryName = requestCategory.CategoryName;
                db.SaveChanges();
                TempData["message"] = "Categoria a fost modificată!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {            
            Category? category = db.Categories
                                   .Include(c => c.Products)
                                        .ThenInclude(a => a.Reviews)
                                   .Where(c => c.Id == id)
                                   .FirstOrDefault();

            if (category is null)
            {
                TempData["message"] = "Categoria nu există!";
                return RedirectToAction("Index");
            }
            else
            {
                db.Categories.Remove(category);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost ștearsă";
                return RedirectToAction("Index");
            }
        }
    }
}
