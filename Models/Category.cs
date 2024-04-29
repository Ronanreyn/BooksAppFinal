using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BooksApp.Models
{
    public class Category
    {
        public int ID { get; set; }

        [DisplayName("Category Name: ")]
        [Required(ErrorMessage = "You must provide a category in order to continue.")]
        public string Name { get; set; }

        [DisplayName("Category Description: "), Required(ErrorMessage = "A description must be provided"), MaxLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
        public string? Description { get; set; } 


    }
}
