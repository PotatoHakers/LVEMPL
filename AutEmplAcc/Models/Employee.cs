using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Office.Interop.Word;

namespace AutEmplAcc.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public int WorkExperience { get; set; }
        public double Rate { get; set; } //Ставка
        public int HoursWorked { get; set; }
        public DateTime HiringDate { get; set; }

        // Ссылка на кандидата
        public int CandidateId { get; set; }
        [ForeignKey("CandidateId")]
        public Candidate Candidate { get; set; }

        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public int PositionId { get; set; }
        [ForeignKey("PositionId")]
        public Position Position { get; set; }

        public ICollection<Vacation> Vacations { get; set; } = new List<Vacation>();
        public ICollection<SickLeave> SickLeaves { get; set; } = new List<SickLeave>();

        public double CalculateSalary()
        {
            double baseSalary = Position.HourlyRate * HoursWorked; // 160 часов работы в месяц
            double experienceBonus = WorkExperience * 0.2 * baseSalary;
            double salary = baseSalary + experienceBonus;
            return salary;
        }
    }
}
