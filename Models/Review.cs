using System.ComponentModel.DataAnnotations;

namespace Proiect_DAW_2025.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public string? Text { get; set; }

        [Range(1, 5, ErrorMessage = "Ratingul trebuie să fie între 1 și 5")]
        public int? Rating { get; set; }

        public DateTime Date { get; set; }

        public int ProductId { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual Product? Product { get; set; }
    }
}
