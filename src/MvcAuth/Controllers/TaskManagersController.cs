using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MvcAuth.Models;

namespace MvcAuth.Controllers
{
    public class TaskManagersController : Controller
    {
        private ApplicationDbContext _context;

        public TaskManagersController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: TaskManagers
        public IActionResult Index()
        {
            return View(_context.TaskManager.ToList());
        }

        // GET: TaskManagers/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            TaskManager taskManager = _context.TaskManager.Single(m => m.ID == id);
            if (taskManager == null)
            {
                return HttpNotFound();
            }

            return View(taskManager);
        }

        // GET: TaskManagers/Create
        public IActionResult Create()
        {
            ViewBag.Priority = new TaskManager.Priority[] {TaskManager.Priority.VeryLow,
                TaskManager.Priority.Low, TaskManager.Priority.Average, TaskManager.Priority.High,
                TaskManager.Priority.VeryHigh};
            return View();
        }

        // POST: TaskManagers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TaskManager taskManager)
        {
            if (ModelState.IsValid)
            {
                _context.TaskManager.Add(taskManager);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(taskManager);
        }

        // GET: TaskManagers/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            TaskManager taskManager = _context.TaskManager.Single(m => m.ID == id);
            if (taskManager == null)
            {
                return HttpNotFound();
            }
            return View(taskManager);
        }

        // POST: TaskManagers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TaskManager taskManager)
        {
            if (ModelState.IsValid)
            {
                _context.Update(taskManager);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(taskManager);
        }

        // GET: TaskManagers/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            TaskManager taskManager = _context.TaskManager.Single(m => m.ID == id);
            if (taskManager == null)
            {
                return HttpNotFound();
            }

            return View(taskManager);
        }

        // POST: TaskManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            TaskManager taskManager = _context.TaskManager.Single(m => m.ID == id);
            _context.TaskManager.Remove(taskManager);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
