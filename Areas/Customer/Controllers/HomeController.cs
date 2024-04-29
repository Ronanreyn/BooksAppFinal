using BooksApp.Data;
using BooksApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace BooksApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private BooksDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, BooksDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var listBooks = _context.Books.Include(c => c.category);
            return View(listBooks.ToList());
        }
        public IActionResult Details(int id)
        {
            Book book = _context.Books.Find(id); // fetch the book
            _context.Books.Entry(book).Reference(b => b.category).Load(); //load the category information

            var cart = new Cart
            {
                BookId = id, Book = book, Quantity = 1
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(Cart cart)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            cart.UserId = userId;
            Cart existingCart = _context.Carts.FirstOrDefault(c => c.UserId == userId && c.BookId == cart.BookId);
            if (existingCart != null)
            {
                existingCart.Quantity += cart.Quantity;
                _context.Carts.Update(existingCart);
            }
            else
            {
                _context.Carts.Add(cart);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}