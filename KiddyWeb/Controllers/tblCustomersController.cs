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

namespace KiddyWeb.Controllers
{
    public class tblCustomersController : Controller
    {
        static HttpClient client = new HttpClient();
        private string baseURL = "http://localhost:50815/api/Customers/";

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "tblToys");
        }

        [HttpPost]
        public async Task<ActionResult> Login([Bind(Include = "username, password")] CustomerDTO customer)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(baseURL + "CheckLogin", customer);
            string resString = response.Content.ReadAsStringAsync().Result;
            CustomerDTO dto = JsonConvert.DeserializeObject<CustomerDTO>(resString);
            if (dto != null)
            {
                Session["USER"] = dto.username;
                Session["LASTNAME"] = dto.lastname;
                return RedirectToAction("Index", "tblToys");
            }
            else
            {
                ViewBag.Invalid = "Invalid email or password!";
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Register([Bind(Include = "username, firstname, lastname, password")]CustomerDTO dto)
        {
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(baseURL, dto);
            if (responseMessage.IsSuccessStatusCode)
            {
                string strResponse = responseMessage.Content.ReadAsStringAsync().Result;
                CustomerDTO customer = JsonConvert.DeserializeObject<CustomerDTO>(strResponse);
                if (customer != null)
                {
                    Session["USER"] = dto.username;
                    Session["LASTNAME"] = dto.lastname;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Fail = "Register fail!";
                }
            } else if(responseMessage.StatusCode == HttpStatusCode.Conflict)
            {
                ViewBag.Duplicate = "This username is already existed!";
            }

            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(string oldpassword, string newpassword)
        {
            if (Session["USER"] != null)
            {
                string username = Session["USER"].ToString();
                CustomerDTO customer = new CustomerDTO { username = username, password = oldpassword };
                HttpResponseMessage response = await client.PostAsJsonAsync(baseURL + "CheckLogin", customer);
                string resString = response.Content.ReadAsStringAsync().Result;
                CustomerDTO dto = JsonConvert.DeserializeObject<CustomerDTO>(resString);
                if (dto != null)
                {
                    dto.password = newpassword;
                    response = await client.PutAsJsonAsync(baseURL + "ChangePassword", dto);
                    response.EnsureSuccessStatusCode();
                    ViewBag.Success = "Change Password Success!";
                }
                else
                {
                    ViewBag.Invalid = "Old password is wrong! Please try again!";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpGet]
        public async Task<ActionResult> TransactionHistory()
        {
            if (Session["USER"] != null)
            {
                string username = Session["USER"].ToString();
                IEnumerable<OrderDTO> list = null;
                if (username != null)
                {
                    HttpResponseMessage response = await client.GetAsync("http://localhost:50815/api/Orders/OrdersByCusID?cusID=" + username);
                    string strResponse = response.Content.ReadAsStringAsync().Result;
                    list = JsonConvert.DeserializeObject<IEnumerable<OrderDTO>>(strResponse);
                }
                return View(list);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public async Task<ActionResult> TransactionHistoryDetail([Bind(Include = "id, date, payment, status, address")]OrderDTO dto)
        {
            IEnumerable<OrderDetailDTO> list = null;
            HttpResponseMessage response = await client.GetAsync("http://localhost:50815/api/OrderDetails/OrderDetailsByOrderID?orderID=" + dto.id);
            string strResponse = response.Content.ReadAsStringAsync().Result;
            list = JsonConvert.DeserializeObject<IEnumerable<OrderDetailDTO>>(strResponse);
            ViewBag.Order = dto;
            return View(list);
        }

        [HttpGet]
        public ActionResult ProductFeedback(int toyID, string name, int orderID)
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Feedback([Bind(Include = "content, toyID")]FeedbackDTO dto, int orderID)
        {
            if (Session["USER"] != null)
            {
                dto.cusID = Session["USER"].ToString();
                HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:50815/api/Feedbacks?orderID=" + orderID, dto);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("Details", "tblToys", new { id = dto.toyID });
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // POST: tblCustomers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "username,password,isActived,firstname,lastname")] tblCustomer tblCustomer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.tblCustomers.Add(tblCustomer);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(tblCustomer);
        //}

        // GET: tblCustomers/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    tblCustomer tblCustomer = db.tblCustomers.Find(id);
        //    if (tblCustomer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tblCustomer);
        //}

        // POST: tblCustomers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "username,password,isActived,firstname,lastname")] tblCustomer tblCustomer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(tblCustomer).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(tblCustomer);
        //}

        // GET: tblCustomers/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    tblCustomer tblCustomer = db.tblCustomers.Find(id);
        //    if (tblCustomer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tblCustomer);
        //}
    }
}
