using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using OrderManagementSystemBLL.Repository;
using OrderManagementSystemDAL.Models;
using OrderManagementSystemDAL.Models.Dto;
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

        public IActionResult GetCustomerOrder()
        {
            List<CustomerOrder> customerOrders = new List<CustomerOrder>();
            CustomerOrderRepository customerOrderRepository = new CustomerOrderRepository();
            customerOrders = customerOrderRepository.GetAllCustomerOrders();

            return View(customerOrders);
        }
        public IActionResult Index()
        {
            List<customer> customers = new List<customer>();
            CustomerRepository customerRepository = new CustomerRepository();

            customers = customerRepository.GetAllCustomers();

            return View(customers);
        }
        
        public ActionResult AddNewCustomer(CustomerDTO customerDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerRepository _DbCustomer = new CustomerRepository();
                    string filePath = _DbCustomer.UploadFile(customerDetails.File);
                    customerDetails.profileImgPath = filePath;
                    if (_DbCustomer.AddCustomer(customerDetails))
                    {
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        

        public ActionResult EditCustomer(int id)
        {
            customer customers = new customer();
            CustomerRepository customerRepository = new CustomerRepository();
            customers = customerRepository.GetCustomerById(id);

            return View(customers);
        }
        public ActionResult EditCustomerDetails(CustomerByIdDTO customerDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerRepository _DbCustomer = new CustomerRepository();

                    if (customerDetails.File != null)
                    {
                        string filePath = _DbCustomer.UploadFile(customerDetails.File);
                        customerDetails.profileImgPath = filePath;
                    }

                    if (_DbCustomer.EditCustomerDetails(customerDetails))
                    {
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Index");
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
        [HttpPost]
        public ActionResult DeleteSelectedCustomer(string customerids)
        {
            try
            {
                CustomerRepository _DbCustomer = new CustomerRepository();

                if (_DbCustomer.DeleteSelectedCustomerDetails(customerids))
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


        //public IActionResult CustomerImage(int customerId)
        //{
        //    CustomerRepository customerRepository = new CustomerRepository();

        //    var imageData = customerRepository.GetCustomerImage(customerId);
        //    if (imageData != null && imageData.Length > 0)
        //    {
        //        return File(imageData, "image/jpeg"); // Assuming the image is JPEG format, adjust as needed
        //    }
        //    else
        //    {
        //        // Handle case when image data is not found
        //        return NotFound();
        //    }
        //}

        //public IActionResult UploadImage(customer customer)
        //{
        //    CustomerRepository _DbCustomer = new CustomerRepository();
        //    if (_DbCustomer.UploadFile(customer))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //        return RedirectToAction("Index");
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
