using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace AutEmplAcc.Pages.Candidates
{
    public class AllEmployeeExportModel : PageModel
    {

        private readonly ApplicationDbContext _context;

        public AllEmployeeExportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Получаем всех сотрудников из базы данных
            var employees = await _context.Employees
                .Include(e => e.Candidate) // Включаем данные о кандидате
                .Include(e => e.Branch) // Включаем данные о филиале
                .Include(e => e.Position) // Включаем данные о должности
                .ToListAsync();

            if (employees.Count == 0)
            {
                return NotFound();
            }

            // Генерируем отчет в Word
            var report = GenerateWordReport(employees);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Отчет_по_всем_сотрудникам.docx");
        }

        private byte[] GenerateWordReport(List<AutEmplAcc.Models.Employee> employees)
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
                    title.AppendChild(new Run(new Text("Отчет по сотрудникам")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // Добавление таблицы
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // Добавление заголовков столбцов
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Имя")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Фамилия")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Отчество")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Филиал")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Должность")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Дата приема")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Ставка")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Оклад")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Опыт работы")))));

                    // Цикл по всем сотрудникам
                    foreach (var employee in employees)
                    {
                        // Добавление строки в таблицу
                        var dataRow = table.AppendChild(new TableRow());
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.Candidate.FirstName)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.Candidate.LastName)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.Candidate.Patronymic)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.Branch.Name)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.Position.Name)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.HiringDate.ToString("dd.MM.yyyy"))))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.Rate.ToString())))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.CalculateSalary().ToString())))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.WorkExperience.ToString())))));
                    }

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
