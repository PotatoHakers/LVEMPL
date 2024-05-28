using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutEmplAcc.Models
{
    public class Vacation
    {
        [Key]
        public int VacationId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Type { get; set; } // "Оплачиваемый", "Неоплачиваемый", "Другой"

    }
}
