using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Controllers {
    public class ProductsController : Controller {

        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IWebHostEnvironment env) 
        {
            db = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index() 
        {
            IQueryable<Product> query = db.Products
                                          .Include(p => p.Category)
                                          .Include(p => p.Reviews);

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                query = query.Where(p => p.Title.Contains(search) ||
                                         p.Category.CategoryName.Contains(search)); ;
            }

            ViewBag.SearchString = search;

            var status = Convert.ToString(HttpContext.Request.Query["status"]);
            var isAdmin = User.IsInRole("Admin");
            var isColaborator = User.IsInRole("Colaborator");

            if (!isAdmin && !isColaborator) {
                query = query.Where(p => p.Status == "aprobat");
            }

            if (isColaborator && !isAdmin) {
                query = query.Where(p =>
                    p.Status == "aprobat" ||
                    p.CollaboratorId == _userManager.GetUserId(User));
            }

            ViewBag.Status = status;

            if (string.IsNullOrEmpty(status)) {
                query = query.Where(p => p.Status == "aprobat");
            }
            else if (isAdmin || isColaborator) {
                query = query.Where(p => p.Status == status);
            }

            List<Product> products = query.ToList();

            foreach (var product in products)
            {
                product.Rating = product.CalculateScore();
            }

            var sort = Convert.ToString(HttpContext.Request.Query["sort"]);

            ViewBag.Sort = sort;

            switch (sort)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "rating_asc":
                    products = products.OrderBy(p => p.Rating).ToList();
                    break;
                case "rating_desc":
                    products = products.OrderByDescending(p => p.Rating).ToList();
                    break;
                default:
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
            }
       
            if (TempData.ContainsKey("message")) {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            int _perPage = 8;
            int totalItems = products.Count();
            int currentPage = 1;

            if (int.TryParse(HttpContext.Request.Query["page"], out int page)) {
                currentPage = page;
            }

            var offset = 0;

            if (currentPage > 1) {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedProducts = products.Skip(offset).Take(_perPage).ToList();

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            ViewBag.currentPage = currentPage;
            ViewBag.Products = paginatedProducts;

            return View();
        }

        public IActionResult Show(int id) 
        {
            if (TempData.ContainsKey("DraftReviewText"))
            {
                ViewBag.DraftText = TempData["DraftReviewText"];
                ViewBag.DraftRating = TempData["DraftReviewRating"];
            }

            Product? product = db.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.Collaborator)
                                 .Include(p => p.Reviews)
                                    .ThenInclude(r => r.User)
                                 .Where(p => p.Id == id)
                                 .FirstOrDefault();

            if (product is null) {
                TempData["message"] = "Produsul nu există!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            product.Rating = product.CalculateScore();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(product);
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpGet]
        public IActionResult New() 
        {
            Product product = new Product();

            product.Categ = GetAllCategories();

            return View(product);
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public async Task<IActionResult> New(Product product, IFormFile? ImageFile) {
            product.CollaboratorId = _userManager.GetUserId(User);

            if (ImageFile != null && ImageFile.Length > 0) {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();

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
                    await ImageFile.CopyToAsync(fileStream);
                }
               

                product.Image = databaseFileName;
            }
            else {
                ModelState.AddModelError("Image",
                    "Imaginea este obligatorie");
            }

            if (User.IsInRole("Admin")) {
                product.Status = "aprobat";
            }
            else {
                if (User.IsInRole("Colaborator")) {
                    product.Status = "asteptare";
                }
            }

            ModelState.Remove(nameof(product.Status));

            if (!TryValidateModel(product)) {
                product.Categ = GetAllCategories();
                return View(product);
            }

            db.Products.Add(product);
            db.SaveChanges();

            TempData["message"] = "Produsul a fost adăugat cu succes!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Colaborator,Admin")]
        [HttpGet]
        public IActionResult Edit(int id) 
        {
            Product? product = db.Products
                                 .Include(p => p.Category)
                                 .Where(p => p.Id == id)
                                 .FirstOrDefault();

            if (product is null) {
                TempData["message"] = "Produsul nu există!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            if (product.CollaboratorId != _userManager.GetUserId(User) && !User.IsInRole("Admin")) {
                TempData["message"] = "Nu poți edita un produs care nu îți aparține!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            product.Categ = GetAllCategories();

            return View(product);
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product requestProduct, IFormFile? Image) {
            var originalProduct = db.Products.Find(id);

            if (originalProduct == null) {
                TempData["message"] = "Produsul nu există!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            if (!User.IsInRole("Admin") &&
                originalProduct.CollaboratorId != _userManager.GetUserId(User)) {
                TempData["message"] = "Nu poți edita un produs care nu îți aparține!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            originalProduct.Title = requestProduct.Title;
            originalProduct.Price = requestProduct.Price;
            originalProduct.Stock = requestProduct.Stock;
            originalProduct.CategoryId = requestProduct.CategoryId;
            originalProduct.Description = requestProduct.Description;

            if (User.IsInRole("Admin")) {
                originalProduct.Status = "aprobat";
            }
            else {
                if (User.IsInRole("Colaborator")) {
                    originalProduct.Status = "asteptare";
                }
            }

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
            TempData["message"] = "Produsul a fost modificat cu succes!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public IActionResult Delete(int id) 
        {
            Product? product = db.Products
                                 .Include(p => p.Reviews)
                                 .Where(p => p.Id == id)
                                 .FirstOrDefault();

            if (product is null) {
                TempData["message"] = "Produsul nu există!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Admin") || product.CollaboratorId == _userManager.GetUserId(User)) {
                db.Products.Remove(product);
                TempData["message"] = "Produsul a fost șters cu succes!";
                TempData["messageType"] = "alert-success";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            TempData["message"] = "Nu poți șterge un produs care nu îți aparține!";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Show([FromForm] Review review)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["infoMessage"] = "Pentru a continua, autentifică-te sau creează un cont";
                TempData["messageType"] = "alert-info";

                TempData["DraftReviewText"] = review.Text;
                TempData["DraftReviewRating"] = review.Rating?.ToString();

                return Redirect("/Identity/Account/Login?ReturnUrl=/Products/Show/" + review.ProductId);
            }

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
                    TempData["message"] = "Produsul nu există!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }

                product.Rating = product.CalculateScore();

                ViewBag.DraftText = review.Text;
                ViewBag.DraftRating = review.Rating;

                return View(product);
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories() 
        {
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
