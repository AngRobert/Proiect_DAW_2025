using System.ComponentModel.DataAnnotations;

namespace Proiect_DAW_2025.Models
{
    public class FAQ
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Textul întrebării este obligatoriu")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Răspunsul este obligatoriu")]
        public string Answer { get; set; }

        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }


    }
}
