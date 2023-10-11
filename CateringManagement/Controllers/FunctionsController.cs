using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CateringManagement.Data;
using CateringManagement.Models;

namespace CateringManagement.Controllers
{
    public class FunctionsController : Controller
    {
        private readonly CateringContext _context;

        public FunctionsController(CateringContext context)
        {
            _context = context;
        }

        // GET: Functions
        public async Task<IActionResult> Index()
        {
            var functions = _context.Functions
                .Include(f => f.Customer)
                .Include(f => f.FunctionType)
                .AsNoTracking();
            return View(await functions.ToListAsync());
        }

        // GET: Functions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Functions == null)
            {
                return NotFound();
            }

            var function = await _context.Functions
                .Include(f => f.Customer)
                .Include(f => f.FunctionType)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (function == null)
            {
                return NotFound();
            }

            return View(function);
        }

        // GET: Functions/Create
        public IActionResult Create()
        {
            Function function = new Function();
            PopulateDropDownLists(function);
            return View(function);
        }

        // POST: Functions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,LobbySign,StartTime,SetupNotes,DurationDays,BaseCharge,PerPersonCharge,GuaranteedNumber,SOCAN,Deposit,DepositPaid,NoHST,NoGratuity,CustomerID,FunctionTypeID")] Function function) //changed date to starttime
        {                                                               //added SetupNotes
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(function);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            
            PopulateDropDownLists(function);
            return View(function);
        }

        // GET: Functions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Functions == null)
            {
                return NotFound();
            }

            var function = await _context.Functions.FindAsync(id);
            if (function == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(function);
            return View(function);
        }

        // POST: Functions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            // Go get the function to update
            var functionToUpdate = await _context.Functions.FirstOrDefaultAsync(f => f.ID == id);

            // Check that we got the function or exit with a not found error
            if (functionToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Function>(functionToUpdate, "", 
                f => f.Name, f => f.LobbySign, f => f.SetupNotes, f => f.StartTime, f => f.BaseCharge, f => f.PerPersonCharge,
                f => f.GuaranteedNumber, f => f.SOCAN, f => f.Deposit, f => f.DepositPaid, f => f.NoHST, f => f.NoGratuity,
                f => f.CustomerID, f => f.FunctionTypeID))          //removed DurationDays
            {                                                       //added SetupNotes
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunctionExists(functionToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            PopulateDropDownLists(functionToUpdate);
            return View(functionToUpdate);
        }

        // GET: Functions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Functions == null)
            {
                return NotFound();
            }

            var function = await _context.Functions
                .Include(f => f.Customer)
                .Include(f => f.FunctionType)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (function == null)
            {
                return NotFound();
            }

            return View(function);
        }

        // POST: Functions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Functions == null)
            {
                return Problem("There are no Functions to delete.");
            }
            var function = await _context.Functions.FindAsync(id);
            try
            {
                if (function != null)
                {
                    _context.Functions.Remove(function);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(function);
        }


        //This is a twist on the PopulateDropDownLists approach
        //  Create methods that return each SelectList separately
        //  and one method to put them all into ViewData.
        //This approach allows for AJAX requests to refresh
        //DDL Data at a later date.
        private SelectList CustomerSelectList(int? selectedId)
        {
            return new SelectList(_context.Customers
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName), "ID", "FormalName", selectedId);
        }
        private SelectList FunctionTypeList(int? selectedId)
        {
            return new SelectList(_context
                .FunctionTypes
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }
        private void PopulateDropDownLists(Function function = null)
        {
            ViewData["CustomerID"] = CustomerSelectList(function?.CustomerID);
            ViewData["FunctionTypeID"] = FunctionTypeList(function?.FunctionTypeID);
        }
        private bool FunctionExists(int id)
        {
          return _context.Functions.Any(e => e.ID == id);
        }
    }
}
