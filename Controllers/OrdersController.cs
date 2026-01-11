using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Controllers {

    [Authorize]
    public class OrdersController : Controller {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            db = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index() {
            var userId = _userManager.GetUserId(User);

            var orders = await db.Orders.Where(o => o.UserId == userId)
                .Include(o => o.OrderItems).OrderByDescending(o => o.Date).ToListAsync();

            if (TempData.ContainsKey("message")) {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(orders);
        }

        public async Task<IActionResult> Add() {
            var userId = _userManager.GetUserId(User);
            var items = await db.ShoppingCartItems.Where(c => c.UserId == userId)
                .Include(c => c.Product).ToListAsync();

            if (!items.Any()) {
                TempData["message"] = "Coșul este invalid!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Cart");
            }

            decimal total = 0;
            var stockErrors = new List<string>();

            foreach (var item in items) {
                if (item.Product.Stock < item.Quantity) {
                    stockErrors.Add("Produsul cu titlul " + item.Product.Title + " nu mai are decât " +
                        item.Product.Stock + " stoc! Ajustați cantitatea corespunzător!");
                }
            }

            if (stockErrors.Any()) {
                TempData["message"] = string.Join("<br />", stockErrors);
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Cart");
            }

            foreach (var item in items) {
                total = total + ((item.Product.Price ?? 0) * item.Quantity);
            }

            var order = new Order { 
                UserId = userId,
                Date = DateTime.Now,
                TotalAmount = total
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();
            
            foreach (var item in items) {
                var orderItem = new OrderItem {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Title,
                    Quantity = item.Quantity
                };

                item.Product.Stock -= item.Quantity;

                db.OrderItems.Add(orderItem);
            }

            db.ShoppingCartItems.RemoveRange(items);

            await db.SaveChangesAsync();

            TempData["message"] = "Comanda a fost plasată cu succes!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }
    }
}
