using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutEmplAcc.Pages.Employees
{
    public class ExportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ExportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public AutEmplAcc.Models.Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Employee = await _context.Employees
                .Include(e => e.Candidate)
                .Include(e => e.Branch)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == Id);

            if (Employee == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Employee = await _context.Employees
                .Include(e => e.Candidate)
                .Include(e => e.Branch)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == Id);

            if (Employee == null)
            {
                return NotFound();
            }

            var report = GenerateWordReport(Employee);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"�����_{Employee.Candidate.FirstName}_{Employee.Candidate.LastName}.docx");
        }

        private byte[] GenerateWordReport(AutEmplAcc.Models.Employee employee)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                {
                    var document = wordDocument.AddMainDocumentPart();
                    document.Document = new Document();

                    var body = document.Document.AppendChild(new Body());

                    // ���������� ���������
                    var title = body.AppendChild(new Paragraph());
                    title.AppendChild(new Run(new Text("����� �� ����������")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // ���������� ������� � �������
                    var section = body.AppendChild(new Paragraph());
                    section.AppendChild(new Run(new Text($"���: {employee.Candidate.FirstName}")));
                    section.AppendChild(new Run(new Text($"�������: {employee.Candidate.LastName}")));
                    section.AppendChild(new Run(new Text($"��������: {employee.Candidate.Patronymic}")));
                    section.AppendChild(new Run(new Text($"������: {employee.Branch.Name}")));
                    section.AppendChild(new Run(new Text($"���������: {employee.Position.Name}")));
                    section.AppendChild(new Run(new Text($"���� ������ �� ������: {employee.HiringDate.ToShortDateString()}")));
                    section.AppendChild(new Run(new Text($"������: {employee.Rate}")));
                    section.AppendChild(new Run(new Text($"�����: {employee.CalculateSalary()}")));
                    section.AppendChild(new Run(new Text($"���� ������: {employee.WorkExperience}")));

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
