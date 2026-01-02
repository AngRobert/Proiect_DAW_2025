using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Proiect_DAW_2025.Data;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Controllers {
    public class UsersController : Controller {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index() {
            var users = await db.ApplicationUsers.OrderByDescending(u => u.UserName).ToListAsync();

            int _perPage = 20;

            int totalUsers = users.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            if (currentPage > 0) {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedUsers = users.Skip(offset).Take(_perPage);

            ViewBag.LastPage = Math.Ceiling((float)totalUsers / (float)_perPage);
            ViewBag.currentPage = currentPage;
            ViewBag.Users = paginatedUsers;
            ViewBag.Roles = GetAllRoles();

            var userRoles = new Dictionary<string, string>();

            foreach (var user in users) {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault();
            }

            ViewBag.UserRoles = userRoles;

            if (TempData.ContainsKey("message")) {
                ViewBag.Msg = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles() {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                             select role;

            foreach (var role in roles) {

                selectList.Add(new SelectListItem {
                    Value = role.Name,
                    Text = role.Name
                });
            }

            return selectList;
        }
    }
}
