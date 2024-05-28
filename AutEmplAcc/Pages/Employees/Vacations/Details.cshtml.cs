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
    public class DetailsModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public DetailsModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
