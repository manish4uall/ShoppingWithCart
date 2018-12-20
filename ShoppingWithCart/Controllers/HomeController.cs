using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingWithCart.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using(Entities dbObj = new Entities())
            {
                var results = dbObj.tblProductDtls.ToList();
                return View(results);
            }
            
        }
        public ActionResult Cart()
        {
            return View();
        }

        public ActionResult PlaceOrder()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddToCart(int id)
        {
            List<int> ProductIds = new List<int>();
            bool productAlreadyExists = false;

            if ( Session["Cart"] == null)
            {
                ProductIds.Add(id);
                Session["Cart"] = ProductIds;
            }
            else
            {
                ProductIds = (List<int>)Session["Cart"];
                if(ProductIds.Any(x => x == id)){
                    productAlreadyExists = true;
                }
                else{
                    ProductIds.Add(id);
                    Session["Cart"] = ProductIds;
                }
                
            }
            return Json(new {cartItemsCount = ProductIds.Count, cartItems = ProductIds, productStatus = productAlreadyExists },JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCartItems()
        {
            List<int> ids = new List<int>();
            if (Session["Cart"] != null)
            {
                ids = (List<int>)Session["Cart"];
                return Json(new { cartItemsCount = ids.Count, cartItems = ids });
            }
            else
            {
                return Json(new { cartItemsCount = 0, cartItems = ids },JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpGet]
        public ActionResult Test()
        {
            return Json(new { msg = "test" }, JsonRequestBehavior.AllowGet);
        }
    }
}