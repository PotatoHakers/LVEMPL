using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace AutEmplAcc.Pages.View.Positions.HiringPlan
{
    public class AllPositionsExportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AllPositionsExportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Получаем все должности из базы данных
            var positions = await _context.Positions
                .Include(p => p.Employees)
                .ToListAsync();

            if (positions.Count == 0)
            {
                return NotFound();
            }

            // Генерируем отчет в Word
            var report = GenerateWordReport(positions);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Отчет_по_всем_должностям.docx");
        }

        private byte[] GenerateWordReport(List<AutEmplAcc.Models.Position> positions)
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
                    title.AppendChild(new Run(new Text("Отчет по планам набора по всем должностям")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // Цикл по всем должностям
                    foreach (var position in positions)
                    {
                        // Добавление раздела с данными о должности
                        var section = body.AppendChild(new Paragraph());
                        section.AppendChild(new Run(new Text($"Название должности: {position.Name}")));

                        // Добавление таблицы с данными
                        var table = body.AppendChild(new Table());
                        table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                        var row1 = table.AppendChild(new TableRow());
                        row1.AppendChild(new TableCell(new Paragraph(new Run(new Text("Ставка оплаты:")))));
                        row1.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.HourlyRate.ToString())))));

                        var row2 = table.AppendChild(new TableRow());
                        row2.AppendChild(new TableCell(new Paragraph(new Run(new Text("Цель набора:")))));
                        row2.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.HiringTarget.ToString())))));

                        var row3 = table.AppendChild(new TableRow());
                        row3.AppendChild(new TableCell(new Paragraph(new Run(new Text("Дата начала:")))));
                        row3.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.HiringStartDate.ToShortDateString())))));

                        var row4 = table.AppendChild(new TableRow());
                        row4.AppendChild(new TableCell(new Paragraph(new Run(new Text("Дата окончания:")))));
                        row4.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.HiringEndDate.ToShortDateString())))));

                        var row5 = table.AppendChild(new TableRow());
                        row5.AppendChild(new TableCell(new Paragraph(new Run(new Text("Текущее количество сотрудников:")))));
                        row5.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.Employees.Count.ToString())))));

                        var row6 = table.AppendChild(new TableRow());
                        row6.AppendChild(new TableCell(new Paragraph(new Run(new Text("Остаток от цели:")))));
                        row6.AppendChild(new TableCell(new Paragraph(new Run(new Text((position.HiringTarget - position.Employees.Count).ToString())))));

                        // Добавление разделителя между должностями
                        body.AppendChild(new Paragraph(new Run(new Break())));
                    }

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
