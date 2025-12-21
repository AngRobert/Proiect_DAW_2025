using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_DAW_2025.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Titlul trebuie să aibă mai mult de 5 caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Descrierea produsului este obligatorie")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Prețul este obligatoriu")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Prețul trebuie să fie mai mare ca 0")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stocul este obligatoriu")]
        [Range(0, int.MaxValue, ErrorMessage = "Stocul nu poate fi negativ")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int? CategoryId { get; set; }

        public string? Image { get; set; }

        public virtual Category? Category { get; set; }

        public string? CollaboratorId { get; set; }

        public virtual ApplicationUser? Collaborator { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<FAQ> FAQs { get; set; } = new List<FAQ>();

        [NotMapped]
        public double Rating = 0;


        public double CalculateScore() {
            if (Reviews == null || !Reviews.Any())
                return 0;

            double s = 0;
            int count = 0;

            foreach (var r in Reviews) {
                if (r.Rating != null) {
                    s += (double)r.Rating;
                    count++;
                }
            }

            if (count == 0) {
                return 0;
            }

            return Math.Round(s / count, 2);
        }


        [NotMapped]
        public IEnumerable<SelectListItem> Categ { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
