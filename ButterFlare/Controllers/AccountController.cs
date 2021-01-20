using ButterFlare.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ButterFlare.Views
{
    public class AccountController : Controller
    {
        public bool loggedIn = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

        // GET: Account
        public ActionResult Login()
        {
            PasswordHasher hasher = new PasswordHasher();
            string hashedPassword1 = hasher.HashPassword("password123");
            string hashedPassword2 = hasher.HashPassword("password123");

            //Debug.WriteLine("PASSWORDS\n " + hashedPassword1 + "\n" + hashedPassword2);
            if (loggedIn)
            {
                return RedirectToAction("Profile");
            }
            return View();
        }

        [HttpPost]
        public ActionResult LoginToAccount(string email, string password)
        {
            if (!loggedIn)
            {
                AppDataBase database = new AppDataBase();
                PasswordHasher hasher = new PasswordHasher();

                //check database
                if (database.Users.Where(u => u.email == email).SingleOrDefault() != null)
                {
                    //user exists
                    User user = database.Users.Where(u => u.email == email).SingleOrDefault();
                    //string hashedPassword = hasher.HashPassword(password);

                    //check password against the one in the database
                    if ( hasher.VerifyHashedPassword(user.password, password) == PasswordVerificationResult.Success)
                    {
                        //successful login
                        FormsAuthentication.SetAuthCookie(email, false);
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        //failed
                        Debug.WriteLine("Incorrect password");
                        ViewBag.Error = "INCORRECT PASSWORD";
                    }
                }
                else
                {
                    Debug.WriteLine("User Doesnt exist!");
                    ViewBag.Error = "NO SUCH USER";
                }
            }
            else { return RedirectToAction("Profile"); }

            return View("Login", ViewBag) ;
        }

        public ActionResult SignUp()
        {
            if (loggedIn)
            {
                return RedirectToAction("Profile");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateAccount(string uname, string email, string password, string confirmPassword)
        {
            AppDataBase database = new AppDataBase();
            PasswordHasher hasher = new PasswordHasher();

            //check database
            if (database.Users.Where(u => u.email == email).SingleOrDefault() != null ||
                database.Users.Where(u => u.username == uname).SingleOrDefault() != null)
            {
                //user already exists
                Debug.WriteLine("failed");
                ViewBag.Error = "User already exists";
            }
            else
            {
                if (password == confirmPassword)
                {
                    Guid userID = Guid.NewGuid();

                    while (database.Users.Where(u => u.UID == userID).SingleOrDefault() != null)
                    {
                        userID = Guid.NewGuid();
                    }
                    string hashedPassword = hasher.HashPassword(password);
                    database.Users.Add(new User { UID =  userID, username = uname, email = email, password = hashedPassword});
                    database.SaveChanges();
                    return View("Login");
                }
                else
                {
                    Debug.WriteLine("failed");
                    ViewBag.Error = "passwords dont match";
                }
            }

            return View("SignUp", ViewBag);
        }
        
        [Authorize]
        public new ActionResult Profile()
        {
            AppDataBase database = new AppDataBase();
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            string email = ticket.Name;
            User user = database.Users.Where(u => u.email == email).SingleOrDefault();

            Post[] myPosts = database.Posts.Where(p => p.UID == user.UID).ToArray();
            ViewBag.Posts = myPosts;
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return View("Login");
        }
    }
}