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
                .ThenInclude(e => e.Candidate) // �������� ������ � ���������
                .FirstOrDefaultAsync(sl => sl.SickLeaveId == Id); // ���� �� SickLeaveId

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

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"�����_����������_{SickLeave.Employee.Candidate.FirstName}_{SickLeave.Employee.Candidate.LastName}.docx");
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

                    // ���������� ���������
                    var title = body.AppendChild(new Paragraph());
                    title.AppendChild(new Run(new Text("����� �� ����������� �����")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // ���������� ������� � �������
                    var section = body.AppendChild(new Paragraph());
                    section.AppendChild(new Run(new Text($"���������: {sickLeave.Employee.Candidate.FirstName} {sickLeave.Employee.Candidate.LastName}")));
                    section.AppendChild(new Run(new Text($"������ �����������: {sickLeave.StartDate.ToShortDateString()}")));
                    section.AppendChild(new Run(new Text($"����� �����������: {sickLeave.EndDate.ToShortDateString()}")));
                    section.AppendChild(new Run(new Text($"�������: {sickLeave.Diagnosis}")));

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
