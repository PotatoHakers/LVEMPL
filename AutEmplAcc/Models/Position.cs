using System.ComponentModel.DataAnnotations;

namespace AutEmplAcc.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double HourlyRate { get; set; }
        public int HiringTarget { get; set; } // Цель по набору
        public DateTime HiringStartDate { get; set; } // Дата начала набора
        public DateTime HiringEndDate { get; set; } // Дата окончания набора

        // Navigation property
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}

