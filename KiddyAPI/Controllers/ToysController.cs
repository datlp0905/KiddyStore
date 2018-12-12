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
    public class ToysController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/Toys
        public IEnumerable<ToyDTO> GettblToys()
        {
            var list = db.tblToys.Where(toy => toy.isActived == true)
                .Select(toy => new ToyDTO { id = toy.id, name = toy.name,
                                            price = toy.price, image = toy.image,
                                            category = toy.category, quantity = toy.quantity, description = toy.desciption})
                .ToList();
            return list;
        }

        //GET: api/Toys/Newest
        [HttpGet]
        [Route("api/Toys/Newest")]
        public IEnumerable<ToyDTO> GetNewest()
        {
            var list = db.tblToys.OrderByDescending(toy => toy.id)
                        .Where(toy => toy.isActived == true)
                        .Take(12)
                        .Select(toy => new ToyDTO
                        {
                            id = toy.id,
                            name = toy.name,
                            price = toy.price,
                            image = toy.image,
                            category = toy.category,
                            quantity = toy.quantity,
                            description = toy.desciption
                        })
                        .ToList();
            return list;
        }

        //GET: api/Toys/Images
        [HttpGet]
        [Route("api/Toys/Images")]
        public IEnumerable<ToyDTO> GetImages()
        {
            var list = db.tblToys.Where(toy => toy.isActived == true)
                        .Select(toy => new ToyDTO
                        {
                            id = toy.id,
                            image = toy.image
                        }).ToList();
            return list;
        }

        // GET: api/Toys/5
        [ResponseType(typeof(ToyDTO))]
        public IHttpActionResult GettblToy(int id)
        {
            var toy = db.tblToys.SingleOrDefault(t => t.id == id && t.isActived == true);
            ToyDTO dto = null;
            if(toy != null)
            {
                dto = new ToyDTO
                {
                    id = toy.id,
                    name = toy.name,
                    price = toy.price,
                    image = toy.image,
                    description = toy.desciption,
                    quantity = toy.quantity,
                    category = toy.category
                };
            }
             
            if (toy == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        //GET: api/Toys?category=BoardGame
        [HttpGet]
        public IEnumerable<ToyDTO> GetToysByCategory(string category)
        {
            var list = db.tblToys.Where(toy => toy.category == category && toy.isActived == true)
                .Select(toy => new ToyDTO
                {
                    id = toy.id,
                    name = toy.name,
                    price = toy.price,
                    image = toy.image
                }).ToList();
            return list;
        }

        //GET: api/Toys/Search?value=uno
        [HttpGet]
        [Route("api/Toys/Search")]
        public IEnumerable<ToyDTO> Search(string value)
        {
            var list = db.tblToys.Where(toy => toy.isActived == true && toy.name.Contains(value))
                        .Select(toy => new ToyDTO
                        {
                            id = toy.id,
                            name = toy.name,
                            price = toy.price,
                            image = toy.image,
                            description = toy.desciption,
                            category = toy.category
                        }).ToList();
            return list;
        }

        //GET: api/Toys?id=1&related=BoardGame
        public IEnumerable<ToyDTO> getHotToysByCategory(int id, string related)
        {
            var list = db.tblToys.OrderByDescending(toy => toy.id)
                .Where(toy => toy.category == related && toy.isActived == true && toy.id != id)
                .Select(toy => new ToyDTO { id = toy.id, name = toy.name, image = toy.image, price = toy.price, category = toy.category })
                .Take(4).ToList();
            return list;
        }

        //GET: api/Toys/Feedbacks
        //Get list toys which have feedbacks are waiting to confirm
        [HttpGet]
        [Route("api/Toys/Feedbacks")]
        public IEnumerable<ToyDTO> GetToyWithFeedback()
        {
            List<FeedbackDTO> listFeedback = db.tblFeedbacks.Where(feedback => feedback.status == 0)
                .Select(feedback => new FeedbackDTO { toyID = feedback.toyID}).ToList();
            List<int?> toyIDs = new List<int?>();
            foreach (FeedbackDTO feedback in listFeedback)
            {
                toyIDs.Add(feedback.toyID);
            }
            var list = from toy in db.tblToys
                       where toyIDs.Contains(toy.id)
                       select new ToyDTO { id = toy.id, name = toy.name};
            return list;
        }

        // PUT: api/Toys/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PuttblToy(int id, ToyDTO tblToy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tblToy.id)
            {
                return BadRequest();
            }
            
            tblToy toy = db.tblToys.Find(id);

            toy.name = tblToy.name;
            toy.price = tblToy.price;
            toy.quantity = tblToy.quantity;
            toy.category = tblToy.category;
            toy.desciption = tblToy.description;
            toy.image = tblToy.image;

            db.Entry(toy).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblToyExists(id))
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

        // PUT: api/Toys/5
        [ResponseType(typeof(void))]
        [Route("api/Toys/Delete")]
        public IHttpActionResult PuttblToy(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            tblToy toy = db.tblToys.Find(id);

            if (toy == null)
            {
                return BadRequest();
            }

            toy.isActived = false;

            db.Entry(toy).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblToyExists(id))
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

        // POST: api/Toys
        [ResponseType(typeof(ToyDTO))]
        public IHttpActionResult PosttblToy(ToyDTO toy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tblToys.Add(new tblToy {name = toy.name, price = toy.price, category = toy.category,
                                        desciption = toy.description, image = toy.image,
                                        createdBy = toy.createdBy, isActived = true, quantity = toy.quantity});
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = toy.id }, toy);
        }
        [HttpPost]
        [Route("api/Toys/listToys")]
        [ResponseType(typeof(IEnumerable<ToyDTO>))]
        public IEnumerable<ToyDTO> listToys(IEnumerable<ToyDTO> listToys)
        {
            foreach(ToyDTO toy in listToys)
            {
                tblToy temp = db.tblToys.Single(tempToy => tempToy.id == toy.id);
                toy.image = temp.image;
                toy.name = temp.name;
                toy.price = temp.price;
                toy.category = temp.category;
                toy.description = temp.desciption;
            }

            return listToys;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblToyExists(int id)
        {
            return db.tblToys.Count(e => e.id == id) > 0;
        }
    }
}