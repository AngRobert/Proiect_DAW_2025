using System.ComponentModel.DataAnnotations;

namespace Proiect_DAW_2025.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cantitatea este obligatorie")]
        [Range(1, int.MaxValue, ErrorMessage = "Cantitatea trebuie să fie cel puțin 1")]
        public int Quantity { get; set; }

        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}
