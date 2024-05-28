using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutEmplAcc.Pages.Candidates
{
    public class AllCandidatesExportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AllCandidatesExportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Получаем всех кандидатов из базы данных
            var candidates = await _context.Candidates.ToListAsync();

            if (candidates.Count == 0)
            {
                return NotFound();
            }

            // Генерируем отчет в Word
            var report = GenerateWordReport(candidates);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Отчет_по_всем_кандидатам.docx");
        }

        private byte[] GenerateWordReport(List<AutEmplAcc.Models.Candidate> candidates)
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
                    title.AppendChild(new Run(new Text("Отчет по кандидатам")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // Добавление таблицы
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // Добавление заголовков столбцов
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Имя")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Фамилия")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Отчество")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Дата рождения")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Номер телефона")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Адрес")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Email")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Гражданство")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Служба")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Номер военного билета")))));

                    // Цикл по всем кандидатам
                    foreach (var candidate in candidates)
                    {
                        // Добавление строки в таблицу
                        var dataRow = table.AppendChild(new TableRow());
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.FirstName)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.LastName)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.Patronymic)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.DateOfBirth.ToString("dd.MM.yyyy"))))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.PhoneNumber)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.Address)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.Email)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.Citizenship)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.HasServed ? "Да" : "Нет")))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.MilitaryTicketNumber ?? "-")))));
                    }

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
