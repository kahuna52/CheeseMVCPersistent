using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;

namespace CheeseMVC.Controllers
{
    public class CategoryController : Controller
    {

        private readonly CheeseDbContext _context;

        public CategoryController( CheeseDbContext dbContext )
        {
            _context = dbContext;
        }

        public IActionResult Index()
        {
            IList<CheeseCategory> categoryList = _context.Categories.ToList();

            return View( categoryList );
        }

        public IActionResult Add()
        {
            var addCategoryViewModel = new AddCategoryViewModel();

            return View( addCategoryViewModel );
        }
        [HttpPost]
        public IActionResult Add( AddCategoryViewModel addCategoryViewModel )
        {
            if (!ModelState.IsValid) return View(addCategoryViewModel);
            var newCategory = new CheeseCategory
            {
                Name = addCategoryViewModel.Name

            };
            _context.Categories.Add( newCategory );
            _context.SaveChanges();
            return Redirect( "/Category/Index" );
        }

    }
}