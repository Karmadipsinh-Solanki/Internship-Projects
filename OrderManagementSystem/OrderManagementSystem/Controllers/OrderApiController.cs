using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystemBLL.Interface;
using OrderManagementSystemBLL.Repository;
using OrderManagementSystemDAL.Models;
using OrderManagementSystemDAL.Models.Dto;
using System.Net;

namespace OrderManagementSystem.Controllers
{
    [Route("api/OrderApi")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;
        public OrderApiController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            this._response = new();
        }
        // GET: api/<HomeApiController>
        [HttpGet]
        [Authorize]
        public IActionResult GetAllOrders()
        {
            OrderRepository _orderRepository = new OrderRepository();
            List<order> orders = _orderRepository.GetAllOrders();
            return Ok(orders);
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetAllOrdersById(int id)
        {
            OrderRepository _orderRepository = new OrderRepository();
            order order = _orderRepository.GetOrderById(id);
            return Ok(order);
        }

        [HttpPost("add")]
        [HttpPost]
        [Authorize]
        public IActionResult AddNewOrder([FromBody] order orderDetails)
        {
            try
            {
                OrderRepository _DbOrder = new OrderRepository();
                if (_DbOrder.AddOrder(orderDetails))
                {
                    return Ok(); // 200 OK
                }
                return BadRequest(); // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return StatusCode(500, "An error occurred while processing your request."); // 500 Internal Server Error
            }
        }


        // PUT api/<HomeApiController>/5
        [HttpPost("{id}")]
        [Authorize]
        public IActionResult EditOrderDetails(int orderId, [FromBody] order orderDetails)
        {
            try
            {
                OrderRepository _DbOrder = new OrderRepository();
                if (_DbOrder.EditOrderDetails(orderId, orderDetails))
                {
                    return Ok(); // 200 OK
                }
                return NotFound(); // 404 Not Found
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request."); // 500 Internal Server Error
            }
        }

        // DELETE api/<HomeApiController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteOrder(int id)
        {
            try
            {
                OrderRepository _orderRepository = new OrderRepository();
                if (_orderRepository.DeleteOrderDetails(id))
                {
                    return Ok(new { isDeleted = true });
                }
                else
                {
                    return NotFound(new { isDeleted = false, errorMessage = "Failed to delete Order." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isDeleted = false, errorMessage = "An error occurred while processing your request." });
            }
        }
    }
}
