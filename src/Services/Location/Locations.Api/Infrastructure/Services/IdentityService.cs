using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Locations.Api.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private HttpContextAccessor _httpContextAccessor;

        public IdentityService(HttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetUserIdentity()
        {
            return this._httpContextAccessor.HttpContext.User.FindFirst("sub").Value;
        }
    }
}
