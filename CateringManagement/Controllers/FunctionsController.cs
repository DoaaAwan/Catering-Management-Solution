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
using CateringManagement.Utilities;
using CateringManagement.CustomControllers;
using System.Numerics;

namespace CateringManagement.Controllers
{
    public class FunctionsController : ElephantController
    {
        private readonly CateringContext _context;

        public FunctionsController(CateringContext context)
        {
            _context = context;
        }

        // GET: Functions
        public async Task<IActionResult> Index(string SearchString, int? CustomerID, int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Function Date")
        {
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;
            //Then in each "test" for filtering, add to the count of Filters applied

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] {"Function Date", "Guaranteed Number", "Customer"};

            PopulateDropDownLists();    //Data for Customer Filter DDL

            //Start with Includes but make sure your expression returns an
            //IQueryable<Patient> so we can add filter and sort 
            //options later.
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

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }

            //Now we know which field and direction to sort by
            if (sortField == "Customer")
            {
                if (sortDirection == "asc")
                {
                    functions = functions
                        .OrderBy(f => f.Customer.LastName)
                        .ThenBy(f => f.Customer.FirstName);
                }
                else
                {
                    functions = functions
                        .OrderByDescending(f => f.Customer.LastName)
                        .ThenByDescending(f => f.Customer.FirstName);
                }
            }
            else if (sortField == "Guaranteed Number")
            {
                if (sortDirection == "asc")
                {
                    functions = functions
                        .OrderBy(f => f.Customer.CompanyName);
                }
                else
                {
                    functions = functions
                        .OrderByDescending(f => f.Customer.CompanyName);
                }
            }
            else //Sorting by Function Date
            {
                if (sortDirection == "asc")
                {
                    functions = functions
                        .OrderByDescending(f => f.StartTime);
                }
                else
                {
                    functions = functions
                        .OrderBy(f => f.StartTime);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Function>.CreateAsync(functions.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
            //return View(await functions.ToListAsync());
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
                    return RedirectToAction("Details", new { function.ID });
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
        public async Task<IActionResult> Edit(int id, string[] selectedOptions, Byte[] RowVersion)
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

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(functionToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            if (await TryUpdateModelAsync<Function>(functionToUpdate, "", 
                f => f.Name, f => f.LobbySign, f => f.SetupNotes, f => f.StartTime, f => f.EndTime, f => f.BaseCharge, f => f.PerPersonCharge,
                f => f.GuaranteedNumber, f => f.SOCAN, f => f.Deposit, f => f.DepositPaid, f => f.NoHST, f => f.NoGratuity, f => f.Alcohol, f => f.MealTypeID, f => f.CustomerID, f => f.FunctionTypeID))          
                //removed DurationDays, added SetupNotes, added EndTime, added Alcohol, added MealTypeID
            {                                                       
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { functionToUpdate.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException ex)// Added for concurrency
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Function)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Patient was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Function)databaseEntry.ToObject();
                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Name", "Current value: "
                                + databaseValues.Name);
                        if (databaseValues.LobbySign != clientValues.LobbySign)
                            ModelState.AddModelError("LobbySign", "Current value: "
                                + databaseValues.LobbySign);
                        if (databaseValues.SetupNotes != clientValues.SetupNotes)
                            ModelState.AddModelError("SetupNotes", "Current value: "
                                + databaseValues.SetupNotes);
                        if (databaseValues.StartTime != clientValues.StartTime)
                            ModelState.AddModelError("StartTime", "Current value: "
                                + String.Format("{0:d}", databaseValues.StartTime));
                        if (databaseValues.EndTime != clientValues.EndTime)
                            ModelState.AddModelError("EndTime", "Current value: "
                                + String.Format("{0:d}", databaseValues.EndTime));
                        if (databaseValues.BaseCharge != clientValues.BaseCharge)
                            ModelState.AddModelError("BaseCharge", "Current value: "
                                + databaseValues.BaseCharge);
                        if (databaseValues.PerPersonCharge != clientValues.PerPersonCharge)
                            ModelState.AddModelError("PerPersonCharge", "Current value: "
                                + databaseValues.PerPersonCharge);
                        if (databaseValues.GuaranteedNumber != clientValues.GuaranteedNumber)
                            ModelState.AddModelError("GuaranteedNumber", "Current value: "
                                + databaseValues.GuaranteedNumber);
                        if (databaseValues.SOCAN != clientValues.SOCAN)
                            ModelState.AddModelError("SOCAN", "Current value: "
                                + databaseValues.SOCAN);
                        if (databaseValues.Deposit != clientValues.Deposit)
                            ModelState.AddModelError("Deposit", "Current value: "
                                + databaseValues.Deposit);
                        if (databaseValues.DepositPaid != clientValues.DepositPaid)
                            ModelState.AddModelError("DepositPaid", "Current value: "
                                + databaseValues.DepositPaid);
                        if (databaseValues.NoHST != clientValues.NoHST)
                            ModelState.AddModelError("NoHST", "Current value: "
                                + databaseValues.NoHST);
                        if (databaseValues.NoGratuity != clientValues.NoGratuity)
                            ModelState.AddModelError("NoGratuity", "Current value: "
                                + databaseValues.NoGratuity);
                        if (databaseValues.Alcohol != clientValues.Alcohol)
                            ModelState.AddModelError("Alcohol", "Current value: "
                                + databaseValues.Alcohol);
                        //For the foreign key, we need to go to the database to get the information to show
                        if (databaseValues.CustomerID != clientValues.CustomerID)
                        {
                            Customer databaseCustomer = await _context.Customers.FirstOrDefaultAsync(i => i.ID == databaseValues.CustomerID);
                            ModelState.AddModelError("CustomerID", $"Current value: {databaseCustomer?.FullName}");
                        }
                        if (databaseValues.FunctionTypeID != clientValues.FunctionTypeID)
                        {
                            FunctionType databaseFunctionType = await _context.FunctionTypes.FirstOrDefaultAsync(i => i.ID == databaseValues.FunctionTypeID);
                            ModelState.AddModelError("FunctionTypeID", $"Current value: {databaseFunctionType?.Name}");
                        }
                        //A little extra work for the nullable foreign key.  No sense going to the database and asking for something
                        //we already know is not there.
                        if (databaseValues.MealTypeID != clientValues.MealTypeID)
                        {
                            if (databaseValues.MealTypeID.HasValue)
                            {
                                MealType databaseMedicalTrial = await _context.MealTypes.FirstOrDefaultAsync(i => i.ID == databaseValues.MealTypeID);
                                ModelState.AddModelError("MealTypeID", $"Current value: {databaseMedicalTrial?.Name}");
                            }
                            else

                            {
                                ModelState.AddModelError("MedicalTrialID", $"Current value: None");
                            }
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to Patient List' hyperlink.");
                        functionToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
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
                return Redirect(ViewData["returnURL"].ToString());
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
        private SelectList MealTypeList(int? selectedId)
        {
            return new SelectList(_context.MealTypes
                .OrderBy(d => d.Name)
                .ThenBy(d => d.ID), "ID", "Name", selectedId);
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
            ViewData["MealTypeID"] = MealTypeList(function?.MealTypeID);
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
