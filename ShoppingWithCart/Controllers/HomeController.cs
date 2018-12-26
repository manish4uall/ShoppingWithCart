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
            using (Entities dbObj = new Entities())
            {
                var pDetails = GetProductDetailsbyProductids();
                int total = 0;
                foreach(var item in pDetails)
                {
                    total+= item.price * item.Quantity;
                }

                List<Cart> ProductIds = (List<Cart>)Session["Cart"];
                tblOrder order = new tblOrder()
                {
                    customerName = checkout.Name,
                    address = checkout.Address,
                    orderAmount = total
                };
                dbObj.tblOrders.Add(order);
                dbObj.SaveChanges();

                var orderId = order.orderId;

               // var orderId = dbObj.tblOrders.OrderByDescending(m => m.orderId).First().orderId;

                foreach (var item in pDetails)
                {
                    if (item.Quantity > 0)
                    {
                        tblOrderProduct tblOrderProduct = new tblOrderProduct()
                        {
                            orderId = orderId,
                            productId = item.productId
                        };
                        dbObj.tblOrderProducts.Add(tblOrderProduct);
                    }
                }
                dbObj.SaveChanges();
                return Json(new { msg = "success" });

            }
                
        }

        [HttpPost]
        public JsonResult AddToCart(int id)
        {
            List<Cart> ProductIds = new List<Cart>();
            bool productAlreadyExists = false;
            int currentItemCount = 1;
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
                            currentItemCount = product.Quantity;
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
            return Json(new {cartItemsCount = ProductIds.Count, cartItems = ProductIds, productStatus = productAlreadyExists, currentItemCount },JsonRequestBehavior.AllowGet);
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
                    var lstId = ProductIds.Select(c => c.Productid).ToList();
                    ProductDetailsVM detailsVM = new ProductDetailsVM();

                    var queryResult = (from prod in dbObj.tblProducts
                                       join prodDtl in dbObj.tblProductDtls on prod.productId equals prodDtl.productId
                                       join ctg in dbObj.tblCategories on prod.categoryId equals ctg.categoryId
                                       
                                       where lstId.Contains(prod.productId)
                                       select new ProductDetailsVM
                                       {
                                           productId = prod.productId,
                                           categoryId = prod.categoryId,
                                           categoryName = ctg.categoryName,
                                           productName = prodDtl.productName,
                                           description = prodDtl.description,
                                           imagePath = prodDtl.imagePath,
                                           price = prodDtl.price                                           
                                       }).ToList();

                    var result = (from qr in queryResult
                                  join pId in ProductIds on qr.productId equals pId.Productid
                                  into rd
                                  from r in rd.DefaultIfEmpty()
                                  select new ProductDetailsVM
                                  {
                                      productId = qr.productId,
                                      categoryId = qr.categoryId,
                                      categoryName = qr.categoryName,
                                      productName = qr.productName,
                                      description = qr.description,
                                      imagePath = qr.imagePath,
                                      price = qr.price,
                                      Quantity = r.Quantity//r.Quantity
                                  }).ToList();


                    //foreach (var item in ProductIds)
                    //{
                    //    var row = queryResult.Where(c => c.productId == item.Productid).FirstOrDefault();
                    //    row.Quantity = item.Quantity;
                    //}
                    return result;
                }
                
            }
            else
            {
                return (new List<ProductDetailsVM>());
            }
        }

        [HttpPost]
        public JsonResult DeleteCart(Nullable<int> i)
        {
            Session["Cart"] = null;
            return Json(new { msg = "Delete Cart Success" });
        }

        [HttpPost]
        public JsonResult UpdateTotalCart(Nullable<int> id)
        {
            var data = GetProductDetailsbyProductids();
            int amount = 0; 
            foreach (var item in data)
            {                
                amount += item.price * item.Quantity;
            }
            if (id != null)
            {
                var result = data.Where(z => z.productId == id).FirstOrDefault();
                if (result != null)
                {
                    return Json(new { amount, itemQuantity = result.Quantity, itemTotalPrice = result.price * result.Quantity });
                }
            }
            return Json(new { amount });
        }

        [HttpPost]
        public JsonResult DecreaseProductAmount(int id)
        {
            List<Cart> ProductIds = new List<Cart>();

            int currentItemCount = 1;
            if (Session["Cart"] == null)
            {
            }
            else
            {
                ProductIds = (List<Cart>)Session["Cart"];
                if (ProductIds.Any(x => x.Productid == id))
                {
                    foreach (var product in ProductIds)
                    {
                        if (product.Productid == id)
                        {
                            if (product.Quantity > 1)
                            {
                                product.Quantity--;
                                currentItemCount = product.Quantity;
                            }
                            
                            else if(product.Quantity == 1)
                            {
                                product.Quantity--;
                                currentItemCount = product.Quantity;
                                RemoveFromCart(id);
                            }
                        }
                    }
                }
            }
            ProductIds = (List<Cart>)Session["Cart"];
            return Json(new { currentItemCount, cartItemsCount = ProductIds.Count() });
        }
    }
}