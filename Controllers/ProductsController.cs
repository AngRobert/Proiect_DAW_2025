using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Controllers {
    public class ProductsController : Controller {
        // partea de identity si baza de date
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager) {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index() {
            var products = db.Products.Include(p => p.Category);

            ViewBag.Products = products;
            
            if (TempData.ContainsKey("goodMessage")) {
                ViewBag.GoodMsg = TempData["goodMessage"];
            }
            else if (TempData.ContainsKey("badMessage")) {
                ViewBag.BadMsg = TempData["badMessage"];
            }

                return View();
        }

        public IActionResult Show(int id) {
            Product? product = db.Products.Include(p => p.Category).Include(p => p.Collaborator).
                Include(p => p.Reviews).ThenInclude(r => r.User).
                Where(p => p.Id == id).FirstOrDefault();

            if (product is null) {
                TempData["badMessage"] = "Articolul cu id-ul " + id + " nu exista!";
                return RedirectToAction("Index");
            }

            ViewBag.Score = product.CalculateScore();
            return View(product);
        }

        [Authorize(Roles = "Colaborator, Admin")]
        [HttpGet]
        public IActionResult New() {
            Product product = new Product();

            product.Categ = GetAllCategories();

            return View(product);
        }

        [Authorize(Roles = "Colaborator, Admin")]
        [HttpPost]
        public IActionResult New(Product product) {

            product.CollaboratorId = _userManager.GetUserId(User);

            if (ModelState.IsValid) {
                db.Add(product);
                db.SaveChanges();
                TempData["goodMessage"] = "Produsul a fost adaugat cu succes!";
                return RedirectToAction("Index");
            }
            else {
                product.Categ = GetAllCategories();
                return View(product);
            }
        }

        [Authorize(Roles = "Colaborator, Admin")]
        [HttpGet]
        public IActionResult Edit(int id) {
            Product? product = db.Products.Include(p => p.Category).Where(p => p.Id == id).FirstOrDefault();

            if (product is null) {
                TempData["badMessage"] = "Articolul cu id-ul " + id + " nu exista!";
                return RedirectToAction("Index");
            }

            if (product.CollaboratorId != _userManager.GetUserId(User) && !User.IsInRole("Admin")) {
                TempData["badMessage"] = "Nu poti edita un produs care nu iti apartine!";
                return RedirectToAction("Index");
            }

            product.Categ = GetAllCategories();

            return View(product);
        }

        [Authorize(Roles = "Colaborator, Admin")]
        [HttpPost]
        public IActionResult Edit(int id, Product requestProduct) {
            Product? originalProduct = db.Products.Find(id);

            if (originalProduct is null) {
                TempData["badMessage"] = "Articolul cu id-ul " + id + " nu exista!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid) {
                if (User.IsInRole("Admin") || originalProduct.CollaboratorId == _userManager.GetUserId(User)) {
                    originalProduct.Title = requestProduct.Title;
                    originalProduct.Price = requestProduct.Price;
                    originalProduct.Stock = requestProduct.Stock;
                    originalProduct.CategoryId = requestProduct.CategoryId;
                    originalProduct.Description = requestProduct.Description;
                    TempData["goodMessage"] = "Produsul a fost modificat cu succes!";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else {
                    TempData["badMessage"] = "Nu poti edita un produs care nu iti apartine!";
                    return RedirectToAction("Index");
                }
            }
            else {
                originalProduct.Categ = GetAllCategories();
                return View(originalProduct);
            }
            
        }
        [HttpPost]
        public IActionResult Delete(int id) {
            Product? product = db.Products.Include(p => p.Reviews).Where(p => p.Id == id).FirstOrDefault();

            if (product is null) {
                TempData["badMessage"] = "Articolul cu id-ul " + id + " nu exista!";
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Admin") || product.CollaboratorId == _userManager.GetUserId(User)) {
                db.Remove(product);
                TempData["goodMessage"] = "Produsul a fost sters cu succes!";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            TempData["badMessage"] = "Nu poti sterge un produs care nu iti apartine!";
            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories() {
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories) {
                
                selectList.Add(new SelectListItem {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName
                });
            }

            return selectList;
        }
    }
}
