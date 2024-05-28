using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;

namespace AutEmplAcc.Pages.Employees.Vacations
{
    public class DeleteModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public DeleteModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Vacation Vacation { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Vacations == null)
            {
                return NotFound();
            }

            var vacation = await _context.Vacations.FirstOrDefaultAsync(m => m.VacationId == id);

            if (vacation == null)
            {
                return NotFound();
            }
            else 
            {
                Vacation = vacation;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Vacations == null)
            {
                return NotFound();
            }
            var vacation = await _context.Vacations.FindAsync(id);

            if (vacation != null)
            {
                Vacation = vacation;
                _context.Vacations.Remove(Vacation);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
