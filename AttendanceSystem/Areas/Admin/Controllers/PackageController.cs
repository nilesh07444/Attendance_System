﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class PackageController : Controller
    {
        // GET: Admin/Package
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Edit(int Id)
        {
            return View();
        }

        public ActionResult View(int Id)
        {
            return View();
        }

    }
}