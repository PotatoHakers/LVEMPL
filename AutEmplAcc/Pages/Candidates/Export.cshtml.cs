using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutEmplAcc.Pages.Candidates
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

        public AutEmplAcc.Models.Candidate Candidate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Id == Id);

            if (Candidate == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Id == Id);

            if (Candidate == null)
            {
                return NotFound();
            }

            var report = GenerateWordReport(Candidate);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет_{Candidate.FirstName}_{Candidate.LastName}.docx");
        }

        private byte[] GenerateWordReport(AutEmplAcc.Models.Candidate candidate)
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
                    title.AppendChild(new Run(new Text("Отчет по кандидату")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // Добавление раздела с данными
                    var section = body.AppendChild(new Paragraph());
                    section.AppendChild(new Run(new Text($"Имя: {candidate.FirstName}")));
                    section.AppendChild(new Run(new Text($"Фамилия: {candidate.LastName}")));
                    section.AppendChild(new Run(new Text($"Отчество: {candidate.Patronymic}")));
                    section.AppendChild(new Run(new Text($"Дата рождения: {candidate.DateOfBirth.ToShortDateString()}")));
                    section.AppendChild(new Run(new Text($"Номер телефона: {candidate.PhoneNumber}")));
                    section.AppendChild(new Run(new Text($"Адрес: {candidate.Address}")));
                    section.AppendChild(new Run(new Text($"Email: {candidate.Email}")));
                    section.AppendChild(new Run(new Text($"Гражданство: {candidate.Citizenship}")));
                    section.AppendChild(new Run(new Text($"Служба: {(candidate.HasServed ? "Да" : "Нет")}")));
                    section.AppendChild(new Run(new Text($"Номер военного билета: {candidate.MilitaryTicketNumber}")));

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
