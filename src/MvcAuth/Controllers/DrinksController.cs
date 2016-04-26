using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MvcAuth.Models;

namespace MvcAuth.Controllers
{
    public class DrinksController : Controller
    {
        private ApplicationDbContext _context;

        public DrinksController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Drinks
        public IActionResult Index()
        {
            return View(_context.Drink.ToList());
        }

        // GET: Drinks/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Drink drink = _context.Drink.Single(m => m.ID == id);
            if (drink == null)
            {
                return HttpNotFound();
            }

            return View(drink);
        }

        // GET: Drinks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drinks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Drink drink)
        {
            if (ModelState.IsValid)
            {
                _context.Drink.Add(drink);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(drink);
        }

        // GET: Drinks/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Drink drink = _context.Drink.Single(m => m.ID == id);
            if (drink == null)
            {
                return HttpNotFound();
            }
            return View(drink);
        }

        // POST: Drinks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Drink drink)
        {
            if (ModelState.IsValid)
            {
                _context.Update(drink);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(drink);
        }

        // GET: Drinks/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Drink drink = _context.Drink.Single(m => m.ID == id);
            if (drink == null)
            {
                return HttpNotFound();
            }

            return View(drink);
        }

        // POST: Drinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Drink drink = _context.Drink.Single(m => m.ID == id);
            _context.Drink.Remove(drink);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
