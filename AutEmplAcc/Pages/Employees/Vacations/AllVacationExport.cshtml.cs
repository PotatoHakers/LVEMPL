using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutEmplAcc.Pages.Candidates
{
    public class AllVacationExportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AllVacationExportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Получаем все отпуска из базы данных
            var vacations = await _context.Vacations
                .Include(v => v.Employee) // Включаем в запрос данные о сотруднике
                .ToListAsync();

            if (vacations.Count == 0)
            {
                return NotFound();
            }

            // Генерируем отчет в Word
            var report = GenerateWordReport(vacations);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Отчет_по_всем_отпускам.docx");
        }

        private byte[] GenerateWordReport(List<AutEmplAcc.Models.Vacation> vacations)
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
                    title.AppendChild(new Run(new Text("Отчет по отпускам")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // Добавление таблицы
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // Добавление заголовков столбцов
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Сотрудник")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Начало отпуска")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Конец отпуска")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Тип отпуска")))));

                    // Цикл по всем отпускам
                    foreach (var vacation in vacations)
                    {
                        // Добавление строки в таблицу
                        var dataRow = table.AppendChild(new TableRow());
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text($"{vacation.Employee.Candidate.FirstName} {vacation.Employee.Candidate.LastName}")))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(vacation.StartDate.ToString("dd.MM.yyyy"))))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(vacation.EndDate.ToString("dd.MM.yyyy"))))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(vacation.Type)))));
                    }

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
