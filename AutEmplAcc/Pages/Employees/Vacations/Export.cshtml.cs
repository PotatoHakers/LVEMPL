using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutEmplAcc.Pages.Vacations
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

        public AutEmplAcc.Models.Vacation Vacation { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Vacation = await _context.Vacations
                .Include(v => v.Employee)
                .ThenInclude(e => e.Candidate) // �������� ������ � ���������
                .FirstOrDefaultAsync(v => v.VacationId == Id); // ���� �� VacationId

            if (Vacation == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Vacation = await _context.Vacations
                .Include(v => v.Employee)
                .ThenInclude(e => e.Candidate)
                .FirstOrDefaultAsync(v => v.VacationId == Id);

            if (Vacation == null)
            {
                return NotFound();
            }

            var report = GenerateWordReport(Vacation);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"�����_������_{Vacation.Employee.Candidate.FirstName}_{Vacation.Employee.Candidate.LastName}.docx");
        }

        private byte[] GenerateWordReport(AutEmplAcc.Models.Vacation vacation)
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
                    title.AppendChild(new Run(new Text("����� �� �������")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // ���������� ������� � �������
                    var section = body.AppendChild(new Paragraph());
                    section.AppendChild(new Run(new Text($"���������: {vacation.Employee.Candidate.FirstName} {vacation.Employee.Candidate.LastName}")));
                    section.AppendChild(new Run(new Text($"������ �������: {vacation.StartDate.ToShortDateString()}")));
                    section.AppendChild(new Run(new Text($"����� �������: {vacation.EndDate.ToShortDateString()}")));
                    section.AppendChild(new Run(new Text($"��� �������: {vacation.Type}")));

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
