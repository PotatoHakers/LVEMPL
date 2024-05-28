using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutEmplAcc.Models
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string FIO { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Citizenship { get; set; }

        //Служба
        public bool HasServed { get; set; }
        public string? MilitaryTicketNumber { get; set; }

        public Employee? Employee { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName} {Patronymic}";
            }
        }
    }
}
