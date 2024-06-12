using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemDAL.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessages { get; set; }
        public object Result { get; set; }
        //public object Data { get; set; }
        public bool IsSuccess { get; set; }
    }

}
