using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutEmplAcc.Pages.View.Positions.HiringPlan
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

        public AutEmplAcc.Models.Position Position { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Position = await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == Id);

            if (Position == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Position = await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == Id);

            if (Position == null)
            {
                return NotFound();
            }

            var report = GenerateWordReport(Position);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"�����_{Position.Name}.docx");
        }

        private byte[] GenerateWordReport(AutEmplAcc.Models.Position position)
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
                    title.AppendChild(new Run(new Text("����� �� ����� ������")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // ���������� ������� � �������
                    var section = body.AppendChild(new Paragraph());
                    section.AppendChild(new Run(new Text($"�������� ���������: {position.Name}")));

                    // ���������� ������� � �������
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    var row1 = table.AppendChild(new TableRow());
                    row1.AppendChild(new TableCell(new Paragraph(new Run(new Text("������ ������:")))));
                    row1.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.HourlyRate.ToString())))));

                    var row2 = table.AppendChild(new TableRow());
                    row2.AppendChild(new TableCell(new Paragraph(new Run(new Text("���� ������:")))));
                    row2.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.HiringTarget.ToString())))));

                    var row3 = table.AppendChild(new TableRow());
                    row3.AppendChild(new TableCell(new Paragraph(new Run(new Text("���� ������:")))));
                    row3.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.HiringStartDate.ToShortDateString())))));

                    var row4 = table.AppendChild(new TableRow());
                    row4.AppendChild(new TableCell(new Paragraph(new Run(new Text("���� ���������:")))));
                    row4.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.HiringEndDate.ToShortDateString())))));

                    var row5 = table.AppendChild(new TableRow());
                    row5.AppendChild(new TableCell(new Paragraph(new Run(new Text("������� ���������� �����������:")))));
                    row5.AppendChild(new TableCell(new Paragraph(new Run(new Text(position.Employees.Count.ToString())))));

                    var row6 = table.AppendChild(new TableRow());
                    row6.AppendChild(new TableCell(new Paragraph(new Run(new Text("������� �� ����:")))));
                    row6.AppendChild(new TableCell(new Paragraph(new Run(new Text((position.HiringTarget - position.Employees.Count).ToString())))));

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
