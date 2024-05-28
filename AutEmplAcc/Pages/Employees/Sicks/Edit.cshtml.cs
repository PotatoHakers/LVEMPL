using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;

namespace AutEmplAcc.Pages.Employees.Sicks
{
    public class EditModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public EditModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SickLeave SickLeave { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.SickLeaves == null)
            {
                return NotFound();
            }

            var sickleave =  await _context.SickLeaves.FirstOrDefaultAsync(m => m.SickLeaveId == id);
            if (sickleave == null)
            {
                return NotFound();
            }
            SickLeave = sickleave;
           ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(SickLeave).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SickLeaveExists(SickLeave.SickLeaveId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SickLeaveExists(int id)
        {
          return (_context.SickLeaves?.Any(e => e.SickLeaveId == id)).GetValueOrDefault();
        }
    }
}
