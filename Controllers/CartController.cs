using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Controllers {
    [Authorize]
    public class CartController : Controller {

        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IWebHostEnvironment env) {
            db = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index() {

            if (!User.Identity.IsAuthenticated) {
                TempData["infoMessage"] = "Pentru a continua, autentifică-te sau creează un cont";
                TempData["messageType"] = "alert-info";

                return Redirect("/Identity/Account/Login?ReturnUrl=/Cart/Index/");
            }
            var userId = _userManager.GetUserId(User);

            var cartItems = await db.ShoppingCartItems.Where(c => c.UserId == userId).Include(c => c.Product).ToListAsync();

            if (TempData.ContainsKey("message")) {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(cartItems);
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Add(int id, string returnUrl) {
            if (!User.Identity.IsAuthenticated) {
                TempData["infoMessage"] = "Pentru a continua, autentifică-te sau creează un cont";
                TempData["messageType"] = "alert-info";

                return Redirect("/Identity/Account/Login?ReturnUrl=/Cart/Add/" + id);
            }

            var userId = _userManager.GetUserId(User);

            var product = await db.Products.FindAsync(id);

            if (product == null) {
                TempData["message"] = "Produsul nu există!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }

            var exists = await db.ShoppingCartItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == id);

            if (exists) {
                var item = await db.ShoppingCartItems.Where(w => w.UserId == userId && w.ProductId == id).FirstOrDefaultAsync();
                if (product.Stock < item.Quantity + 1) {
                    TempData["message"] = "Stoc indisponibil!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Cart");
                }
                else {
                    TempData["message"] = "Produsul se afla deja in coș, cantitatea a fost mărită!";
                    TempData["messageType"] = "alert-success";
                    item.Quantity += 1;
                    db.SaveChanges();
                }
            }
            else {
                if (product.Stock > 0) {
                    var shoppingCartItem = new ShoppingCartItem {
                        ProductId = id,
                        UserId = userId,
                        Quantity = 1
                    };

                    db.ShoppingCartItems.Add(shoppingCartItem);
                    await db.SaveChangesAsync();

                    TempData["message"] = "Produsul a fost adăugat în coș!";
                    TempData["messageType"] = "alert-success";
                }
                else {
                    TempData["message"] = "Stoc indisponibil!";
                    TempData["messageType"] = "alert-danger";
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Show", "Products", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
            var userId = _userManager.GetUserId(User);

            var item = await db.ShoppingCartItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item != null) {
                db.ShoppingCartItems.Remove(item);
                await db.SaveChangesAsync();
                TempData["message"] = "Produsul a fost șters din coș!";
                TempData["messageType"] = "alert-success";
            }
            else {
                TempData["message"] = "Produsul nu a fost găsit sau nu îți aparține.";
                TempData["messageType"] = "alert-danger";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Increase(int id) {
            var userId = _userManager.GetUserId(User);

            var item = await db.ShoppingCartItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (item == null) {
                TempData["message"] = "Produsul nu exista!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            if (item.Quantity + 1 > item.Product.Stock) {
                TempData["message"] = "Stoc insuficient!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            item.Quantity++;
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Decrease(int id) {
            var userId = _userManager.GetUserId(User);

            var item = await db.ShoppingCartItems
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (item == null) {
                TempData["message"] = "Produsul nu exista!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            if (item.Quantity > 1) {
                item.Quantity--;
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }


    }
}
