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
            // �������� ��� ���������� �� ���� ������
            var sickLeaves = await _context.SickLeaves
                .Include(sl => sl.Employee) // �������� � ������ ������ � ����������
                .ToListAsync();

            if (sickLeaves.Count == 0)
            {
                return NotFound();
            }

            // ���������� ����� � Word
            var report = GenerateWordReport(sickLeaves);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "�����_��_����_����������.docx");
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

                    // ���������� ���������
                    var title = body.AppendChild(new Paragraph());
                    title.AppendChild(new Run(new Text("����� �� ���������� ������")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // ���������� �������
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // ���������� ���������� ��������
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("���������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("������ �����������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("����� �����������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("�������")))));

                    // ���� �� ���� ����������
                    foreach (var sickLeave in sickLeaves)
                    {
                        // ���������� ������ � �������
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
