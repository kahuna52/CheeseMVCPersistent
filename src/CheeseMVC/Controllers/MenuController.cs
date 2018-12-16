using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheeseMVC.Data;
using CheeseMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheeseMVC.ViewModels;

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly CheeseDbContext _context;

        public MenuController( CheeseDbContext dbContext )
        {
            _context = dbContext;
        }
        public IActionResult Index()
        {
            IList<Menu> menus = _context.Menus.Include( c => c.CheeseMenus ).ToList();
            return View( menus );
        }

        public IActionResult Add()
        {
            var addMenuViewModel = new AddMenuViewModel();

            return View( addMenuViewModel );
        }

        [HttpPost]
        public IActionResult Add( AddMenuViewModel addMenuViewModel )
        {
            if (!ModelState.IsValid) return View(addMenuViewModel);
            var menu = new Menu
            {
                Name = addMenuViewModel.Name
            };

            _context.Menus.Add( menu );
            _context.SaveChanges();

            return Redirect( "/Menu/ViewMenu/" + menu.ID );

        }

        public IActionResult ViewMenu( int id )
        {
            var menu = _context.Menus.Single( c => c.ID == id );

            var items = _context
                .CheeseMenus
                .Include( item => item.Cheese )
                .Where( cm => cm.MenuID == id )
                .ToList();

            var viewMenuViewModel = new ViewMenuViewModel( menu , items );
            return View( viewMenuViewModel );
        }

        public IActionResult AddItem( int id )
        {
            var menu = _context.Menus.Single( c => c.ID == id );
            IList<Cheese> cheeses = _context
                .Cheeses
                .Include( c => c.Category ).ToList();

            var addMenuItemViewModel = new AddMenuItemViewModel( menu , cheeses );
            return View( addMenuItemViewModel );
        }

        [HttpPost]
        public IActionResult AddItem( AddMenuItemViewModel addMenuItemViewModel )
        {
            if (!ModelState.IsValid) return View(addMenuItemViewModel);
            IList<CheeseMenu> existingItems = _context.CheeseMenus
                .Where( c => c.CheeseID == addMenuItemViewModel.CheeseID )
                .Where( c => c.MenuID == addMenuItemViewModel.MenuID ).ToList();

            if (existingItems.Count >= 1) return Redirect("/Menu/ViewMenu/" + addMenuItemViewModel.MenuID);
            {
                var newCheeseMenu = new CheeseMenu
                {
                    MenuID = addMenuItemViewModel.MenuID ,
                    Menu = _context.Menus.Single( c => c.ID == addMenuItemViewModel.MenuID ) ,
                    CheeseID = addMenuItemViewModel.CheeseID ,
                    Cheese = _context.Cheeses.Single( c => c.ID == addMenuItemViewModel.CheeseID )
                };
                _context.CheeseMenus.Add( newCheeseMenu );
                _context.SaveChanges();

                return Redirect( "/Menu/ViewMenu/" + newCheeseMenu.MenuID );
            }

        }
    }
}