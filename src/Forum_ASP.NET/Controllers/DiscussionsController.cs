using Forum_ASP.NET.Models;
using Microsoft.AspNet.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Forum_ASP.NET.Controllers
{
    public class DiscussionsController : Controller
    {
        private ApplicationDbContext _context;

        public DiscussionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Discussions.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Entry()
        {
            return View(_context.Discussions.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Discussion discussion)
        {
            if (ModelState.IsValid)
            {
                discussion.CreatingDate = DateTime.Now.ToString();
                discussion.LastDate = DateTime.Now.ToString();
                discussion.Author = User.GetUserName();
                _context.Discussions.Add(discussion);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(discussion);
        }

    }
}