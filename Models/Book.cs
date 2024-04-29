using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksApp.Models
{
    public class Book
    {
        public int BookID { get; set; }

        [DisplayName("Book Title")]
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImgUrl { get; set; }
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category? category { get; set; } //navigational property
    }
}
