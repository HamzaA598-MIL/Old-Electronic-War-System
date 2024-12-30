using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ElecWarSystem.Controllers
{
    public class PersonController : Controller
    {
        private PersonService personRepo;
        public PersonController()
        {
            personRepo = new PersonService();
        }
        // GET: Persons of specific unit and specific title
        [HttpGet]
        public JsonResult GetPersons(int type)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Person> persons = personRepo.GetPersons(userId, type);
            return Json(persons, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetPersonsOfRank(int rankID)
        {
            int userId = int.Parse(Request.Cookies["userID"].Value);
            List<Person> persons = personRepo.GetPersonOfRank(userId, rankID);
            return Json(persons, JsonRequestBehavior.AllowGet);
        }

        // GET: Person by id
        [HttpGet]
        public JsonResult GetPerson(int id)
        {
            Person person = personRepo.Find(id);
            return Json(person, JsonRequestBehavior.AllowGet);
        }
        // Post: Create New Person
        [HttpPost]
        public void Create(Person person)
        {
            personRepo.Add(person);
        }
        // Put: Edit exist Person
        [HttpPost]
        public void Edit(int id, Person person)
        {
            personRepo.Update(id, person);
        }
        // Delete: Delete Person
        [HttpPost]
        public bool Delete(long? id)
        {
            return personRepo.Delete(id);
        }
    }
}