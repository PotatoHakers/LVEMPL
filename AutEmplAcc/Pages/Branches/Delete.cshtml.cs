using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;

namespace AutEmplAcc.Pages.Bracnhes
{
    public class DeleteModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public DeleteModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Branch Branch { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches.FirstOrDefaultAsync(m => m.Id == id);

            if (branch == null)
            {
                return NotFound();
            }
            else 
            {
                Branch = branch;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Branches == null)
            {
                return NotFound();
            }
            var branch = await _context.Branches.FindAsync(id);

            if (branch != null)
            {
                Branch = branch;
                _context.Branches.Remove(Branch);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
