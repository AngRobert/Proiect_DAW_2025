using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;
using Proiect_DAW_2025.Models;
using System.Threading.Tasks;

namespace Proiect_DAW_2025.Controllers {
    public class ProductsController : Controller {

        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IWebHostEnvironment env) {
            db = context;
            _env = env;
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
            Product? product = db.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.Collaborator)
                                 .Include(p => p.Reviews)
                                    .ThenInclude(r => r.User)
                                 .Where(p => p.Id == id)
                                 .FirstOrDefault();

            if (product is null) {
                TempData["badMessage"] = "Produsul nu există!";
                return RedirectToAction("Index");
            }

            ViewBag.Score = product.CalculateScore();
            return View(product);
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpGet]
        public IActionResult New() {
            Product product = new Product();

            product.Categ = GetAllCategories();

            return View(product);
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public async Task<IActionResult> New(Product product, IFormFile Image) {
            product.CollaboratorId = _userManager.GetUserId(User);

            if (Image != null && Image.Length > 0) {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var fileExtension = Path.GetExtension(Image.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension)) {
                    ModelState.AddModelError(
                        "Image",
                        "Fișierul trebuie să fie o imagine (.jpg, .jpeg, .png, .webp)"
                    );

                    product.Categ = GetAllCategories();
                    return View(product);
                }

                var fileName = Guid.NewGuid() + fileExtension;

                var storagePath = Path.Combine(_env.WebRootPath, "Images", fileName);
                var databaseFileName = "/Images/" + fileName;

                using (var fileStream = new FileStream(storagePath, FileMode.Create)) {
                    await Image.CopyToAsync(fileStream);
                }

                product.Image = databaseFileName;
                ModelState.Remove(nameof(product.Image));
            }

            if (!TryValidateModel(product)) {
                product.Categ = GetAllCategories();
                return View(product);
            }

            db.Products.Add(product);
            db.SaveChanges();

            TempData["goodMessage"] = "Produsul a fost adăugat cu succes!";
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Colaborator,Admin")]
        [HttpGet]
        public IActionResult Edit(int id) {
            Product? product = db.Products
                                 .Include(p => p.Category)
                                 .Where(p => p.Id == id)
                                 .FirstOrDefault();

            if (product is null) {
                TempData["badMessage"] = "Produsul nu există!";
                return RedirectToAction("Index");
            }

            if (product.CollaboratorId != _userManager.GetUserId(User) && !User.IsInRole("Admin")) {
                TempData["badMessage"] = "Nu poți edita un produs care nu îți aparține!";
                return RedirectToAction("Index");
            }

            product.Categ = GetAllCategories();

            return View(product);
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product requestProduct, IFormFile Image) {
            var originalProduct = db.Products.Find(id);

            if (originalProduct == null) {
                TempData["badMessage"] = "Produsul nu există!";
                return RedirectToAction("Index");
            }

            if (!User.IsInRole("Admin") &&
                originalProduct.CollaboratorId != _userManager.GetUserId(User)) {
                TempData["badMessage"] = "Nu poți edita un produs care nu îți aparține!";
                return RedirectToAction("Index");
            }

            originalProduct.Title = requestProduct.Title;
            originalProduct.Price = requestProduct.Price;
            originalProduct.Stock = requestProduct.Stock;
            originalProduct.CategoryId = requestProduct.CategoryId;
            originalProduct.Description = requestProduct.Description;

            if (Image != null && Image.Length > 0) {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var fileExtension = Path.GetExtension(Image.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension)) {
                    ModelState.AddModelError("Image",
                        "Fișierul trebuie să fie o imagine (.jpg, .jpeg, .png, .webp)");

                    requestProduct.Categ = GetAllCategories();
                    return View(requestProduct);
                }

                if (!string.IsNullOrEmpty(originalProduct.Image)) {
                    var oldPath = Path.Combine(
                        _env.WebRootPath,
                        originalProduct.Image.TrimStart('/')
                    );

                    if (System.IO.File.Exists(oldPath)) {
                        System.IO.File.Delete(oldPath);
                    }
                }

                var fileName = Guid.NewGuid() + fileExtension;
                var storagePath = Path.Combine(_env.WebRootPath, "Images", fileName);
                var databaseFileName = "/Images/" + fileName;

                using var fileStream = new FileStream(storagePath, FileMode.Create);
                await Image.CopyToAsync(fileStream);

                originalProduct.Image = databaseFileName;
                ModelState.Remove(nameof(originalProduct.Image));   
            }

            if (!TryValidateModel(originalProduct)) {
                requestProduct.Categ = GetAllCategories();
                return View(requestProduct);
            }

            db.SaveChanges();
            TempData["goodMessage"] = "Produsul a fost modificat cu succes!";
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public IActionResult Delete(int id) {
            Product? product = db.Products
                                 .Include(p => p.Reviews)
                                 .Where(p => p.Id == id)
                                 .FirstOrDefault();

            if (product is null) {
                TempData["badMessage"] = "Produsul nu există!";
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Admin") || product.CollaboratorId == _userManager.GetUserId(User)) {
                db.Products.Remove(product);
                TempData["goodMessage"] = "Produsul a fost șters cu succes!";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            TempData["badMessage"] = "Nu poți șterge un produs care nu îți aparține!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        [HttpPost]
        public IActionResult Show([FromForm] Review review)
        {
            review.Date = DateTime.Now;

            review.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                return Redirect("/Products/Show/" + review.ProductId);
            }
            else
            {
                Product? product = db.Products
                                .Include(a => a.Category)
                                .Include(a => a.Reviews)
                                   .ThenInclude(c => c.User)
                                 .Include(a => a.Collaborator)
                                .Where(product => product.Id == review.ProductId)
                                .FirstOrDefault();

                if (product is null)
                {
                    TempData["badMessage"] = "Produsul nu există!";
                    return RedirectToAction("Index");
                }

                return View(product);
            }

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
