using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    //[Route("[controller]/[action]")]
    //[Route("Home")]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;

        public HomeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment, ILogger<HomeController> logger)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        //[Route("~/Home")]
        //[Route("~/")]
        //[Route("Index")]
        [AllowAnonymous]
        public ViewResult Index()
        {
            //return _employeeRepository.GetEmployee(1).Name;
            var model = _employeeRepository.GetAllEmployee();

            return View(model);
        }

        //[Route("{id?}")]
        //[Route("Details/{id?}")]
        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            //throw new Exception("Error");

            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");

            Employee employee = _employeeRepository.GetEmployee(id.Value);

            // Check the coming id from the link if it is exists or not
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };

            //Employee model = _employeeRepository.GetEmployee(1);

            /*** Using ViewData method
             *
            ViewData["Employee"] = model;
            ViewData["PageTitle"] = "Employee Details";

            */

            //ViewBag.Employee = model;
            //ViewBag.PageTitle = "Employee Details";
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
                //string uniqueFileName = null;
                // Check the upcoming model photo object is null
                //if(model.Photo != null)
                //{
                //    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                //    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                //    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                //}

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            // Getting the Id with using repository
            Employee employee = _employeeRepository.GetEmployee(id);

            // Retrieving existing employee detail from database
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath,
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                _employeeRepository.Update(employee);

                return RedirectToAction("index");
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            // Check the upcoming model photo object is null
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        //public RedirectToActionResult Delete(Employee employee)
        //{
        //    _employeeRepository.Delete(employee);
        //    return RedirectToAction("index");
        //}
    }
}
