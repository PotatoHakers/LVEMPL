using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;

namespace AutEmplAcc.Pages.LogAndReg
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public void OnGet()
        {
            // �������������� ��������� ������, ���� ���������.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ��������, ���������� �� ������������ � ����� ������ 
            if (_context.Users.Any(u => u.Username == Input.Username))
            {
                ModelState.AddModelError(string.Empty, "������������ � ����� ������ ��� ����������.");
                return Page();
            }

            // ����������� ������ 
            Input.Password = BCrypt.Net.BCrypt.HashPassword(Input.Password);

            // �������� ������ ������������
            var user = new User
            {
                Username = Input.Username,
                Password = Input.Password,
                Email = Input.Email,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // �������� ������ � �������������� (���������� ��� �����)
            // ...

            // ��������������� �� �������� �����
            return RedirectToPage("/Login");
        }

        public class InputModel
        {
            [Required]
            [StringLength(50)]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "������ �� ���������.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

        }
    }
}
