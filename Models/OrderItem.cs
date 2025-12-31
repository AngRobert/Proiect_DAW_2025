using System.ComponentModel.DataAnnotations;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Models {
    public class OrderItem {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

    }
}
