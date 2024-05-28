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

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет_{Employee.Candidate.FirstName}_{Employee.Candidate.LastName}.docx");
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

                    // Добавление заголовка
                    var title = body.AppendChild(new Paragraph());
                    title.AppendChild(new Run(new Text("Отчет по сотруднику")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // Добавление раздела с данными
                    var section = body.AppendChild(new Paragraph());
                    section.AppendChild(new Run(new Text($"Имя: {employee.Candidate.FirstName}")));
                    section.AppendChild(new Run(new Text($"Фамилия: {employee.Candidate.LastName}")));
                    section.AppendChild(new Run(new Text($"Отчество: {employee.Candidate.Patronymic}")));
                    section.AppendChild(new Run(new Text($"Филиал: {employee.Branch.Name}")));
                    section.AppendChild(new Run(new Text($"Должность: {employee.Position.Name}")));
                    section.AppendChild(new Run(new Text($"Дата приема на работу: {employee.HiringDate.ToShortDateString()}")));
                    section.AppendChild(new Run(new Text($"Ставка: {employee.Rate}")));
                    section.AppendChild(new Run(new Text($"Оклад: {employee.CalculateSalary()}")));
                    section.AppendChild(new Run(new Text($"Опыт работы: {employee.WorkExperience}")));

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
