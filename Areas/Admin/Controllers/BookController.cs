using BooksApp.Data;
using BooksApp.Models;
using BooksApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private BooksDbContext _context;

        private IWebHostEnvironment _environment;
        public BookController(BooksDbContext context, IWebHostEnvironment environment)
        {
            _context = context;             
            _environment = environment;

        }
        public IActionResult Index()
        {
            var listOfBooks = _context.Books.ToList();
            return View(listOfBooks);
        }
        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> listOfCategories = _context.Categories.ToList().Select(o => new SelectListItem { Text = o.Name, Value = o.ID.ToString() });

            BookWithCategoriesVM bookWithCategoriesVMobj = new BookWithCategoriesVM();
            bookWithCategoriesVMobj.Book = new Book();
            bookWithCategoriesVMobj.ListOfCategories = listOfCategories;
            return View(bookWithCategoriesVMobj);
        }

        [HttpPost]
        public IActionResult Create(BookWithCategoriesVM bookWithCategoriesVMObj, IFormFile imgFile)
        {
            if (ModelState.IsValid)
            {
                string wwwrootPath = _environment.WebRootPath;
                if (imgFile != null)
                {
                    using (var fileStream = new FileStream(Path.Combine(wwwrootPath, @"Images\BookImages\" + imgFile.FileName), FileMode.Create))
                    {
                        imgFile.CopyTo(fileStream);
                    }
                    bookWithCategoriesVMObj.Book.ImgUrl = @"\Images\BookImages\" + imgFile.FileName;
                }
                _context.Books.Add(bookWithCategoriesVMObj.Book);
                _context.SaveChanges();
                return RedirectToAction("Index", "Book");
            }
            return View(bookWithCategoriesVMObj);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(id);



            IEnumerable<SelectListItem> listOfCategories = _context.Categories.ToList().Select(o => new SelectListItem { Text = o.Name, Value = o.ID.ToString() });

            ViewData["ListOfCategoriesVD"] = listOfCategories;
            BookWithCategoriesVM booksWithCategoriesVM = new BookWithCategoriesVM();
            booksWithCategoriesVM.Book = book;
            booksWithCategoriesVM.ListOfCategories = listOfCategories;



            return View(booksWithCategoriesVM);
        }

        [HttpPost]
        public IActionResult Edit(BookWithCategoriesVM bookWithCategoriesVM, IFormFile? imgFile)
        {
            string wwwrootPath = _environment.WebRootPath;
            if (ModelState.IsValid)
            {
                if (imgFile != null) 
                {
                    if (!string.IsNullOrEmpty(bookWithCategoriesVM.Book.ImgUrl)) 
                    {
                        var oldImgPath = Path.Combine(wwwrootPath, bookWithCategoriesVM.Book.ImgUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    } 
                    using (var fileStream = new FileStream(Path.Combine(wwwrootPath, @"Images\BookImages\" + imgFile.FileName), FileMode.Create))
                    {
                        imgFile.CopyTo(fileStream);
                    }
                    bookWithCategoriesVM.Book.ImgUrl = @"\images\bookImages\" + imgFile.FileName;

                }
                _context.Books.Update(bookWithCategoriesVM.Book);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookWithCategoriesVM); 

        }

    }
}
