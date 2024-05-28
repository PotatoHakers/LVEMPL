using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutEmplAcc.Pages.Candidates
{
    public class AllSickExportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AllSickExportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Получаем все больничные из базы данных
            var sickLeaves = await _context.SickLeaves
                .Include(sl => sl.Employee) // Включаем в запрос данные о сотруднике
                .ToListAsync();

            if (sickLeaves.Count == 0)
            {
                return NotFound();
            }

            // Генерируем отчет в Word
            var report = GenerateWordReport(sickLeaves);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Отчет_по_всем_больничным.docx");
        }

        private byte[] GenerateWordReport(List<AutEmplAcc.Models.SickLeave> sickLeaves)
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
                    title.AppendChild(new Run(new Text("Отчет по больничным листам")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // Добавление таблицы
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // Добавление заголовков столбцов
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Сотрудник")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Начало больничного")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Конец больничного")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Диагноз")))));

                    // Цикл по всем больничным
                    foreach (var sickLeave in sickLeaves)
                    {
                        // Добавление строки в таблицу
                        var dataRow = table.AppendChild(new TableRow());
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text($"{sickLeave.Employee.Candidate.FirstName} {sickLeave.Employee.Candidate.LastName}")))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(sickLeave.StartDate.ToString("dd.MM.yyyy"))))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(sickLeave.EndDate.ToString("dd.MM.yyyy"))))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(sickLeave.Diagnosis)))));
                    }

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
