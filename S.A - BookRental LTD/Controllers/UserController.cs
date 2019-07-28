using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using S.A___BookRental_LTD.Models;
using S.A___BookRental_LTD.ViewModel;
using S.A___BookRental_LTD.Utility;

namespace S.A___BookRental_LTD.Controllers
{
    [Authorize(Roles = SD.AdminUserRole)]
    public class UserController : Controller
    {
        private ApplicationDbContext db;

        public UserController()
        {
            db =  ApplicationDbContext.Create();
        }
        // GET: User
        public ActionResult Index()
        {
            var user = (from u in db.Users
                        join m in db.MembershipTypes on u.MembershipTypeId equals m.Id 
                        where m.Id!=1
                        select new
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Email = u.Email,
                            Phone = u.Phone,
                            BirthDate = u.BirthDate,
                            MembershipTypeId = u.MembershipTypeId,
                            membershipTypes = (ICollection<MembershipType>)db.MembershipTypes.ToList().Where(n => n.Id ==u.MembershipTypeId),
                            Disable = u.Disable

                        }).ToList().Select(c => new UserViewModel
                        {
                            Id = c.Id,
                            FirstName = c.FirstName,
                            LastName = c.LastName,
                            Email = c.Email,
                            Phone = c.Phone,
                            BirthDate = c.BirthDate,
                            MembershipTypeId = c.MembershipTypeId,
                            //membershipTypes = (ICollection<MembershipType>)db.MembershipTypes.ToList().Where(n => n.Id == c.MembershipTypeId),
                            Disable = c.Disable

                        }).ToList();
            var UsersList = user.ToList();
            return View(UsersList);
        }

        // GET: User/Details/5
        public ActionResult Details(string id)
        {
            if (id == null || id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            UserViewModel model = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Id = user.Id,
                MembershipTypeId = user.MembershipTypeId,
                membershipTypes = db.MembershipTypes.ToList(),
                Phone = user.Phone,
                Disable = user.Disable

        };
            if (model.MembershipTypeId == 1)
            {
                model.TypeOfMembership = "SuperAdmin";
            }
            else if (model.MembershipTypeId == 2)
            {
                model.TypeOfMembership = "Member";
            }
            else if(model.MembershipTypeId == 3)
            {
                model.TypeOfMembership = "Pay Per Rental";
            }
            return View(model);
        }


        // GET: User/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            UserViewModel userViewModel = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Id = user.Id,
                MembershipTypeId = user.MembershipTypeId,
                membershipTypes = db.MembershipTypes.ToList(),
                Phone = user.Phone,
                Disable = user.Disable
            };
            return View(userViewModel);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var userInDb = db.Users.Single(u => u.Id == user.Id);
                userInDb.FirstName = user.FirstName;
                userInDb.LastName = user.LastName;
                userInDb.BirthDate = user.BirthDate;
                userInDb.Email = user.Email;
                userInDb.Phone = user.Phone;
                userInDb.MembershipTypeId = user.MembershipTypeId;  
                ///if the system breaks then delete the next row////
                userInDb.Disable = false;
            }
            else
            {
                UserViewModel model = new UserViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Id = user.Id,
                    MembershipTypeId = user.MembershipTypeId,
                    membershipTypes = db.MembershipTypes.ToList(),
                    Phone = user.Phone,
                    Disable = user.Disable
                };
                return View("Edit", model);

            }
            db.SaveChanges();
            return RedirectToAction("Index", "User");
        }

        // GET: User/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null || id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            UserViewModel model = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Id = user.Id,
                MembershipTypeId = user.MembershipTypeId,
                membershipTypes = db.MembershipTypes.ToList(),
                Phone = user.Phone,
                Disable = user.Disable
            };
            if (model.MembershipTypeId == 1)
            {
                model.TypeOfMembership = "SuperAdmin";
            }
            else if (model.MembershipTypeId == 2)
            {
                model.TypeOfMembership = "Member";
            }
            else
            {
                model.TypeOfMembership = "Pay Per Rental";
            }

            return View(model);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var userInDb = db.Users.Find(id);
            if(id==null || id.Length ==0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userInDb.Disable = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}
