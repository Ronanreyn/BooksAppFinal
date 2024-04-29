using BooksApp.Data;
using BooksApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class CategoryController : Controller
    {
        private BooksDbContext _context; //BooksDbContext is session
        public CategoryController(BooksDbContext dbContext)
        {
            _context = dbContext;
        }
        public IActionResult Index()// list or fetch all objects
        {
            var listOfCategories = _context.Categories.ToList();
            return View(listOfCategories);
        }

        [HttpGet] //gets us the empty form
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category categoryobj)
        {
            //custom validation 
            if (categoryobj.Name != null && categoryobj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("Name", "Category name cannot be test");
            }

            if (categoryobj.Name == categoryobj.Description)
            {
                ModelState.AddModelError("Description", "Category name and description cannot be the same");
            }
            if (ModelState.IsValid)
            {
                _context.Categories.Add(categoryobj);
                _context.SaveChanges();
                return RedirectToAction("Index", "Category");
            }

            return View(categoryobj);

        }
        [HttpGet] //gets us the empty form
        public IActionResult Edit(int id)
        {
            Category myCategory = _context.Categories.Find(id);
            return View(myCategory);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("ID, Name, Description")] Category categoryobj)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(categoryobj);
                _context.SaveChanges();
                return RedirectToAction("Index", "Category");
            }

            return View(categoryobj);

        }
        [HttpGet] //gets us the empty form
        public IActionResult Delete(int id)
        {
            Category myCategory = _context.Categories.Find(id);
            return View(myCategory);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePOST(int id)
        {
            Category categoryObj = _context.Categories.Find(id);
            _context.Categories.Remove(categoryObj);
            _context.SaveChanges();
            return RedirectToAction("Index", "Category");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Category myCategory = _context.Categories.Find(id);
            return View(myCategory);
        }

    }
}
