using Dapper;
using OrderManagementSystemDAL.Models.Dto;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OrderManagementSystemDAL.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using OrderManagementSystemDAL.Data;
using OrderManagementSystemBLL.Interface;

namespace OrderManagementSystemBLL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        private string _connectionString;
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = "Server=PCA161\\SQL2019;database=New_Project;User Id=sa;password=Tatva@123;";
        }

        public LoginResponseDTO Login(LoginRequestDTO loginRequestDTO)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@user_email", loginRequestDTO.user_email);
                parameters.Add("@password", loginRequestDTO.password);

                var user = connection.QueryFirstOrDefault<User>("GetUser", parameters, commandType: CommandType.StoredProcedure);

                if (user == null)
                {
                    return new LoginResponseDTO()
                    {
                        token = "",
                        user = null
                    };
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.user_id.ToString())
            };

                var jwtToken = new JwtSecurityToken(
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddDays(30),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(_configuration["ApplicationSettings:JWT_Secret"])
                            ),
                        SecurityAlgorithms.HmacSha256Signature)
                    );

                return new LoginResponseDTO()
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    user = user
                };
            }

        }
    }
}

