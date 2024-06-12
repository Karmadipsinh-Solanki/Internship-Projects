using OrderManagementSystemDAL.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystemBLL.Interface
{
    public interface IJwtService
    {
        public string GenerateJWTAuthetication(User user);

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);
    }
}
