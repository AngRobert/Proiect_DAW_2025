using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_DAW_2025.Models {
    public class Order {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}