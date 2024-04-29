using BooksApp.Data;
using BooksApp.Models;
using BooksApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Permissions;

namespace BooksApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class OrderController : Controller
    {
        private BooksDbContext _context;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(BooksDbContext dbContext)
        {
            _context = dbContext;
        }
        public IActionResult Index()
        {
            IEnumerable<Order> listOfOrders = _context.Orders.Include(o => o.ApplicationUser);
            return View(listOfOrders);
        }
        public IActionResult Details(int id)
        {
            Order order = _context.Orders.Find(id);
            _context.Entry(order).Reference(o => o.ApplicationUser).Load();
            IEnumerable<OrderDetail> orderDetails = _context.OrderDetails.Where(od => od.OrderId == id).Include(od => od.Book);
            OrderVM orderVM = new OrderVM
            {
                Order = order, OrderDetails = orderDetails
            };
            return View(orderVM);
        }

        [HttpPost]
        public IActionResult UpdateOrderInformation()
        {
           Order orderFromDB = _context.Orders.Find(orderVM.Order.OrderId);
            orderFromDB.CustomerName = orderVM.Order.CustomerName; //puts in value from view into the database
            orderFromDB.StreetAddress = orderVM.Order.StreetAddress;
            orderFromDB.City = orderVM.Order.City;
            orderFromDB.State = orderVM.Order.State;
            orderFromDB.PostalCode = orderVM.Order.PostalCode;
            orderFromDB.Phone = orderVM.Order.Phone;
            //if(!string.IsNullOrEmpty(orderVM.Order.ShippingDate.ToString()))
            //{
                orderFromDB.ShippingDate = orderVM.Order.ShippingDate;
            //}
            //if(!string.IsNullOrEmpty(orderVM.Order.TrackingNumber))
            //{
                orderFromDB.TrackingNumber = orderVM.Order.TrackingNumber;
            //}
            //if(!string.IsNullOrEmpty(orderVM.Order.Carrier))
            //{
                orderFromDB.Carrier = orderVM.Order.Carrier;
            //}
            orderFromDB.OrderStatus = orderVM.Order.OrderStatus;
            _context.Orders.Update(orderFromDB);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = orderFromDB.OrderId });
        }
        public IActionResult ProcessOrder()
        {
           Order order = _context.Orders.Find(orderVM.Order.OrderId);
            order.OrderStatus = "Processing";
            order.ShippingDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);
            order.Carrier = "USPS";
            _context.Orders.Update(order);
            _context.SaveChanges();
            return RedirectToAction("Details", new {id = order.OrderId});
        }
        public IActionResult CompleteOrder()
        {
            Order order = _context.Orders.Find(orderVM.Order.OrderId);
            order.OrderStatus = "Shipped and Completed";
            order.ShippingDate = DateOnly.FromDateTime(DateTime.Now);
            _context.Orders.Update(order);
            _context.SaveChanges();
            return RedirectToAction("Details", new {id = order.OrderId});
        }
    }
}
