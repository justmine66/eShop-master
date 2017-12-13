using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Locations.Api.Models;
using Locations.Api.ViewModels;

namespace Locations.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
