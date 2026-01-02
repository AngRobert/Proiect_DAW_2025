using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_DAW_2025.Models {
    public class ApplicationUser : IdentityUser {
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}
