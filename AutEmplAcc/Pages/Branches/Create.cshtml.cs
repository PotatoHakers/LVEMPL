using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;

namespace AutEmplAcc.Pages.Bracnhes
{
    public class CreateModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public CreateModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Branch Branch { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Branches == null || Branch == null)
            {
                return Page();
            }

            _context.Branches.Add(Branch);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
