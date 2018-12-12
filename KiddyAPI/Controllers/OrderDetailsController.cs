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
    public class OrderDetailsController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/OrderDetails
        public IEnumerable<OrderDetailDTO> GettblOrderDetails()
        {
            return db.tblOrderDetails.Select(ordDetail => new OrderDetailDTO
            {
                id = ordDetail.id,
                toyID = ordDetail.toyID,
                orderID = ordDetail.orderID,
                name = ordDetail.name, 
                quantity = ordDetail.quantity
            });
        }

        // GET: api/OrderDetails/5
        [ResponseType(typeof(tblOrderDetail))]
        public IHttpActionResult GettblOrderDetail(int id)
        {
            tblOrderDetail tblOrderDetail = db.tblOrderDetails.Find(id);
            if (tblOrderDetail == null)
            {
                return NotFound();
            }

            return Ok(tblOrderDetail);
        }
        // GET: api/OrderDetails/OrderDetailsByOrderID?orderID=5
        [Route("api/OrderDetails/OrderDetailsByOrderID")]
        [ResponseType(typeof(OrderDetailDTO))]
        public IEnumerable<OrderDetailDTO> GetOrderDetailsByOrderID(int orderID)
        {
            IEnumerable<OrderDetailDTO> list = db.tblOrderDetails
                .Where(ordDetail => ordDetail.orderID == orderID)
                .Select(ordDetail => new OrderDetailDTO
                {
                    id = ordDetail.id,
                    toyID = ordDetail.toyID,
                    orderID = ordDetail.orderID,
                    quantity = ordDetail.quantity,
                    price = ordDetail.price,
                    name = ordDetail.name,
                    isFeedback = ordDetail.isFeedback
                }).ToList();
            return list;
        }

        // PUT: api/OrderDetails/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PuttblOrderDetail(int id, tblOrderDetail tblOrderDetail)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != tblOrderDetail.id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(tblOrderDetail).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!tblOrderDetailExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/OrderDetails
        [ResponseType(typeof(tblOrderDetail))]
        public IHttpActionResult PosttblOrderDetail(IEnumerable<OrderDetailDTO> listDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (OrderDetailDTO item in listDetail)
            {
                tblOrderDetail ordDetail = new tblOrderDetail
                {
                    isFeedback = false,
                    name = item.name,
                    orderID = item.orderID,
                    price = item.price,
                    quantity = item.quantity,
                    toyID = item.toyID
                };
                db.tblOrderDetails.Add(ordDetail);
            }
            db.SaveChanges();

            return StatusCode(HttpStatusCode.Created);
        }

        // DELETE: api/OrderDetails/5
        [ResponseType(typeof(tblOrderDetail))]
        public IHttpActionResult DeletetblOrderDetail(int id)
        {
            tblOrderDetail tblOrderDetail = db.tblOrderDetails.Find(id);
            if (tblOrderDetail == null)
            {
                return NotFound();
            }

            db.tblOrderDetails.Remove(tblOrderDetail);
            db.SaveChanges();

            return Ok(tblOrderDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblOrderDetailExists(int id)
        {
            return db.tblOrderDetails.Count(e => e.id == id) > 0;
        }
    }
}