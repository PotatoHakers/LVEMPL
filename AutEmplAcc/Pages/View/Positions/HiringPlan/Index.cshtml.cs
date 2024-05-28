using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutEmplAcc.Pages.View.Positions.HiringPlan
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Position> Positions { get; set; }

        public void OnGet()
        {
            Positions = _context.Positions.ToList();
        }
    }
}
