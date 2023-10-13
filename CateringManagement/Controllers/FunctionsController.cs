using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CateringManagement.Data;
using CateringManagement.Models;
using CateringManagement.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;

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
        public async Task<IActionResult> Index(string SearchString, int? CustomerID)
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            PopulateDropDownLists();    //Data for Customer Filter DDL
            var functions = _context.Functions
                .Include(f => f.Customer)
                .Include(f => f.FunctionType)
                .Include(f => f.FunctionRooms).ThenInclude(f=>f.Room)
                .AsNoTracking();

            //Add as many filters as needed
            if (CustomerID.HasValue)
            {
                functions = functions.Where(p => p.CustomerID == CustomerID);
                numberFilters++;
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                functions = functions.Where(f => f.Name.ToUpper().Contains(SearchString.ToUpper())
                                       || f.LobbySign.ToUpper().Contains(SearchString.ToUpper()));
                numberFilters++;
            }
            //Give feedback about the state of the filters
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                //Keep the Bootstrap collapse open
                @ViewData["ShowFilter"] = " show";
            }
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
                .Include(f => f.FunctionRooms).ThenInclude(f => f.Room)
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
            PopulateAssignedRoomData(function);
            PopulateDropDownLists(function);
            return View(function);
        }

        // POST: Functions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,LobbySign,StartTime, EndTime, SetupNotes,DurationDays,BaseCharge,PerPersonCharge,GuaranteedNumber,SOCAN,Deposit,DepositPaid,NoHST,NoGratuity,Alcohol,MealTypeID,CustomerID,FunctionTypeID")] Function function, string[] selectedOptions) //changed date to StarTime, added SetupNotes, added EndTime, added Alcohol, added MealTypeID
        {                                 
            try
            {
                //Add the selected rooms
                if (selectedOptions != null)
                {
                    foreach (var room in selectedOptions)
                    {
                        var roomToAdd = new FunctionRoom { FunctionID = function.ID, RoomID = int.Parse(room) };
                        function.FunctionRooms.Add(roomToAdd);
                    }
                }
                if (ModelState.IsValid)
                {
                    _context.Add(function);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            PopulateAssignedRoomData(function);
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

            var function = await _context.Functions
                .Include(f => f.FunctionRooms).ThenInclude(f => f.Room)
                .FirstOrDefaultAsync(f => f.ID == id);
            if (function == null)
            {
                return NotFound();
            }
            PopulateAssignedRoomData(function);
            PopulateDropDownLists(function);
            return View(function);
        }

        // POST: Functions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {
            //Add Includes into LINQ
            //Go get the function to update
            var functionToUpdate = await _context.Functions
                .Include(f => f.FunctionRooms).ThenInclude(f => f.Room)
                .FirstOrDefaultAsync(f => f.ID == id);

            // Check that we got the function or exit with a not found error
            if (functionToUpdate == null)
            {
                return NotFound();
            }

            //Update the function rooms
            UpdateFunctionRooms(selectedOptions, functionToUpdate);


            if (await TryUpdateModelAsync<Function>(functionToUpdate, "", 
                f => f.Name, f => f.LobbySign, f => f.SetupNotes, f => f.StartTime, f => f.EndTime, f => f.BaseCharge, f => f.PerPersonCharge,
                f => f.GuaranteedNumber, f => f.SOCAN, f => f.Deposit, f => f.DepositPaid, f => f.NoHST, f => f.NoGratuity, f => f.Alcohol, f => f.MealTypeID, f => f.CustomerID, f => f.FunctionTypeID))          
                //removed DurationDays, added SetupNotes, added EndTime, added Alcohol, added MealTypeID
            {                                                       
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
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
            PopulateAssignedRoomData(functionToUpdate);
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
                .Include(f => f.FunctionRooms).ThenInclude(f => f.Room)
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

        private void PopulateAssignedRoomData(Function function)
        {
            var allOptions = _context.Rooms; //grab all rooms for user to choose from 
            var currentOptionIDs = new HashSet<int>(function.FunctionRooms.Select(b => b.RoomID));
            var checkBoxes = new List<CheckOptionVM>();
            foreach (var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    ID = option.ID,
                    DisplayText = option.Name,
                    Assigned = currentOptionIDs.Contains(option.ID)
                });
            }
            ViewData["RoomOptions"] = checkBoxes;
        }

        private void UpdateFunctionRooms(string[] selectedOptions, Function functionToUpdate)
        {
            if (selectedOptions == null)
            {
                functionToUpdate.FunctionRooms = new List<FunctionRoom>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var patientOptionsHS = new HashSet<int>
                (functionToUpdate.FunctionRooms.Select(c => c.RoomID));//IDs of the currently selected conditions
            foreach (var option in _context.Rooms)
            {
                if (selectedOptionsHS.Contains(option.ID.ToString())) //It is checked
                {
                    if (!patientOptionsHS.Contains(option.ID))  //but not currently in the history
                    {
                        functionToUpdate.FunctionRooms.Add(new FunctionRoom { FunctionID = functionToUpdate.ID, RoomID = option.ID });
                    }
                }
                else
                {
                    //Checkbox Not checked
                    if (patientOptionsHS.Contains(option.ID)) //but it is currently in the history - so remove it
                    {
                        FunctionRoom conditionToRemove = functionToUpdate.FunctionRooms.SingleOrDefault(c => c.RoomID == option.ID);
                        _context.Remove(conditionToRemove);
                    }
                }
            }
        }
        private bool FunctionExists(int id)
        {
          return _context.Functions.Any(e => e.ID == id);
        }
    }
}
