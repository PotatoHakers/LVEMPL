using AutEmplAcc.DbContexts;
using AutEmplAcc.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace AutEmplAcc.Pages.View.Positions.HiringPlan
{
    public class ReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Models.Position Position { get; set; }
        public int CurrentEmployeesCount { get; set; }
        public int RemainingTarget { get; set; } // Остаток от цели набора
        public bool IsHiringCompleted { get; set; } // Флаг завершения набора

        public void OnGet()
        {
            Position = _context.Positions.Include(p => p.Employees).FirstOrDefault(p => p.Id == Id);

            if (Position == null)
            {
                return;
            }

            CurrentEmployeesCount = Position.Employees.Count;
            RemainingTarget = Position.HiringTarget - CurrentEmployeesCount;
            IsHiringCompleted = Position.HiringEndDate < DateTime.Now && CurrentEmployeesCount >= Position.HiringTarget;
        }
    }
}

//// Страница экспорта отчета в Word
//namespace AutEmplAcc.Pages.HiringPlan
//{
//    public class ExportModel : PageModel
//    {
//        private readonly ApplicationDbContext _context;

//        public ExportModel(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [BindProperty(SupportsGet = true)]
//        public int Id { get; set; }

//        public Models.Position Position { get; set; }

//        public async Task<IActionResult> OnGetAsync()
//        {
//            Position = await _context.Positions
//                .Include(p => p.Employees)
//                .FirstOrDefaultAsync(p => p.Id == Id);

//            if (Position == null)
//            {
//                return NotFound();
//            }

//            return Page();
//        }

//        public async Task<IActionResult> OnPostAsync()
//        {
//            Position = await _context.Positions
//                .Include(p => p.Employees)
//                .FirstOrDefaultAsync(p => p.Id == Id);

//            if (Position == null)
//            {
//                return NotFound();
//            }

//            var report = GenerateWordReport(Position);

//            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет_{Position.Name}.docx");
//        }

//        private byte[] GenerateWordReport(Models.Position position)
//        {
//            using (var memoryStream = new MemoryStream())
//            {
//                using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
//                {
//                    var document = wordDocument.AddMainDocumentPart();
//                    document.Document = new Document();

//                    // Добавление текста в документ
//                    var body = document.Document.AppendChild(new Body());
//                    var paragraph = body.AppendChild(new Paragraph());
//                    var run = paragraph.AppendChild(new Run());
//                    run.AppendChild(new Text("Отчет по плану набора"));

//                    // Добавление таблицы с данными
//                    var table = body.AppendChild(new Table());
//                    var tableProperties = table.AppendChild(new TableProperties());
//                    var tableStyle = tableProperties.AppendChild(new TableStyle());
//                    tableStyle.Val = "TableGrid";

//                    var tableRow = table.AppendChild(new TableRow());
//                    var tableCell1 = tableRow.AppendChild(new TableCell());
//                    var paragraph1 = tableCell1.AppendChild(new Paragraph());
//                    paragraph1.AppendChild(new Run(new Text("Название должности:")));

//                    var tableCell2 = tableRow.AppendChild(new TableCell());
//                    var paragraph2 = tableCell2.AppendChild(new Paragraph());
//                    paragraph2.AppendChild(new Run(new Text(position.Name)));

//                    // Добавление остальных данных в таблицу аналогично

//                    // Сохранение документа
//                    wordDocument.Save();
//                }

//                return memoryStream.ToArray();
//            }
//        }
//    }
//}
