using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;

namespace AutEmplAcc.Pages.Employees.Sicks
{
    public class DeleteModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public DeleteModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
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

            var sickleave = await _context.SickLeaves.FirstOrDefaultAsync(m => m.SickLeaveId == id);

            if (sickleave == null)
            {
                return NotFound();
            }
            else 
            {
                SickLeave = sickleave;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.SickLeaves == null)
            {
                return NotFound();
            }
            var sickleave = await _context.SickLeaves.FindAsync(id);

            if (sickleave != null)
            {
                SickLeave = sickleave;
                _context.SickLeaves.Remove(SickLeave);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
