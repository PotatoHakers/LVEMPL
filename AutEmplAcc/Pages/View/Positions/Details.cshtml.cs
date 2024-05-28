using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;

namespace AutEmplAcc.Pages.View.Positions
{
    public class DetailsModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public DetailsModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
        {
            _context = context;
        }

      public Position Position { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Positions == null)
            {
                return NotFound();
            }

            var position = await _context.Positions.FirstOrDefaultAsync(m => m.Id == id);
            if (position == null)
            {
                return NotFound();
            }
            else 
            {
                Position = position;
            }
            return Page();
        }
    }
}
