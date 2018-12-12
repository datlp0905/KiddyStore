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
    public class OrdersController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/Orders
        public IEnumerable<OrderDTO> GettblOrders()
        {
            return db.tblOrders.Select(ord => new OrderDTO
            {
                id= ord.id,
                date = ord.date,
                cusID = ord.cusID,
                address = ord.address,
                payment = ord.payment,
                emlID = ord.emlID,
                status = ord.status
                
            }).ToList();
        }

        // GET: api/Orders/5
        [ResponseType(typeof(tblOrder))]
        public IHttpActionResult GettblOrder(int id)
        {
            tblOrder tblOrder = db.tblOrders.Find(id);
            if (tblOrder == null)
            {
                return NotFound();
            }

            return Ok(tblOrder);
        }
        // GET: api/Orders/OrdersByCusID
        [Route("api/Orders/OrdersByCusID")]
        public IEnumerable<OrderDTO> GetListOrdersByCustomerId(string cusID)
        {
            return db.tblOrders
                .OrderByDescending(ord => ord.date)
                .Where(ord => ord.cusID.Equals(cusID)).Select(ord => new OrderDTO
            {
                id = ord.id,
                date = ord.date,
                status = ord.status,
                payment = ord.payment,
                address = ord.address
            }).ToList();
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PuttblOrder(int id, OrderDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.id)
            {
                return BadRequest();
            }
            tblOrder tblOrder = new tblOrder
            {
                id = dto.id,
                cusID = dto.cusID,
                emlID = dto.emlID,
                payment = dto.payment,
                address = dto.address,
                date = dto.date,
                status = dto.status
            };
            db.Entry(tblOrder).State = EntityState.Modified;
            if(dto.status == 1)
            {
                IEnumerable<tblOrderDetail> listDetail = db.tblOrderDetails
                    .Where(detail => detail.orderID == dto.id)
                    .ToList();
                foreach (tblOrderDetail detail in listDetail)
                {
                    tblToy toy = db.tblToys.Find(detail.toyID);
                    toy.quantity -= detail.quantity;
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblOrderExists(id))
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

        // POST: api/Orders
        [ResponseType(typeof(tblOrder))]
        public int PosttblOrder(OrderDTO dto)
        {
            tblOrder order = new tblOrder
            {
                cusID = dto.cusID,
                address = dto.address,
                payment = dto.payment,
                date = DateTime.Now,
                status = 0
            };
            db.tblOrders.Add(order);
            db.SaveChanges();

            return order.id;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblOrderExists(int id)
        {
            return db.tblOrders.Count(e => e.id == id) > 0;
        }
    }
}