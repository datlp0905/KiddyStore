using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KiddyWeb.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PagedList.Mvc;
using PagedList;

namespace KiddyWeb.Controllers
{
    public class tblToysController : Controller
    {
        private static HttpClient client = new HttpClient();
        private static string baseURL = "http://localhost:50815/api/";


        public async Task<ActionResult> LoadImage()
        {
            IEnumerable<ToyDTO> list = null;
            HttpResponseMessage response = await client.GetAsync(baseURL + "Toys/Newest");
            if (response.IsSuccessStatusCode)
            {
                string listToy = response.Content.ReadAsStringAsync().Result;
                list = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(listToy);
                foreach (var toy in list)
                {
                    string imageName = "toy_" + toy.id + ".jpg";
                    string path = Server.MapPath(@"~/Content/images/") + imageName;
                    System.IO.File.WriteAllBytes(path, toy.image);
                }
            }
            return RedirectToAction("Index");
        }

        // GET: tblToys
        public async Task<ActionResult> Index()
        {
            IEnumerable<ToyDTO> list = null;
            HttpResponseMessage response = await client.GetAsync(baseURL + "Toys/Newest");
            if (response.IsSuccessStatusCode)
            {
                string listToy = response.Content.ReadAsStringAsync().Result;
                list = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(listToy);
            }
            return View(list);
        }

        public ActionResult Login()
        {
            return RedirectToAction("Login", "tblCustomers");
        }

        public ActionResult ChangePassword()
        {
            return RedirectToAction("ChangePassword", "tblCustomers");
        }

        // GET: tblToys/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage response = await client.GetAsync(baseURL + "Toys" + "\\" + id);
            string strDto = response.Content.ReadAsStringAsync().Result;
            ToyDTO dto = JsonConvert.DeserializeObject<ToyDTO>(strDto);
            if (dto == null)
            {
                return HttpNotFound();
            }

            response = await client.GetAsync(baseURL + "Toys" + "?id=" + id + "&related=" + dto.category);
            string strRelated = response.Content.ReadAsStringAsync().Result;
            IEnumerable<ToyDTO> relatedProduct = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(strRelated);
            ViewBag.RelatedProduct = relatedProduct;

            response = await client.GetAsync(baseURL + "Feedbacks\\ToyId?toyId=" + id);
            string strFeedback = response.Content.ReadAsStringAsync().Result;
            IEnumerable<FeedbackDTO> feedbacks = JsonConvert.DeserializeObject<IEnumerable<FeedbackDTO>>(strFeedback);
            ViewBag.Feedbacks = feedbacks;
            ViewBag.NoFeedback = feedbacks.Count();

            return View(dto);
        }

        public async Task<ActionResult> Category(string category, int? page)
        {
            IEnumerable<ToyDTO> list = null;
            if (category == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!category.Equals("Lego") && !category.Equals("BoardGame") && !category.Equals("Rubik"))
            {
                return HttpNotFound();
            }
            HttpResponseMessage response = await client.GetAsync(baseURL + "Toys" + "?category=" + category);
            if (response.IsSuccessStatusCode)
            {
                string strCategory = response.Content.ReadAsStringAsync().Result;
                list = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(strCategory);
            }

            return View(list.ToPagedList(page ?? 1, 12));
        }

        public ActionResult Logout()
        {
            Session["USER"] = null;
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Search(string value, int? page)
        {
            HttpResponseMessage response = await client.GetAsync(baseURL + "Toys/Search?value=" + value);
            string strResponse = response.Content.ReadAsStringAsync().Result;
            IEnumerable<ToyDTO> list = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(strResponse);
            return View(list.ToPagedList(page ?? 1, 12));
        }
        [HttpGet]
        public ActionResult Cart()
        {

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> Cart(string listObject)
        {
            IEnumerable<ToyDTO> listToys = null;
            IEnumerable<ToyDTO> listCart = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(listObject);
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(baseURL + "Toys/listToys", listCart);
            if (responseMessage.IsSuccessStatusCode)
            {
                string strResponse = responseMessage.Content.ReadAsStringAsync().Result;
                listToys = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(strResponse);
            }

            return View(listToys);
        }
        [HttpGet]
        public ActionResult Checkout()
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> Checkout(string listObject)
        {
            IEnumerable<ToyDTO> listToys = null;
            IEnumerable<ToyDTO> listCart = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(listObject);
            if (listCart != null)
            {
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(baseURL + "Toys/listToys", listCart);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string strResponse = responseMessage.Content.ReadAsStringAsync().Result;
                    listToys = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(strResponse);
                }
            }
            return View(listToys);
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CheckoutCart([Bind(Include = "address, payment")] OrderDTO order,
            string listObject, string firstname, string lastname, string email)
        {
            IEnumerable<ToyDTO> listToy = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(listObject);
            if (Session["USER"] != null)
            {
                string username = Session["USER"].ToString();
                order.cusID = username;
                HttpResponseMessage response = await client.PostAsJsonAsync(baseURL + "Orders", order);
                string strResponse = response.Content.ReadAsStringAsync().Result;
                int orderID = JsonConvert.DeserializeObject<int>(strResponse);
                List<OrderDetailDTO> listDetail = new List<OrderDetailDTO>();
                foreach (ToyDTO item in listToy)
                {
                    listDetail.Add(new OrderDetailDTO
                    {
                        name = item.name,
                        orderID = orderID,
                        price = item.price,
                        toyID = item.id,
                        quantity = item.quantity
                    });
                }
                response = await client.PostAsJsonAsync(baseURL + "OrderDetails", listDetail);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return RedirectToAction("TransactionHistory", "tblCustomers");
                }
            }
            else
            {
                string randomUser = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                CustomerDTO cus = new CustomerDTO
                {
                    username = randomUser,
                    firstname = email,
                    lastname = firstname + " " + lastname
                };
                HttpResponseMessage response = await client.PostAsJsonAsync(baseURL + "Customers", cus);
                if(response.IsSuccessStatusCode)
                {
                    string stringResponse = response.Content.ReadAsStringAsync().Result;
                    CustomerDTO customer = JsonConvert.DeserializeObject<CustomerDTO>(stringResponse);
                    if(customer != null)
                    {
                        order.cusID = randomUser;
                        response = await client.PostAsJsonAsync(baseURL + "Orders", order);
                        string strResponse = response.Content.ReadAsStringAsync().Result;
                        int orderID = JsonConvert.DeserializeObject<int>(strResponse);
                        List<OrderDetailDTO> listDetail = new List<OrderDetailDTO>();
                        foreach (ToyDTO item in listToy)
                        {
                            listDetail.Add(new OrderDetailDTO
                            {
                                name = item.name,
                                orderID = orderID,
                                price = item.price,
                                toyID = item.id,
                                quantity = item.quantity
                            });
                        }
                        response = await client.PostAsJsonAsync(baseURL + "OrderDetails", listDetail);
                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
