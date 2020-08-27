using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using OfficialBallot.Models;

namespace OfficialBallot.Controllers
{
    public class UserController : Controller
    {
        private MyContext DbContext;

        public UserController(MyContext context)
        {
            DbContext = context;
        }

        [HttpGet("")]
        public ViewResult LogReg()
        {
            return View("LogReg");
        }

        [HttpPost("users/register")]
        public IActionResult Register(LogRegWrapper FromForm)
        {
            if (ModelState.IsValid)
            {
                if (DbContext.DbUsers.Any(u => u.Email == FromForm.Register.Email))
                {
                    ModelState.AddModelError("Register.Email", "Already registered? Please Log In.");
                    return LogReg();
                }

                if (DbContext.DbUsers.Any(u => u.FirstName == FromForm.Register.FirstName))
                {
                    ModelState.AddModelError("Register.FirstName", "This First Name exists. Please Log In or You should have been faster.");
                    return LogReg();
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                FromForm.Register.Password = Hasher.HashPassword(FromForm.Register, FromForm.Register.Password);

                DbContext.Add(FromForm.Register);
                DbContext.SaveChanges();


                HttpContext.Session.SetInt32("UserId", FromForm.Register.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return LogReg();
            }
        }

        [HttpPost("users/login")]
        public IActionResult Login(LogRegWrapper FromForm)
        {
            if (ModelState.IsValid)
            {
                User InDb = DbContext.DbUsers
                    .FirstOrDefault(u => u.Email == FromForm.Login.Email);

                if (InDb == null)
                {
                    ModelState.AddModelError("Login.Email", "Invalid email/password");
                    return LogReg();
                }

                PasswordHasher<LogUser> Hasher = new PasswordHasher<LogUser>();
                PasswordVerificationResult Result = Hasher.VerifyHashedPassword(FromForm.Login, InDb.Password, FromForm.Login.Password);

                if (Result == 0)
                {
                    ModelState
                    .AddModelError("Login.Email", "Invalid email/password");
                    return LogReg();
                }
                HttpContext.Session.SetInt32("UserId", InDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return LogReg();
            }
        }

        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LogReg");
        }
    }
}