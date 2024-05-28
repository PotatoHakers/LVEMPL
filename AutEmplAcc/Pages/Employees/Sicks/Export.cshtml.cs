using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutEmplAcc.Pages.Sicks
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

        public AutEmplAcc.Models.SickLeave SickLeave { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            SickLeave = await _context.SickLeaves
                .Include(sl => sl.Employee)
                .ThenInclude(e => e.Candidate) // Включаем данные о кандидате
                .FirstOrDefaultAsync(sl => sl.SickLeaveId == Id); // Ищем по SickLeaveId

            if (SickLeave == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            SickLeave = await _context.SickLeaves
                .Include(sl => sl.Employee)
                .ThenInclude(e => e.Candidate)
                .FirstOrDefaultAsync(sl => sl.SickLeaveId == Id);

            if (SickLeave == null)
            {
                return NotFound();
            }

            var report = GenerateWordReport(SickLeave);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет_больничный_{SickLeave.Employee.Candidate.FirstName}_{SickLeave.Employee.Candidate.LastName}.docx");
        }

        private byte[] GenerateWordReport(AutEmplAcc.Models.SickLeave sickLeave)
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
                    title.AppendChild(new Run(new Text("Отчет по больничному листу")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // Добавление раздела с данными
                    var section = body.AppendChild(new Paragraph());
                    section.AppendChild(new Run(new Text($"Сотрудник: {sickLeave.Employee.Candidate.FirstName} {sickLeave.Employee.Candidate.LastName}")));
                    section.AppendChild(new Run(new Text($"Начало больничного: {sickLeave.StartDate.ToShortDateString()}")));
                    section.AppendChild(new Run(new Text($"Конец больничного: {sickLeave.EndDate.ToShortDateString()}")));
                    section.AppendChild(new Run(new Text($"Диагноз: {sickLeave.Diagnosis}")));

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
