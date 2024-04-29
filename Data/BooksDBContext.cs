using BooksApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BooksApp.Data
{

    public class BooksDbContext : IdentityDbContext<IdentityUser>
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
        {

        }
        //add the book table to the database using a DbSet object.
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Book> Books { get; set; }//DbSet (table) of books. allows us to add a table to the database called books
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().HasData(    // model builder talks to the book entity and hasdata adds seed data to entity 

                new Book
                {
                    BookID = 1,
                    Title = "C# for Beginners",
                    Description = "Learn C#",
                    Author="Salman Nazir",
                    Price=19.99m,
                    CategoryID=3,
                    ImgUrl=""
                },
                new Book
                {
                    BookID = 2,
                    Title = "Advanced C#",
                    Description = "This is useful",
                    Author = "Salman Nazir",
                    Price = 19.99m,
                    CategoryID = 3,
                    ImgUrl = ""

                },
                new Book
                {
                    BookID = 3,
                    Title = "HTML for Beginners",
                    Description = "This is useful",
                    Author = "Salman Nazir",
                    Price = 19.99m,
                    CategoryID = 3,
                    ImgUrl = ""
                }

                );
            modelBuilder.Entity<Category>().HasData(    // model builder talks to the book entity and hasdata adds seed data to entity 

                    new Category { ID = 1, Name = "Travel", Description = "How to Travel" },
                    new Category { ID = 2, Name = "Sci-Fi", Description = "Fictional Science" },
                    new Category { ID = 3, Name = "Technology", Description = "Technology" }
                    );
        }
    }
}
