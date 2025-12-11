using System.ComponentModel.DataAnnotations;

namespace Proiect_DAW_2025.Models
{
    public class WishlistItem
    {
        [Key]
        public int Id { get; set; }

        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

    }
}
