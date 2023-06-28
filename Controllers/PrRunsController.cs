using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BddFindCulpritTool.Data;
using BddFindCulpritTool.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;
using static BddFindCulpritTool.Controllers.HelperClasses.GitAndStageProcessing;

namespace BddFindCulpritTool.Controllers
{
    public class PrRunsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private List<string> _stagesBlackList;

        public PrRunsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PrRuns
        public async Task<IActionResult> Index()
        {
            return View(await _context.PrRun.ToListAsync());
        }

        // GET: PrRuns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prRun = await _context.PrRun
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prRun == null)
            {
                return NotFound();
            }

            return View(prRun);
        }

        public async Task<IActionResult> ProcessThenCreateForm()
        {
            return View();
        }

        public Task<IActionResult> ProcessThenCreate(
            string name,
            string gitHash,
            bool allPlatforms, 
            bool android,
            bool apple,
            bool globalSettings,
            bool iOS,
            bool linux,
            bool mac,
            bool sotiHub,
            bool windowsClassicDesktop,
            bool windowsModern,
            bool wmce,
            bool accSecurity,
            bool accGeneral,
            bool upgradev14,
            bool upgradev155,
            bool upgradev156,
            bool upgradeCurrent
            )
        {
            string backendPath = _context.Configuration.First().BackendPath;
            string bddRelativePath = backendPath + @"\Bdd\Bdd.Upgrade\Features";
            string[] args = { backendPath, name, gitHash };
            if (ShellProcessing(@"Checkout.ps1",args).Item2 != "")
            {
                ShowpopUpError();
            }

            if (!allPlatforms && Directory.Exists(bddRelativePath + @"\AllPlatforms"))
            {
                Directory.Delete(bddRelativePath + @"\AllPlatforms", true);
            }
            if (!android && Directory.Exists(bddRelativePath + @"\Android"))
            {
                Directory.Delete(bddRelativePath + @"\Android", true);
            }
            if (!apple && Directory.Exists(bddRelativePath + @"\Apple"))
            {
                Directory.Delete(bddRelativePath + @"\Apple", true);
            }
            if (!globalSettings && Directory.Exists(bddRelativePath + @"\GlobalSettings"))
            {
                Directory.Delete(bddRelativePath + @"\GlobalSettings", true);
            }
            if (!iOS && Directory.Exists(bddRelativePath + @"\IOS"))
            {
                Directory.Delete(bddRelativePath + @"\IOS", true);
            }
            if (!linux && Directory.Exists(bddRelativePath + @"\Linux"))
            {
                Directory.Delete(bddRelativePath + @"\Linux", true);
            }
            if (!mac && Directory.Exists(bddRelativePath + @"\Mac"))
            {
                Directory.Delete(bddRelativePath + @"\Mac", true);
            }
            if (!sotiHub && Directory.Exists(bddRelativePath + @"\SotiHub"))
            {
                Directory.Delete(bddRelativePath + @"\SotiHub", true);
            }
            if (!windowsClassicDesktop && Directory.Exists(bddRelativePath + @"\WindowsClassicDesktop"))
            {
                Directory.Delete(bddRelativePath + @"\WindowsClassicDesktop", true);
            }
            if (!windowsModern && Directory.Exists(bddRelativePath + @"\WindowsModern"))
            {
                Directory.Delete(bddRelativePath + @"\WindowsModern", true);
            }
            if (!wmce && Directory.Exists(bddRelativePath + @"\WMCE"))
            {
                Directory.Delete(bddRelativePath + @"\WMCE", true);
            }

            _stagesBlackList = new List<string>();
            if (!accGeneral)
            {
                _stagesBlackList.Add("stage(.*)Acceptance-General");
            }
            if (!accSecurity)
            {
                _stagesBlackList.Add("stage(.*)Acceptance-Security");
            }
            if (!upgradev14)
            {
                _stagesBlackList.Add("stage(.*)Upgrade-v14");
            }
            if (!upgradev155)
            {
                _stagesBlackList.Add("stage(.*)Upgrade-v15.5");
            }
            if (!upgradev156)
            {
                _stagesBlackList.Add("stage(.*)Upgrade-v15.6");
            }
            if (!upgradeCurrent)
            {
                _stagesBlackList.Add("stage(.*)Upgrade-CurrentBuild");
            }
            if (_stagesBlackList.Any())
            {
                JenkinsStageRemove(backendPath, _stagesBlackList);
            }

            if (ShellProcessing(@"CreatePR.ps1", args).Item2 != "")
            {
                ShowpopUpError();
            }
            PrRun newPr = new PrRun();
            newPr.Name = name;
            newPr.GitHash = gitHash;
            newPr.RunStatus = 0;
            return Create(newPr);
        }



        // POST: PrRuns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,GitHash,Link,RunStatus")] PrRun prRun)
        {

            if (ModelState.IsValid)
            {
                _context.Add(prRun);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return null;
        }

        public async Task<IActionResult> ShowpopUpError()
        {
            return PartialView("_ErrorPopUp");
        }

        // GET: PrRuns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prRun = await _context.PrRun.FindAsync(id);
            if (prRun == null)
            {
                return NotFound();
            }
            return View(prRun);
        }

        // POST: PrRuns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Link,RunStatus")] PrRun prRun)
        {
            if (id != prRun.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prRun);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrRunExists(prRun.Id))
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
            return View(prRun);
        }

        // GET: PrRuns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prRun = await _context.PrRun
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prRun == null)
            {
                return NotFound();
            }

            return View(prRun);
        }

        // POST: PrRuns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prRun = await _context.PrRun.FindAsync(id);
            _context.PrRun.Remove(prRun);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrRunExists(int id)
        {
            return _context.PrRun.Any(e => e.Id == id);
        }

    }
}
