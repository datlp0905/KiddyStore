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
    public class FeedbacksController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/Feedbacks
        public IEnumerable<FeedbackDTO> GettblFeedbacks()
        {
            var list = db.tblFeedbacks.Where(fb => fb.status == 0)
                .Select(fb => new FeedbackDTO { id = fb.id, content = fb.content, cusName = fb.cusID, toyID = fb.toyID })
                .ToList();
            foreach (var item in list)
            {
                item.cusName = db.tblCustomers.SingleOrDefault(cus => cus.username == item.cusName).lastname;
            }
            return list;
        }

        //// GET: api/Feedbacks/5
        //[ResponseType(typeof(tblFeedback))]
        //public IHttpActionResult GettblFeedback(int id)
        //{
        //    tblFeedback tblFeedback = db.tblFeedbacks.Find(id);
        //    if (tblFeedback == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(tblFeedback);
        //}

        //GET: api/Feedbacks?toyId=1
        [Route("api/Feedbacks/ToyId")]
        [ResponseType(typeof(FeedbackDTO))]
        public IHttpActionResult GetFeedbackList(int toyId)
        {
            var list = db.tblFeedbacks.Where(fb => fb.status == 1 && fb.toyID == toyId)
                .Select(fb => new FeedbackDTO { id = fb.id, content = fb.content, cusName = fb.cusID})
                .ToList();
            foreach (var item in list)
            {
                item.cusName = db.tblCustomers.SingleOrDefault(cus => cus.username == item.cusName).lastname;    
            }
            return Ok(list);
        }

        //// PUT: api/Feedbacks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PuttblFeedback(int id, FeedbackDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.id)
            {
                return BadRequest();
            }
            tblFeedback feedback = db.tblFeedbacks.Find(dto.id);
            if(feedback != null)
            {
                feedback.status = dto.status;
                db.Entry(feedback).State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblFeedbackExists(id))
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

        // POST: api/Feedbacks?orderID=1
        [ResponseType(typeof(FeedbackDTO))]
        public IHttpActionResult PosttblFeedback(int orderID, FeedbackDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            tblFeedback feedback = new tblFeedback
            {
                toyID = dto.toyID,
                content = dto.content,
                cusID = dto.cusID,
                status = 0
            };
            db.tblFeedbacks.Add(feedback);
            tblOrderDetail ordDetail = db.tblOrderDetails.FirstOrDefault(detail => detail.id == orderID && detail.toyID == dto.toyID);
            ordDetail.isFeedback = true;
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dto.id }, dto);
        }

        //// DELETE: api/Feedbacks/5
        //[ResponseType(typeof(tblFeedback))]
        //public IHttpActionResult DeletetblFeedback(int id)
        //{
        //    tblFeedback tblFeedback = db.tblFeedbacks.Find(id);
        //    if (tblFeedback == null)
        //    {
        //        return NotFound();
        //    }

        //    db.tblFeedbacks.Remove(tblFeedback);
        //    db.SaveChanges();

        //    return Ok(tblFeedback);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblFeedbackExists(int id)
        {
            return db.tblFeedbacks.Count(e => e.id == id) > 0;
        }
    }
}