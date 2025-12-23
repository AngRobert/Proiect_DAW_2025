using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var wishlistItems = await db.WishlistItems
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(wishlistItems);
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Add(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["infoMessage"] = "Pentru a continua, autentifică-te sau creează un cont";
                TempData["messageType"] = "alert-info";

                return Redirect("/Identity/Account/Login?ReturnUrl=/Wishlist/Add/" + id);
            }

            var userId = _userManager.GetUserId(User);

            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                TempData["badMessage"] = "Produsul nu există!";
                return RedirectToAction("Index", "Products");
            }

            var exists = await db.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == id);

            if (exists)
            {
                TempData["badMessage"] = "Produsul este deja în wishlist!";
            }
            else
            {
                var wishlistItem = new WishlistItem
                {
                    ProductId = id,
                    UserId = userId
                };

                db.WishlistItems.Add(wishlistItem);
                await db.SaveChangesAsync();

                TempData["goodMessage"] = "Produsul a fost adăugat în wishlist!";
            }

            return RedirectToAction("Show", "Products", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            var item = await db.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item != null)
            {
                db.WishlistItems.Remove(item);
                await db.SaveChangesAsync();
                TempData["message"] = "Produsul a fost șters din wishlist!";
                TempData["messageType"] = "alert-success";
            }
            else
            {
                TempData["message"] = "Produsul nu a fost găsit sau nu îți aparține.";
                TempData["messageType"] = "alert-danger";
            }

            return RedirectToAction("Index");
        }
    }
}