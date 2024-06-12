using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystemBLL.Interface;
using OrderManagementSystemBLL.Repository;
using OrderManagementSystemDAL.Models;
using OrderManagementSystemDAL.Models.Dto;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderManagementSystem.Controllers
{
    //[CustomAuth]
    [Route("api/HomeApi")]
    [ApiController]
    public class HomeApiController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;
        public HomeApiController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            this._response = new();
        }
        // GET: api/<HomeApiController>
        [HttpGet]
        [Authorize]
        public IActionResult GetAllCustomers()
        {
            CustomerRepository _customerRepository = new CustomerRepository();
            List<customer> customers = _customerRepository.GetAllCustomers();
            return Ok(customers);
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetAllCustomersById(int id)
        {
            CustomerRepository _customerRepository = new CustomerRepository();
            customer customers = _customerRepository.GetCustomerById(id);
            return Ok(customers);
        }
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        // GET api/<HomeApiController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<APIResponse> Login([FromBody] LoginRequestDTO loginRequestDTO)
        //public ActionResult<APIResponse> Login([FromBody] LoginRequestDTO loginRequestDTO)
        //{
        //    var LoginResponse = _userRepo.Login(loginRequestDTO);
        //    return View();
        //}
        {
            try
            {
                if (loginRequestDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages="Please Provide Required Credentials";
                    return BadRequest(_response);
                }

                LoginResponseDTO loginResponseDTO = _userRepo.Login(loginRequestDTO);

                if (loginResponseDTO.user == null || string.IsNullOrEmpty(loginResponseDTO.token))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages="Please Provide Valid Credentials";
                    return BadRequest(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = loginResponseDTO;
                return Ok(_response);
            }
            catch (Exception exp)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages=exp.ToString();
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }




        //public void Post([FromBody] string value)
        //{
        //}
        // POST api/<HomeApiController>
        
        [HttpPost("add")]
        [Authorize]
        public IActionResult AddNewCustomer([FromForm] CustomerDTO customerDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerRepository _customerRepository = new CustomerRepository();
                    string filePath = _customerRepository.UploadFile(customerDetails.File);
                    customerDetails.profileImgPath = filePath;
                    if (_customerRepository.AddCustomer(customerDetails))
                    {
                        return Ok("Customer added successfully.");
                    }
                }
                return BadRequest("Invalid customer details.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // PUT api/<HomeApiController>/5
        [HttpPost("{id}")]
        [Authorize]
        public IActionResult EditCustomerDetails(int id, [FromForm] CustomerByIdDTO customerDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerRepository _customerRepository = new CustomerRepository();
                    // Check if the provided id matches the id in customerDetails
                    //if (id != customerDetails.customer_id)
                    //{
                    //    return BadRequest("Customer id mismatch.");
                    //}
                    customerDetails.customer_id = id;

                    if (customerDetails.File != null)
                    {
                        string filePath = _customerRepository.UploadFile(customerDetails.File);
                        customerDetails.profileImgPath = filePath;
                    }

                    if (_customerRepository.EditCustomerDetails(customerDetails))
                    {
                        return Ok("Customer details updated successfully.");
                    }
                }
                return BadRequest("Invalid customer details.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // DELETE api/<HomeApiController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteCustomer(int id)
        {
            try
            {
                CustomerRepository _customerRepository = new CustomerRepository();
                if (_customerRepository.DeleteCustomerDetails(id))
                {
                    return Ok(new { isDeleted = true });
                }
                else
                {
                    return NotFound(new { isDeleted = false, errorMessage = "Failed to delete customer." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isDeleted = false, errorMessage = "An error occurred while processing your request." });
            }
        }
    }
}




//public ActionResult<APIResponse> Login([FromBody] LoginRequestDTO loginRequestDTO)
////public ActionResult<APIResponse> Login([FromBody] LoginRequestDTO loginRequestDTO)
////{
////    var LoginResponse = _userRepo.Login(loginRequestDTO);
////    return View();
////}
//{
//    try
//    {
//        if (loginRequestDTO == null)
//        {
//            _userRepo.StatusCode = HttpStatusCode.BadRequest;
//            _userRepo.IsSuccess = false;
//            _userRepo.ErrorMessages.Add("Please Provide Required Credentials");
//            return BadRequest(_userRepo);
//        }

//        LoginResponseDTO loginResponseDTO = _auth.Login(loginRequestDTO);

//        if (loginResponseDTO.user == null || string.IsNullOrEmpty(loginResponseDTO.token))
//        {
//            _userRepo.StatusCode = HttpStatusCode.BadRequest;
//            _userRepo.IsSuccess = false;
//            _userRepo.ErrorMessages.Add("Please Provide Valid Credentials");
//            return BadRequest(_userRepo);
//        }

//        _userRepo.StatusCode = HttpStatusCode.OK;
//        _userRepo.IsSuccess = true;
//        _userRepo.Result = loginResponseDTO;
//        return Ok(_userRepo);
//    }
//    catch (Exception exp)
//    {
//        _userRepo.StatusCode = HttpStatusCode.InternalServerError;
//        _userRepo.IsSuccess = false;
//        _userRepo.ErrorMessages.Add(exp.ToString());
//        return StatusCode(StatusCodes.Status500InternalServerError, _userRepo);
//    }
//}