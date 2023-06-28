using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BddFindCulpritTool.Data;
using BddFindCulpritTool.Models;

namespace BddFindCulpritTool.Controllers
{
    public class ConfigurationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConfigurationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Configurations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Configuration.ToListAsync());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BackendPath")] Configuration configuration)
        {

            if (ModelState.IsValid)
            {
                _context.Add(configuration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return null;
        }

        public Task<IActionResult> Update(string backendPath)
        {
            Configuration conf = new Configuration();
            conf.BackendPath = backendPath;
            if (!_context.Configuration.Any())
            {
                return Create(conf);
            }

            _context.Configuration.First().BackendPath = backendPath;
            ViewBag.pathValidated = Directory.Exists(backendPath);
            return Edit(_context.Configuration.First().Id, _context.Configuration.First());
        }

        // POST: Configurations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BackendPath")] Configuration configuration)
        {
            if (id != configuration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(configuration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConfigurationExists(configuration.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return Redirect(nameof(Index));
        }

        private bool ConfigurationExists(int id)
        {
            return _context.Configuration.Any(e => e.Id == id);
        }
    }
}
