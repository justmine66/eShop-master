﻿using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Auth.Server
{
    public class IdentitySecurityScheme : SecurityScheme
    {
        public IdentitySecurityScheme()
        {
            Type = "IdentitySecurityScheme";
            Description = "Security definition that provides to the user of Swagger a mechanism to obtain a token from the identity service that secures the api";
            Extensions.Add("authorizationUrl", "http://localhost:5103/Auth/Client/popup.html");
            Extensions.Add("flow", "implicit");
            Extensions.Add("scopes", new List<string>
            {
                "basket"
            });
        }
    }
}
