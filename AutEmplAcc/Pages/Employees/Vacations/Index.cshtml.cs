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
    public class IndexModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public IndexModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Vacation> Vacation { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Vacations != null)
            {
                Vacation = await _context.Vacations
                .Include(v => v.Employee).ToListAsync();
            }
        }
    }
}
