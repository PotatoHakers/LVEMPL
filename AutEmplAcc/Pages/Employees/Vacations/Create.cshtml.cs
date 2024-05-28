using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;

namespace AutEmplAcc.Pages.Employees.Vacations
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
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Vacation Vacation { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Vacations == null || Vacation == null)
            {
                return Page();
            }

            _context.Vacations.Add(Vacation);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
