using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingWithCart.ViewModel;

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
            if (Session["Cart"] != null)
            {
                using (Entities dbObj = new Entities())
                {
                    var ProductIds = (List<int>)Session["Cart"];

                    var queryResult = from prod in dbObj.tblProducts
                                      join prodDtl in dbObj.tblProductDtls on prod.productId equals prodDtl.productId
                                      join ctg in dbObj.tblCategories on prod.categoryId equals ctg.categoryId
                                      where (prod.productId == prodDtl.productId) && (prod.categoryId == ctg.categoryId)
                                      select new { prod.productId, prod.categoryId, ctg.categoryName, prodDtl.productName, prodDtl.description, prodDtl.imagePath, prodDtl.price };
                    var results = queryResult.ToList();

                    List<ProductDetailsVM> listproductDetailsVM = new List<ProductDetailsVM>();
                    foreach (var item in ProductIds)
                    {
                        var row = results.Where(c => c.productId == item);
                        foreach (var values in row)
                        {
                            ProductDetailsVM productDetails = new ProductDetailsVM()
                            {
                                productId = values.productId,
                                categoryId = values.categoryId,
                                categoryName = values.categoryName,
                                productName = values.productName,
                                description = values.description,
                                imagePath = values.imagePath,
                                price = values.price
                            };

                            listproductDetailsVM.Add(productDetails);

                        }
                    }

                    return View(listproductDetailsVM);
                }

            }
            else
            {
                return View(new List<ProductDetailsVM>());
            }
            
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

        [HttpPost]
        public JsonResult RemoveFromCart(int id)
        {
            List<int> ProductIds = (List<int>)Session["Cart"];
            List<int> result = ProductIds.Where(x => x != id).ToList();
            Session["Cart"] = null;
            Session["Cart"] = result;
            return Json(new { cartItemsCount = result.Count() });
        }
    }
}