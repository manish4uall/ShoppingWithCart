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
        [HttpGet]
        public ActionResult Index()
        {
            using(Entities dbObj = new Entities())
            {
                var results = dbObj.tblProductDtls.ToList();
                return View(results);
            }
            
        }

        [HttpGet]
        public ActionResult Cart()
        {
            if (Session["Cart"] != null)
            {
                return View(GetProductDetailsbyProductids());

            }
            else
            {
                return View(new List<ProductDetailsVM>());
            }
            
        }

        [HttpGet]
        public ActionResult PlaceOrder()
        {
            
            return View(GetProductDetailsbyProductids());
        }

        [HttpPost]
        public ActionResult PlaceOrder(Checkout checkout)
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddToCart(int id)
        {
            List<Cart> ProductIds = new List<Cart>();
            bool productAlreadyExists = false;

            if ( Session["Cart"] == null)
            {
                Cart items = new Cart()
                {
                    Productid = id,
                    Quantity = 1
                    
                };
                ProductIds.Add(items);
                Session["Cart"] = ProductIds;
            }
            else
            {
                ProductIds = (List<Cart>)Session["Cart"];
                if(ProductIds.Any(x => x.Productid == id)){
                    productAlreadyExists = true;
                    foreach(var product in ProductIds)
                    {
                        if(product.Productid == id)
                        {
                            product.Quantity++;
                        }
                    }
                    
                }
                else{
                    Cart items = new Cart()
                    {
                        Productid = id,
                        Quantity = 1

                    };
                    ProductIds.Add(items);
                    
                    Session["Cart"] = ProductIds;
                }
                
            }
            return Json(new {cartItemsCount = ProductIds.Count, cartItems = ProductIds, productStatus = productAlreadyExists },JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCartItems()
        {
            List<Cart> ids = new List<Cart>();
            if (Session["Cart"] != null)
            {
                ids = (List<Cart>)Session["Cart"];
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
            List<Cart> ProductIds = (List<Cart>)Session["Cart"];
            List<Cart> result = ProductIds.Where(x => x.Productid != id).ToList();
            Session["Cart"] = null;
            Session["Cart"] = result;
            return Json(new { cartItemsCount = result.Count() });
        }

        public  List<ProductDetailsVM> GetProductDetailsbyProductids()
        {
            if (Session["Cart"] != null)
            {
                using (Entities dbObj = new Entities())
                {
                    var ProductIds = (List<Cart>)Session["Cart"];

                    var queryResult = from prod in dbObj.tblProducts
                                      join prodDtl in dbObj.tblProductDtls on prod.productId equals prodDtl.productId
                                      join ctg in dbObj.tblCategories on prod.categoryId equals ctg.categoryId
                                      where (prod.productId == prodDtl.productId) && (prod.categoryId == ctg.categoryId)
                                      select new { prod.productId, prod.categoryId, ctg.categoryName, prodDtl.productName, prodDtl.description, prodDtl.imagePath, prodDtl.price };
                    var results = queryResult.ToList();

                    List<ProductDetailsVM> listproductDetailsVM = new List<ProductDetailsVM>();
                    foreach (var item in ProductIds)
                    {
                        var row = results.Where(c => c.productId == item.Productid);
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
                                price = values.price,
                                Quantity = item.Quantity
                            };

                            listproductDetailsVM.Add(productDetails);

                        }
                    }

                    return listproductDetailsVM;
                }
            
            }
            else
            {
                return (new List<ProductDetailsVM>());
            }
        }
    }
}