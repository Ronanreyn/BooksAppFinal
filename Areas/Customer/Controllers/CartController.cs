using BooksApp.Data;
using BooksApp.Models;
using BooksApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace BooksApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private BooksDbContext _context;
        public CartController(BooksDbContext dBcontext)
        {
            _context = dBcontext;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItemsList = _context.Carts.Where(c => c.UserId == userId).Include(c => c.Book);
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                CartItems = cartItemsList,
                Order = new Order()
            };
            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.SubTotal = cartItem.Book.Price * cartItem.Quantity;
                shoppingCartVM.Order.OrderTotal += cartItem.SubTotal;

            }
            return View(shoppingCartVM);
        }

        public IActionResult IncrementByOne(int id)
        {
            Cart cart = _context.Carts.Find(id);
            cart.Quantity++;
            _context.Update(cart);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult DecrementByOne(int id)
        {
            Cart cart = _context.Carts.Find(id);
            if (cart.Quantity <= 1)
            {
                _context.Carts.Remove(cart);
                _context.SaveChanges();

            }
            else
            {
                cart.Quantity--;
                _context.Update(cart);
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            Cart cart = _context.Carts.Find(id);
            _context.Carts.Remove(cart);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        public IActionResult ReviewOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItemsList = _context.Carts.Where(c => c.UserId == userId).Include(c => c.Book);
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                CartItems = cartItemsList,
                Order = new Order()
            };
            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.SubTotal = cartItem.Book.Price * cartItem.Quantity;
                shoppingCartVM.Order.OrderTotal += cartItem.SubTotal;

            }
            shoppingCartVM.Order.ApplicationUser = _context.ApplicationUsers.Find(userId);
            shoppingCartVM.Order.CustomerName = shoppingCartVM.Order.ApplicationUser.Name;
            shoppingCartVM.Order.StreetAddress = shoppingCartVM.Order.ApplicationUser.StreetAddress;
            shoppingCartVM.Order.City = shoppingCartVM.Order.ApplicationUser.City;
            shoppingCartVM.Order.State = shoppingCartVM.Order.ApplicationUser.State;
            shoppingCartVM.Order.PostalCode = shoppingCartVM.Order.ApplicationUser.PostalCode;
            shoppingCartVM.Order.Phone = shoppingCartVM.Order.ApplicationUser.PhoneNumber;


            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("ReviewOrder")]
        public IActionResult ReviewOrderPOST(ShoppingCartVM shoppingCartVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItemsList = _context.Carts.Where(c => c.UserId == userId).Include(c => c.Book);
            shoppingCartVM.CartItems = cartItemsList;
            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.SubTotal = cartItem.Book.Price * cartItem.Quantity;
                shoppingCartVM.Order.OrderTotal += cartItem.SubTotal;
            }
            shoppingCartVM.Order.ApplicationUser = _context.ApplicationUsers.Find(userId);
            shoppingCartVM.Order.CustomerName = shoppingCartVM.Order.ApplicationUser.Name;
            shoppingCartVM.Order.StreetAddress = shoppingCartVM.Order.ApplicationUser.StreetAddress;
            shoppingCartVM.Order.City = shoppingCartVM.Order.ApplicationUser.City;
            shoppingCartVM.Order.State = shoppingCartVM.Order.ApplicationUser.State;
            shoppingCartVM.Order.PostalCode = shoppingCartVM.Order.ApplicationUser.PostalCode;
            shoppingCartVM.Order.Phone = shoppingCartVM.Order.Phone;
            shoppingCartVM.Order.OrderDate = DateOnly.FromDateTime(DateTime.Now);
            shoppingCartVM.Order.OrderStatus = "Pending";
            shoppingCartVM.Order.PaymentStatus = "Pending";

            _context.Orders.Add(shoppingCartVM.Order);
            _context.SaveChanges();

            foreach (var eachCartItem in shoppingCartVM.CartItems)
            {
                OrderDetail orderDetail = new()
                {
                    OrderId = shoppingCartVM.Order.OrderId,
                    BookId = eachCartItem.BookId,
                    Quantity = eachCartItem.Quantity,
                    Price = eachCartItem.Book.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/cart/orderconfirmation?id={shoppingCartVM.Order.OrderId}",
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var eachCartItem in shoppingCartVM.CartItems)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(eachCartItem.Book.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = eachCartItem.Book.Title
                        }
                    },
                    Quantity = eachCartItem.Quantity,
                };
                options.LineItems.Add(sessionLineItem);
            }


            var service = new Stripe.Checkout.SessionService();
            Session session = service.Create(options);
            shoppingCartVM.Order.SessionID = session.Id;
            _context.SaveChanges();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);



        }

        public IActionResult OrderConfirmation(int id)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Order order = _context.Orders.Find(id);
            var sessID = order.SessionID;
            var service = new SessionService();
            Session session = service.Get(sessID);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                order.PaymentIntentID = session.PaymentIntentId;
                order.PaymentStatus = "Approved";
            }

            List<Cart> listOfCartItems = _context.Carts.ToList().Where(c => c.UserId == userID).ToList();
            _context.Carts.RemoveRange(listOfCartItems);
            _context.SaveChanges();
            return View(id);
        }
    }
}
