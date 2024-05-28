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
            // �������� ��� ������� �� ���� ������
            var vacations = await _context.Vacations
                .Include(v => v.Employee) // �������� � ������ ������ � ����������
                .ToListAsync();

            if (vacations.Count == 0)
            {
                return NotFound();
            }

            // ���������� ����� � Word
            var report = GenerateWordReport(vacations);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "�����_��_����_��������.docx");
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

                    // ���������� ���������
                    var title = body.AppendChild(new Paragraph());
                    title.AppendChild(new Run(new Text("����� �� ��������")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // ���������� �������
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // ���������� ���������� ��������
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("���������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("������ �������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("����� �������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("��� �������")))));

                    // ���� �� ���� ��������
                    foreach (var vacation in vacations)
                    {
                        // ���������� ������ � �������
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
