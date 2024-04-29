using BooksApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private BooksDbContext _context;
        private UserManager<IdentityUser> _userManager;
        public UserController(BooksDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _context = dbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            List<ApplicationUser> userList = _context.ApplicationUsers.ToList();
            var allRoles = _context.Roles.ToList();

            var userRoles = _context.UserRoles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRoles.Find(ur => ur.UserId == user.Id).RoleId;//fetches roleId of current user
                var roleName = allRoles.Find(r => r.Id == roleId).Name;//fetches name of current user
                user.RoleName = roleName;
            }
            return View(userList);
        }
        public IActionResult LockUnlock(string id)
        {
            var userFromDb = _context.ApplicationUsers.Find(id);
            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                userFromDb.LockoutEnd = DateTime.Now;//if user account is locked, unlock
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(10);// if user account is unlocked, lock it
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditUserRole(string id)
        {
            var currentUserRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == id);
            IEnumerable<SelectListItem> listOfRoles = _context.Roles.ToList().Select(r => new SelectListItem { Text = r.Name, Value = r.Id.ToString() });
            ViewBag.ListOfRoles = listOfRoles;
            ViewBag.UserInfo = _context.ApplicationUsers.Find(id);
            return View(currentUserRole);
        }
        [HttpPost]
        public IActionResult EditUserRole(Microsoft.AspNetCore.Identity.IdentityUserRole<string> updatedRole)
        {
            ApplicationUser applicationUser = _context.ApplicationUsers.Find(updatedRole.UserId);
            string newRoleName = _context.Roles.Find(updatedRole.RoleId).Name;
            string oldRoleID = _context.UserRoles.FirstOrDefault(u => u.UserId == applicationUser.Id).RoleId;
            string oldRoleName = _context.Roles.Find(oldRoleID).Name;
            _userManager.RemoveFromRoleAsync(applicationUser, oldRoleName).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(applicationUser, newRoleName).GetAwaiter().GetResult();
            return RedirectToAction("Index");

        }
    }
}
