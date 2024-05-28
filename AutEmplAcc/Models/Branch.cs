using System.ComponentModel.DataAnnotations;

namespace AutEmplAcc.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        // Navigation property
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
