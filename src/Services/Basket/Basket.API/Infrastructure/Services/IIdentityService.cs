using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure.Services
{
    public interface IIdentityService
    {
        string GetUserIdentity();
    }
}
