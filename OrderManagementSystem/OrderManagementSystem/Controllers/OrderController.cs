using Microsoft.AspNetCore.Mvc;
using OrderManagementSystemDAL.Data;
using OrderManagementSystemDAL.Models;
using System.Diagnostics;

namespace OrderManagementSystem.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<order> orders = new List<order>();
            OrderRepository orderRepository = new OrderRepository();
            orders = orderRepository.GetAllOrders();

            return View(orders);
        }
        public ActionResult EditOrder(int id)
        {
            order orders = new order();
            OrderRepository orderRepository = new OrderRepository();
            orders = orderRepository.GetOrderById(id);

            return View(orders);
        }
        public ActionResult EditOrderDetails(int id, order orderDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    OrderRepository _DbOrder = new OrderRepository();
                    if (_DbOrder.EditOrderDetails(id, orderDetails))
                    {
                        return RedirectToAction("Index");
                    }
                }
                return View();
            }
            catch
            {
                return View();
            }
        }
        public ActionResult AddOrder()
        {
            return View();
        }
        public ActionResult DeleteOrder(int Id)
        {
            try
            {
                OrderRepository _DbOrder = new OrderRepository();

                if (_DbOrder.DeleteOrderDetails(Id))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete order.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }


        public ActionResult AddNewOrder(order orderDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    OrderRepository _DbOrder = new OrderRepository();
                    if (_DbOrder.Addorder(orderDetails))
                    {
                        return RedirectToAction("Index");
                    }
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
