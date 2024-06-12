//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace OrderManagementSystemBLL.Auth
//{
//    [AttributeUsage(AttributeTargets.All)]
//    public class CustomAuthorize : Attribute, IAuthorizationFilter
//    {
//        private readonly string _user_name;

//        public CustomAuthorize(string user_name = "")
//        {
//            _user_name = user_name;
//        }

//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
//            var _context = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();
//            if (jwtService == null)
//            {
//                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
//                return;
//            }

//            var request = context.HttpContext.Request;
//            var token = request.Cookies["jwt"];
//            //CookieModel cookieModel = jwtService.getDetails(token);
//            //if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
//            //{
//            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
//            //    return;
//            //}
//            HttpRequest request1 = context.HttpContext.Request;
//            if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
//            {
//                if (isAjaxRequest(request1))
//                {
//                    context.Result = new JsonResult(new { error = "Failed to Authenticate User" })
//                    {
//                        StatusCode = 401
//                    };
//                }
//                else
//                {
//                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Login", action = "Index" }));
//                }
//                return;
//            }

//            CookieModel cookieModel = jwtService.getDetails(token);
//            if (cookieModel.role == "Provider")
//            {
//                var id = context.RouteData.Values["id"];
//                if (id != null)
//                {
//                    var isAllowed = _context.Requests.FirstOrDefault(r => r.RequestId == Convert.ToInt32(id));
//                    if (isAllowed != null && isAllowed.PhysicianId != cookieModel.userId)
//                    {
//                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
//                        return;
//                    }
//                }
//            }

//            if (cookieModel.role == "Patient")
//            {
//                var id = context.RouteData.Values["id"];
//                if (id != null)
//                {
//                    var isAllowed = _context.Requests.FirstOrDefault(r => r.RequestId == Convert.ToInt32(id));
//                    if (isAllowed != null && isAllowed.UserId != cookieModel.userId)
//                    {
//                        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
//                        return;
//                    }
//                }
//            }
//            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
//            var menus = jwtToken.Claims.FirstOrDefault(i => i.Type == "Menus").Value;


//            if (role == null)
//            {
//                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
//                return;
//            }

//            if (string.IsNullOrWhiteSpace(_role) || !_role.Contains(role.Value))
//            {
//                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
//                return;
//            }
//            if (_page != null)
//            {
//                if (!menus.Contains(_page))
//                {
//                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
//                    return;
//                }
//            }
//        }
//        private bool isAjaxRequest(HttpRequest request)
//        {
//            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
//        }
//    }
//}
