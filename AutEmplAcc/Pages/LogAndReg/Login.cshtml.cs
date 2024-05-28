using AutEmplAcc.DbContexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AutEmplAcc.Pages.LogAndReg
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
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

            // Поиск пользователя по имени пользователя
            var user = _context.Users.FirstOrDefault(u => u.Username == Input.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return Page();
            }

            // Проверка пароля
            if (!BCrypt.Net.BCrypt.Verify(Input.Password, user.Password))
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return Page();
            }

            // Создание Claims Identity 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // Добавление роли в Claims
            };
            var identity = new ClaimsIdentity(claims, "CookieAuthentication");
            var principal = new ClaimsPrincipal(identity);

            // Перенаправление на главную страницу или страницу профиля
            return RedirectToPage("/Employees/Index");
        }

        public class InputModel
        {
            [Required]
            [StringLength(50)]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Запомнить меня")]
            public bool RememberMe { get; set; }
        }
    }
}
    