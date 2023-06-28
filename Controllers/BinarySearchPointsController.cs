using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BddFindCulpritTool.Data;
using BddFindCulpritTool.Models;
using System.IO;

namespace BddFindCulpritTool.Controllers
{
    public class BinarySearchPointsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BinarySearchPointsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BinarySearchPoints
        public async Task<IActionResult> Index()
        {
            return View(await _context.BinarySearchPoint.ToListAsync());
        }

        // GET: BinarySearchPoints/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binarySearchPoint = await _context.BinarySearchPoint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (binarySearchPoint == null)
            {
                return NotFound();
            }

            return View(binarySearchPoint);
        }

        // GET: BinarySearchPoints/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BinarySearchPoints/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,lastBadCommit,lastGoodCommit,bisectedCommit")] BinarySearchPoint binarySearchPoint)
        {
            if (ModelState.IsValid)
            {
                _context.Add(binarySearchPoint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: BinarySearchPoints/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binarySearchPoint = await _context.BinarySearchPoint.FindAsync(id);
            if (binarySearchPoint == null)
            {
                return NotFound();
            }
            return View(binarySearchPoint);
        }

        public Task<IActionResult> UpdateFirst(string lastGoodCommit, string lastBadCommit, string bisectedCommit)
        {
            _context.BinarySearchPoint.First().lastGoodCommit= lastGoodCommit;
            _context.BinarySearchPoint.First().lastBadCommit = lastBadCommit;
            _context.BinarySearchPoint.First().bisectedCommit= bisectedCommit;
            return Edit(_context.Configuration.First().Id, _context.BinarySearchPoint.First());
        }

        public Task<IActionResult> UpdateFromStatus(string commitStatus)
        {
            if (commitStatus == "success")
            {
                _context.BinarySearchPoint.First().lastGoodCommit = _context.BinarySearchPoint.First().bisectedCommit;
            }
            else if (commitStatus == "failure")
            {
                _context.BinarySearchPoint.First().lastBadCommit = _context.BinarySearchPoint.First().bisectedCommit;
            }
            return Edit(_context.Configuration.First().Id, _context.BinarySearchPoint.First());
        }

        // POST: BinarySearchPoints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,lastBadCommit,lastGoodCommit,bisectedCommit")] BinarySearchPoint binarySearchPoint)
        {
            if (id != binarySearchPoint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(binarySearchPoint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BinarySearchPointExists(binarySearchPoint.Id))
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
            return View(binarySearchPoint);
        }

        // GET: BinarySearchPoints/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binarySearchPoint = await _context.BinarySearchPoint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (binarySearchPoint == null)
            {
                return NotFound();
            }

            return View(binarySearchPoint);
        }

        // POST: BinarySearchPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var binarySearchPoint = await _context.BinarySearchPoint.FindAsync(id);
            _context.BinarySearchPoint.Remove(binarySearchPoint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BinarySearchPointExists(int id)
        {
            return _context.BinarySearchPoint.Any(e => e.Id == id);
        }
    }
}
