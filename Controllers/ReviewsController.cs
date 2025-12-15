using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect_DAW_2025.Data;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Controllers
{
    public class ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser>
        userManager, RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly ApplicationDbContext db = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Delete(int id)
        {
            Review? rev = db.Reviews.Find(id);

            if (rev is null)
            {
                return NotFound();
            }

            else
            {
                if (rev.UserId == _userManager.GetUserId(User)
                        || User.IsInRole("Admin"))
                {
                    db.Reviews.Remove(rev);
                    db.SaveChanges();
                    return Redirect("/Products/Show/" + rev.ProductId);
                }

                else
                {
                    TempData["message"] = "Nu aveți dreptul să ștergeți recenzia";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Products");
                }
            }

        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Edit(int id)
        {
            Review? rev = db.Reviews.Find(id);

            if (rev is null)
            {
                return NotFound();
            }

            else
            {
                if (rev.UserId == _userManager.GetUserId(User)
                        || User.IsInRole("Admin"))
                {
                    return View(rev);
                }

                else
                {
                    TempData["message"] = "Nu aveți dreptul să editați recenzia";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Products");
                }
            }


        }

        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Edit(int id, Review requestReview)
        {
            Review? rev = db.Reviews.Find(id);

            if (rev is null)
            {
                return NotFound();
            }

            else
            {
                if (rev.UserId == _userManager.GetUserId(User)
                        || User.IsInRole("Admin"))
                {
                    if (ModelState.IsValid)
                    {

                        rev.Text = requestReview.Text;
                        rev.Rating = requestReview.Rating;

                        db.SaveChanges();

                        return Redirect("/Products/Show/" + rev.ProductId);
                    }

                    else
                    {
                        return View(requestReview);
                    }
                }

                else
                {
                    TempData["message"] = "Nu aveți dreptul să editați recenzia";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index", "Products");
                }
            }
        }
    }
}
