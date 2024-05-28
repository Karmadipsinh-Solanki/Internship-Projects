using Microsoft.AspNetCore.Mvc;
using OrderManagementSystemDAL.Data;
using OrderManagementSystemDAL.Models;
using System.Diagnostics;

namespace OrderManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<customer> customers = new List<customer>();
            CustomerRepository customerRepository = new CustomerRepository();
            customers = customerRepository.GetAllCustomers();

            return View(customers);
        }
        public ActionResult EditCustomer(int id)
        {
            customer customers = new customer();
            CustomerRepository customerRepository = new CustomerRepository();
            customers = customerRepository.GetCustomerById(id);

            return View(customers);
        }
        public ActionResult EditCustomerDetails(int id,customer customerDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerRepository _DbCustomer = new CustomerRepository();
                    if (_DbCustomer.EditCustomerDetails(id,customerDetails))
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
        public ActionResult AddCustomer()
        {
            return View();
        }
        public ActionResult DeleteCustomer(int Id)
        {
            try
            {
                CustomerRepository _DbCustomer = new CustomerRepository();

                if (_DbCustomer.DeleteCustomerDetails(Id))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete customer.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }


        public ActionResult AddNewCustomer(customer customerDetails) 
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerRepository _DbCustomer = new CustomerRepository();
                    if (_DbCustomer.AddCustomer(customerDetails))
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
