using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutEmplAcc.Models
{
    public class CandidateEmployee
    {
        [Key]
        public int Id { get; set; }

        // Ссылка на кандидата
        [ForeignKey("Candidate")]
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        // Включите поля Employee прямо в CandidateEmployee
        public int WorkExperience { get; set; }
        public double Rate { get; set; } //Ставка
        public int HoursWorked { get; set; }
        public DateTime HiringDate { get; set; }
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }
        public int PositionId { get; set; }
        [ForeignKey("PositionId")]
        public Position Position { get; set; }

        // Используйте ICollection для связи с другими моделями
        public ICollection<Vacation> Vacations { get; set; } = new List<Vacation>();
        public ICollection<SickLeave> SickLeaves { get; set; } = new List<SickLeave>();

        // Метод для вычисления зарплаты
        public double CalculateSalary()
        {
            double baseSalary = Position.HourlyRate * HoursWorked; // 160 часов работы в месяц
            double experienceBonus = WorkExperience * 0.2 * baseSalary;
            double salary = baseSalary + experienceBonus;
            return salary;
        }

        // Свойства для удобного доступа к данным
        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{Candidate.FirstName} {Candidate.LastName} {Candidate.Patronymic}";
            }
        }
    }
}
