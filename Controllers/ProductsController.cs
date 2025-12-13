using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            
            return View();
        }

        public IActionResult Show(int id) {
            Product? product = db.Products.Include(p => p.Category).Include(p => p.Reviews).ThenInclude(r => r.User).
                Where(p => p.Id == id).FirstOrDefault();

            ViewBag.Score = product.CalculateScore();

            return View(product);
        }

        [Authorize(Roles = "Colaborator, Admin")]
        [HttpGet]
        public IActionResult New() {
            return View();
        }

        [Authorize(Roles = "Colaborator, Admin")]
        [HttpPost]
        public IActionResult New(Product product) {
            return RedirectToRoute("Index");
        }

        [Authorize(Roles = "Colaborator, Admin")]
        [HttpGet]
        public IActionResult Edit() {
            return View();
        }

        [Authorize(Roles = "Colaborator, Admin")]
        [HttpPost]
        public IActionResult Edit(string id, Product requestProduct) {
            return RedirectToAction("Show", "Products", new {id = id});
        }

        public IActionResult Delete() {
            return RedirectToAction("Index");
        }
    }
}
