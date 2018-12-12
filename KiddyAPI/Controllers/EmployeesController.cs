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
    public class EmployeesController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/Employees
        public IEnumerable<EmployeeDTO> GettblEmployees()
        {
            var emList = db.tblEmployees.Where(em => em.isActived == true && em.role.Equals("Employee")).Select(em => new EmployeeDTO
            {
                username = em.username,
                dob = em.dob,
                firstname = em.firstname,
                lastname = em.lastname,
                gender = em.gender,
                image = em.image
            }).ToList();
            return emList;
        }

        

        // GET: api/Employees/5
        [ResponseType(typeof(tblEmployee))]
        public IHttpActionResult GettblEmployee(string id)
        {
            tblEmployee tblEmployee = db.tblEmployees.Find(id);
            if (tblEmployee == null)
            {
                return NotFound();
            }

            return Ok(tblEmployee);
        }
        // GET: api/Employee/

        // PUT: api/Employees/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PuttblEmployee(string id, EmployeeDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.username)
            {
                return BadRequest();
            }
            tblEmployee employee = db.tblEmployees.Single(em => em.username.Equals(id));
            employee.firstname = dto.firstname;
            employee.lastname = dto.lastname;
            employee.dob = dto.dob;
            employee.gender = dto.gender;
            employee.image = dto.image;

            db.Entry(employee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblEmployeeExists(id))
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
        // PUT: api/Employees/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/Employees/Delete")]
        public IHttpActionResult PutDeleteEmployee(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            tblEmployee employee = db.tblEmployees.Find(id);
            employee.isActived = false;

            db.Entry(employee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblEmployeeExists(id))
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

        // POST: api/Employees
        [ResponseType(typeof(EmployeeDTO))]
        public IHttpActionResult PosttblEmployee(EmployeeDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tblEmployees.Add(new tblEmployee
            {
                username = dto.username,
                password = dto.password,
                dob = dto.dob,
                firstname = dto.firstname,
                gender = dto.gender,
                isActived = dto.isActived,
                lastname = dto.lastname,
                role = dto.role,
                image = dto.image
            });
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (tblEmployeeExists(dto.username))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
                 

            return CreatedAtRoute("DefaultApi", new { id = dto.username }, dto);
        }


        //POST: api/Employees/CheckLogin
        [Route("api/Employees/CheckLogin")]
        [HttpPost]
        [ResponseType(typeof(EmployeeDTO))]
        public EmployeeDTO CheckLogin(EmployeeDTO dto)
        {
            EmployeeDTO result = null;          
                var emp = db.tblEmployees.Single(em => em.username.Equals(dto.username) && em.password.Equals(dto.password));
            if(emp != null)
            {
                result = new EmployeeDTO
                {
                    username = emp.username,
                    password = emp.password,
                    role = emp.role
                   
                };
            }
            return result;
        }

        [HttpPost]
        [Route("api/Employees/ChangePassword")]
        [ResponseType(typeof(void))]
        public IHttpActionResult ChangePassword(EmployeeDTO dto)
        {
            tblEmployee emp = db.tblEmployees.Single(em => em.username.Equals(dto.username));
            if(emp == null)
            {
                return NotFound();
            }
            emp.password = dto.password;
            db.Entry(emp).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            


            return Ok();
        }




        //Code tu sinh------------------------------------
        #region
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tblEmployeeExists(string id)
        {
            return db.tblEmployees.Count(e => e.username == id) > 0;
        }
        #endregion
    }
}