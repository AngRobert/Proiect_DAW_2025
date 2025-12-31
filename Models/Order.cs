using System.ComponentModel.DataAnnotations;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Models {
    public class Order {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
