﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Popup3DMVC.Controllers
{
    public class LojaController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}