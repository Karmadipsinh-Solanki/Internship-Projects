using OrderManagementSystemDAL.Models;
using OrderManagementSystemDAL.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemBLL.Interface
{
    public interface IUserRepository
    {
        public LoginResponseDTO Login(LoginRequestDTO loginRequestDTO);
    }
}
