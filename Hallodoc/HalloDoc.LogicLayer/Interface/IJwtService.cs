﻿using Hallodoc;
using Hallodoc.Models.Models;
using HalloDoc.DataLayer.Models;
using HalloDoc.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.Repository.Interface
{
    public interface IJwtService
    {
        public string GenerateJWTAuthetication(AspNetUser aspNetUser);

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

        CookieModel getDetails(string token);
    }
}
