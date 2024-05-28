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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Branch> Branch { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Branch = await _context.Branches
                .Include(b => b.Employees) // Включить сотрудников в запрос
                .ThenInclude(e => e.Candidate) // Включить кандидата для сотрудника
                .Include(b => b.Employees)
                .ThenInclude(e => e.Position)
                .ToListAsync();
        }
    }
}
