using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksApp.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        [ValidateNever]
        public Book Book { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]

        public ApplicationUser ApplicationUser { get; set; }
        public int Quantity { get; set; }
        [NotMapped]
        public decimal SubTotal { get; set; }

    }
}
