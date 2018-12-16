using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private CheeseDbContext _context;

        public CheeseController( CheeseDbContext dbContext )
        {
            _context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            IList<Cheese> cheeses = _context.Cheeses.Include( c => c.Category ).ToList();

            return View( cheeses );
        }

        public IActionResult Add()
        {
            var addCheeseViewModel = new AddCheeseViewModel( _context.Categories.ToList() );
            return View( addCheeseViewModel );
        }

        [HttpPost]
        public IActionResult Add( AddCheeseViewModel addCheeseViewModel )
        {
            if (!ModelState.IsValid) return View(addCheeseViewModel);
            var newCheeseCategory =
                _context.Categories.Single( c => c.ID == addCheeseViewModel.CategoryID );
            // Add the new cheese to my existing cheeses
            var newCheese = new Cheese
            {
                Name = addCheeseViewModel.Name ,
                Description = addCheeseViewModel.Description ,
                Category = newCheeseCategory ,

            };

            _context.Cheeses.Add( newCheese );
            _context.SaveChanges();

            return Redirect( "/Cheese" );

        }

        public IActionResult Remove()
        {
            ViewBag.title = "Remove Cheeses";
            ViewBag.cheeses = _context.Cheeses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Remove( int [ ] cheeseIds )
        {
            foreach ( int cheeseId in cheeseIds )
            {
                var theCheese = _context.Cheeses.Single( c => c.ID == cheeseId );
                _context.Cheeses.Remove( theCheese );
            }

            _context.SaveChanges();

            return Redirect( "/" );
        }
    }
}