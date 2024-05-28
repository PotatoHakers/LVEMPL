using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutEmplAcc.Pages.View.Positions.HiringPlan
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Position Position { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Positions.Add(Position);
            _context.SaveChanges();

            return RedirectToPage("./Index");
        }
    }
}
