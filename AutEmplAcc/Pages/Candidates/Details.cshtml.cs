using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;

namespace AutEmplAcc.Pages.Candidates
{
    public class DetailsModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public DetailsModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
        {
            _context = context;
        }

      public Candidate Candidate { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Candidates == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates.FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }
            else 
            {
                Candidate = candidate;
            }
            return Page();
        }
    }
}
