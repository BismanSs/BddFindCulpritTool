using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BddFindCulpritTool.Data;
using BddFindCulpritTool.Models;
using static BddFindCulpritTool.Controllers.HelperClasses.GitAndStageProcessing;
using System.IO;
using System.Text.RegularExpressions;

namespace BddFindCulpritTool.Controllers
{
    public class BinarySearchPrRunsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BinarySearchPrRunsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BinarySearchPrRuns
        public async Task<IActionResult> Index()
        {
            if (!_context.BinarySearchPoint.Any())
            {
                BinarySearchPoint bp = new BinarySearchPoint();
                bp.lastBadCommit = "";
                bp.lastGoodCommit = "";
                bp.bisectedCommit = "";
                BinarySearchPointsController bpc = new BinarySearchPointsController(_context);
                await bpc.Create(bp);
            }
            return View(new Tuple<IEnumerable<BinarySearchPrRun>, IEnumerable<BinarySearchPoint>>(await _context.BinarySearchPrRun.ToListAsync(), await _context.BinarySearchPoint.ToListAsync()));
        }

        // GET: BinarySearchPrRuns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binarySearchPrRun = await _context.BinarySearchPrRun
                .FirstOrDefaultAsync(m => m.Id == id);
            if (binarySearchPrRun == null)
            {
                return NotFound();
            }

            return View(binarySearchPrRun);
        }

        public async Task<IActionResult> CreateBisectForm(string lastBadCommit, string lastGoodCommit, string name)
        {
            string backendPath = _context.Configuration.First().BackendPath;
            string[] args = { backendPath, name, lastBadCommit, lastGoodCommit };
            var output = ShellProcessing(@"Bisect.ps1", args);
            for(int i =0; i< 6; i++)
            {
                Regex reg = new Regex("(.*)is the first bad commit");
                Match match = reg.Match(output.Item1[i]);
                if (match.Success)
                {
                    TempData["culpritFound"] = output.Item1[i];
                    return RedirectToAction(nameof(Index));
                }
            }
            TempData["culpritFound"] = null;
            /*if (output.Item2 != "")
            {
                ShowpopUpError();
            }*/
            string bisectedCommit = output.Item1.Last();
            BinarySearchPointsController bpc = new BinarySearchPointsController(_context);
            await bpc.UpdateFirst(lastGoodCommit, lastBadCommit, bisectedCommit);

            ViewBag.bisectedCommit = bisectedCommit;
            ViewBag.name = name;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeCommitStatus(string commitStatus)
        {
            if (commitStatus == null) { return NotFound(); }
            BinarySearchPointsController bpc = new BinarySearchPointsController(_context);
            await bpc.UpdateFromStatus(commitStatus);
            var lastRecord = _context.BinarySearchPrRun.OrderByDescending(r => r.Id).FirstOrDefault();
            if (commitStatus == "success")
            {
                lastRecord.RunStatus = 1;
            }
            else if (commitStatus == "failure")
            {
                lastRecord.RunStatus = 2;
            }
            await Edit(lastRecord.Id, lastRecord);
            return RedirectToAction(nameof(Index));
        }

        // POST: BinarySearchPrRuns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,GitHash,Link,RunStatus")] BinarySearchPrRun binarySearchPrRun)
        {
            if (ModelState.IsValid)
            {
                _context.Add(binarySearchPrRun);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: BinarySearchPrRuns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binarySearchPrRun = await _context.BinarySearchPrRun.FindAsync(id);
            if (binarySearchPrRun == null)
            {
                return NotFound();
            }
            return View(binarySearchPrRun);
        }

        // POST: BinarySearchPrRuns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,GitHash,Link,RunStatus")] BinarySearchPrRun binarySearchPrRun)
        {
            if (id != binarySearchPrRun.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(binarySearchPrRun);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BinarySearchPrRunExists(binarySearchPrRun.Id))
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
            return View(binarySearchPrRun);
        }

        // GET: BinarySearchPrRuns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var binarySearchPrRun = await _context.BinarySearchPrRun
                .FirstOrDefaultAsync(m => m.Id == id);
            if (binarySearchPrRun == null)
            {
                return NotFound();
            }

            return View(binarySearchPrRun);
        }

        // POST: BinarySearchPrRuns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var binarySearchPrRun = await _context.BinarySearchPrRun.FindAsync(id);
            _context.BinarySearchPrRun.Remove(binarySearchPrRun);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BinarySearchPrRunExists(int id)
        {
            return _context.BinarySearchPrRun.Any(e => e.Id == id);
        }

        public Task<IActionResult> ProcessThenCreate(
            string name,
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
            string[] args = { backendPath, name , _context.BinarySearchPoint.First().bisectedCommit};

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

            var _stagesBlackList = new List<string>();
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

            var prOutput = ShellProcessing(@"CreatePR.ps1", args);
            if (prOutput.Item2 != "")
            {
                ShowpopUpError();
            }
            BinarySearchPrRun newPr = new BinarySearchPrRun();
            newPr.Name = name;
            newPr.GitHash = _context.BinarySearchPoint.First().bisectedCommit;
            newPr.RunStatus = 0;
            newPr.Link = prOutput.Item1.Last();
            return Create(newPr);
        }

        public async Task<IActionResult> ShowpopUpError()
        {
            return PartialView("_ErrorPopUp");
        }
    }
}
