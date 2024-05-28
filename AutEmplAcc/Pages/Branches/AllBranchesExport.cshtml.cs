using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace AutEmplAcc.Pages.Branches
{
    public class AllBranchesExportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AllBranchesExportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Получаем все филиалы из базы данных
            var branches = await _context.Branches.ToListAsync();

            if (branches.Count == 0)
            {
                return NotFound();
            }

            // Генерируем отчет в Word
            var report = GenerateWordReport(branches);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Отчет_по_всем_филиалам.docx");
        }

        private byte[] GenerateWordReport(List<AutEmplAcc.Models.Branch> branches)
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
                    title.AppendChild(new Run(new Text("Отчет по филиалам")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // Добавление таблицы
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // Добавление заголовков столбцов
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Название филиала")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Адрес")))));

                    // Цикл по всем филиалам
                    foreach (var branch in branches)
                    {
                        // Добавление строки в таблицу
                        var dataRow = table.AppendChild(new TableRow());
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(branch.Name)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(branch.Address)))));
                    }

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}