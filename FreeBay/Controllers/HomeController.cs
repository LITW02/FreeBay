using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeBay.Data;
using FreeBay.Models;

namespace FreeBay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ItemsManager manager = new ItemsManager(Properties.Settings.Default.ConStr);
            IndexViewModel viewModel = new IndexViewModel();
            viewModel.Items = manager.GetItems();
            if (Request.Cookies["itemids"] != null)
            {
                viewModel.MyIds = Request.Cookies["itemids"].Value.Split(',').Select(int.Parse);
            }

            return View(viewModel);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string name, string phone, string description)
        {
            ItemsManager manager = new ItemsManager(Properties.Settings.Default.ConStr);
            int newId = manager.Add(new Item
            {
                Name = name,
                PhoneNumber = phone,
                Description = description
            });
            string cookieString;
            if (Request.Cookies["itemids"] == null)
            {
                cookieString = newId.ToString();
            }
            else
            {
                cookieString = Request.Cookies["itemids"].Value + "," + newId;
            }

            HttpCookie cookie = new HttpCookie("itemids", cookieString);
            cookie.Expires = DateTime.Now.AddDays(14);
            Response.Cookies.Add(cookie);
            return Redirect("/home/index");

        }

        [HttpPost]
        public ActionResult Delete(int itemId)
        {
            ItemsManager manager = new ItemsManager(Properties.Settings.Default.ConStr);
            manager.Delete(itemId);

            List<int> ids = Request.Cookies["itemids"].Value.Split(',').Select(int.Parse).ToList();
            ids.Remove(itemId);
            string cookieString = String.Join(",", ids);
            HttpCookie cookie = new HttpCookie("itemids", cookieString);

            //ternary operator
            cookie.Expires = ids.Count == 0 ? DateTime.Now.AddDays(-1) : DateTime.Now.AddDays(14);
            Response.Cookies.Add(cookie);

            return Redirect("/home/index");
        }

    }
}
