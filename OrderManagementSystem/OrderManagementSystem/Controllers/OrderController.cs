using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OrderManagementSystemBLL.Repository;
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
            OrderRepository _DbOrder = new OrderRepository();

            // Retrieve the order by its ID
            order order = _DbOrder.GetOrderById(id);

            if (order == null)
            {
                // Handle the case where the order with the specified ID was not found
                return NotFound();
            }

            // Populate the customer property with necessary data
            CustomerRepository _DbCustomer = new CustomerRepository();
            order.customer = _DbCustomer.GetAllCustomers();

            return View(order);
        }

        public ActionResult EditOrderDetails(order orderDetails)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    OrderRepository _DbOrder = new OrderRepository();
                    if (_DbOrder.EditOrderDetails(orderDetails.order_id, orderDetails))
                    {
                        return RedirectToAction("Index");
                    }
                //}
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult AddOrder()
        {
            OrderRepository _DbOrder = new OrderRepository();
            CustomerRepository _DbCustomer = new CustomerRepository();
            // Retrieve the list of customers from the repository
            var customers = _DbCustomer.GetAllCustomers();

            order order = new order
            {
                customer = customers
            };


            //var orders = _DbOrder.GetAllOrders();
            //return View(orders);
            return View(order);
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


        //public ActionResult AddNewOrder(order orderDetails)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            OrderRepository _DbOrder = new OrderRepository();
        //            if (_DbOrder.AddOrder(orderDetails))
        //            {
        //                return RedirectToAction("Index");
        //            }
        //        }
        //        return View();
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        [HttpPost]
        public ActionResult DeleteSelectedOrder(string orderids)
        {
            try
            {
                OrderRepository _DbOrder = new OrderRepository();

                if (_DbOrder.DeleteSelectedOrderDetails(orderids))
                {
                    return Json(new { isDeleted = true });
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete customer.";
                    return Json(new { isDeleted = false });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return Json(new { isDeleted = false });
            }
        }
        [HttpPost]
        public ActionResult AddNewOrder(order orderDetails)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                OrderRepository _DbOrder = new OrderRepository();
                if (_DbOrder.AddOrder(orderDetails))
                {
                    return RedirectToAction("Index");
                }
                //}
                return RedirectToAction("Index");
                //return View();
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
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
