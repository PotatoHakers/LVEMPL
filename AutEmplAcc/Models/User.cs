using System.ComponentModel.DataAnnotations;

namespace AutEmplAcc.Models
{

    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Добавляем поле для роли пользователя
        public string Role { get; set; } = "User"; // По умолчанию "User"

    }
}
