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
            // Предварительно заполнить модель, если требуется.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Проверка, существует ли пользователь с таким именем 
            if (_context.Users.Any(u => u.Username == Input.Username))
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким именем уже существует.");
                return Page();
            }

            // Хеширование пароля 
            Input.Password = BCrypt.Net.BCrypt.HashPassword(Input.Password);

            // Создание нового пользователя
            var user = new User
            {
                Username = Input.Username,
                Password = Input.Password,
                Email = Input.Email,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Отправка письма с подтверждением (реализуйте эту часть)
            // ...

            // Перенаправление на страницу входа
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
            [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

        }
    }
}
