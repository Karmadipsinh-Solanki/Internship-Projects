using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagementSystemBLL.Interface;
using OrderManagementSystemDAL.Data;
using OrderManagementSystemDAL.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Repository
{

    public class JwtService : IJwtService
    {

        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public JwtService(IConfiguration configuration, ApplicationDbContext db)
        {
            _configuration = configuration;
            _db = db;
        }


        string IJwtService.GenerateJWTAuthetication(User user)
        {
            var user_id = "";
            var user_email = "";



            var claims = new List<Claim>
            {
                new Claim("user_id", user.user_id.ToString()),
                new Claim("user_email", user.user_email)
            };


            var secret_key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires =
                DateTime.Now.AddDays(30);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        bool IJwtService.ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {

            jwtSecurityToken = null;
            if (token == null)
            {
                return false;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                // Corrected access to the validatedToken
                jwtSecurityToken = (JwtSecurityToken)validatedToken;

                if (jwtSecurityToken != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //CookieModel IJwtService.getDetails(string token)
        //{
        //    JwtSecurityToken jwtSecurityToken = null;
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        //    tokenHandler.ValidateToken(token, new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(key),
        //        ValidateIssuer = false,
        //        ValidateAudience = false,
        //        ClockSkew = TimeSpan.Zero

        //    }, out SecurityToken validatedToken);

        //    // Corrected access to the validatedToken
        //    jwtSecurityToken = (JwtSecurityToken)validatedToken;

        //    CookieModel cookieModel = new CookieModel()
        //    {
        //        aspId = int.Parse(jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "AspNetUserId").Value),
        //        userId = int.Parse(jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "UserId").Value),
        //        role = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value,
        //        email = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
        //        name = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "Name").Value,
        //        menus = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "Menus").Value,
        //    };

        //    return cookieModel;
        //}

    }
}
