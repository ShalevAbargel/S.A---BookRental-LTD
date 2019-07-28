using S.A___BookRental_LTD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace S.A___BookRental_LTD.Controllers.API
{
    public class UsersAPIController : ApiController
    {
        private ApplicationDbContext db;
        public UsersAPIController()
        {
            db = ApplicationDbContext.Create();
        }
        //TO RETREVE MAIL OR NAIM&BIRTHDATE
        public IHttpActionResult Get(string type,string query=null )
        {
            if(type.Equals("email") && query != null)
            {
                var custumerQuery = db.Users.Where(u => u.Email.ToLower().Contains(query.ToLower()));
                return Ok(custumerQuery.ToList());
            }
            if (type.Equals("name") && query != null)
            {
                var custumerQuery = from u in db.Users
                                    where u.Email.Contains(query)
                                    select new { u.FirstName, u.LastName, u.BirthDate };
                return Ok(custumerQuery.ToList()[0].FirstName + " " + custumerQuery.ToList()[0].LastName + ";" + custumerQuery.ToList()[0].BirthDate);
            }
            return BadRequest();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
        }
    }
}
