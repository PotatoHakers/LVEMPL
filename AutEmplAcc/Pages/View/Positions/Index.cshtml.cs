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
    public class IndexModel : PageModel
    {
        private readonly AutEmplAcc.DbContexts.ApplicationDbContext _context;

        public IndexModel(AutEmplAcc.DbContexts.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Position> Position { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Positions != null)
            {
                Position = await _context.Positions.ToListAsync();
            }
        }
    }
}
