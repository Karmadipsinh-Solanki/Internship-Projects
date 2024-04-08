using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Hallodoc;
//using Hallodoc.Data;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.Data;

//using HalloDoc.DataLayer.Data;
using HalloDoc.DataLayer.Models;
using HalloDoc.LogicLayer.Interface;
using HalloDoc.Repository.Interface;
using HalloDoc.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.LogicLayer.Repository
{

    public class JwtService : IJwtService    {

        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public JwtService(IConfiguration configuration, ApplicationDbContext db)
        {
            _configuration = configuration;
            _context = db;
        }


        string IJwtService.GenerateJWTAuthetication(AspNetUser aspNetUser)
        {

            AspNetUserRole aspNetRole = _context.AspNetUserRoles.Include(a => a.Role).FirstOrDefault(a => a.UserId == aspNetUser.Id);
            //RoleMenu roleMenu = _context.RoleMenus.Include(a => a.Menu).FirstOrDefault(a => a.RoleMenuId == .MenuId);
            var roleId = 0;
            var userId = "";
            var name = "";
            var Accessid = "";
            if (aspNetRole.Role.Name == "Patient")
            {
                //var menu = _context.Menus.FirstOrDefault(m => m.MenuId == menuId);
                var user = _context.Users.FirstOrDefault(a => a.AspNetUserId == aspNetUser.Id);
                userId = user.UserId.ToString();
                name = string.Concat(user.FirstName, " ", user.LastName);
                //Accessid = _context.Admins.FirstOrDefault(a=>a.AdminId == 
            }
            else if (aspNetRole.Role.Name == "Admin")
            {
                var admin = _context.Admins.FirstOrDefault(a => a.AspNetUserId == aspNetUser.Id);
                
                userId = admin.AdminId.ToString();
                name = string.Concat(admin.FirstName, " ", admin.LastName);
                roleId = (int)admin.RoleId;
            }
            else if (aspNetRole.Role.Name == "Provider")
            {
                var physician = _context.Physicians.FirstOrDefault(a => a.AspNetUserId == aspNetUser.Id);
                userId = physician.PhysicianId.ToString();
                name = string.Concat(physician.FirstName, " ", physician.LastName);
                roleId = (int)physician.RoleId;
            }




            var claims = new List<Claim>
            {
                new Claim("AspNetUserId", aspNetUser.Id.ToString()),
                new Claim(ClaimTypes.Email, aspNetUser.Email),
                new Claim(ClaimTypes.Role, aspNetRole.Role.Name),
                new Claim("UserId", userId),
                new Claim("Name", name),
                 new Claim("AccessRoleid",roleId.ToString()),
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


        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
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

        public CookieModel getDetails(string token)
        {
            JwtSecurityToken jwtSecurityToken = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
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

            CookieModel cookieModel = new CookieModel()
            {
                aspId = int.Parse(jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "AspNetUserId").Value),
                userId = int.Parse(jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "UserId").Value),
                role = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value,
                email = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
                name = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "Name").Value,
                AccessRoleId = int.Parse(jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "AccessRoleid").Value),
            };

            return cookieModel;
        }

    }
}
