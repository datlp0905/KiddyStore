using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using KiddyAPI.Models;

namespace KiddyAPI.Controllers
{
    public class CustomersController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/Customers
        public IEnumerable<CustomerDTO> GettblCustomers()
        {
            var cusList = db.tblCustomers.Where(cus => cus.isActived == true)
                .Select(cus => new CustomerDTO { username = cus.username, firstname = cus.firstname, lastname = cus.lastname });
            return cusList;
        }

        // PUT: api/Customers/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/Customers/Block")]
        public IHttpActionResult tblCustomer(CustomerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            tblCustomer cus = db.tblCustomers.Find(dto.username);
            if (cus == null)
            {
                return BadRequest();
            }
            cus.isActived = false;

            db.Entry(cus).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblCustomerExists(dto.username))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Customers/ChangePassword
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/Customers/ChangePassword")]
        public IHttpActionResult ChangePassword(CustomerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            tblCustomer cus = db.tblCustomers.Find(dto.username);

            if(cus == null)
            {
                return BadRequest();
            }
            cus.password = dto.password;

            db.Entry(cus).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblCustomerExists(dto.username))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Customers
        [ResponseType(typeof(tblCustomer))]
        public IHttpActionResult PosttblCustomer(CustomerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            tblCustomer cus = new tblCustomer
            {
                username = dto.username,
                firstname = dto.firstname,
                lastname = dto.lastname,
                password = dto.password,
                isActived = true
            };
            db.tblCustomers.Add(cus);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (tblCustomerExists(cus.username))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cus.username }, dto);
        }


        //POST: api/Customers/CheckLogin
        [HttpPost]
        [Route("api/Customers/CheckLogin")]
        [ResponseType(typeof(CustomerDTO))]
        public CustomerDTO checkLogin(CustomerDTO customer)
        {
            CustomerDTO dto = null;
            var cus = db.tblCustomers.SingleOrDefault(c => c.username == customer.username 
                                                       && c.password == customer.password
                                                       && c.isActived == true);
            if(cus != null)
            {
                dto = new CustomerDTO { username = cus.username, lastname = cus.lastname };
            }
            return dto;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblCustomerExists(string id)
        {
            return db.tblCustomers.Count(e => e.username == id) > 0;
        }
    }
}