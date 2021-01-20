using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ButterFlare.Models;

namespace ButterFlare.Controllers
{
    public class HomeController : Controller
    {
        AppDataBase database = new AppDataBase();

        [Authorize]
        public ActionResult Index()
        {
            string err = (string)TempData["Err"];
            ViewBag.Error = err;
            ViewBag.Posts = database.Posts.ToArray();
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(HttpPostedFileBase file, string caption)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            string email = ticket.Name;
            User user = database.Users.Where(u => u.email == email).SingleOrDefault();

            int lastID = (database.Posts.Any() == false)? 0 : database.Posts.ToArray().Last().id;
            byte[] array = new byte[] { };

            if (file != null && file.ContentLength > 0 && caption.Length > 0) {
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    array = ms.GetBuffer();
                }
                
                database.Posts.Add(
                    new Post { id = lastID + 1, UID = user.UID, caption = caption, image = array });
                database.SaveChanges();
            }
            else
            {
                TempData["Err"] = "CAPTION OR IMAGE CANNOT BE LEFT BLANK.";
            }

            return RedirectToAction("Index");
        }
        

        public ActionResult About()
        {
            ViewBag.Message = "This is a simple social media web app";

            return View();
        }

    }
}