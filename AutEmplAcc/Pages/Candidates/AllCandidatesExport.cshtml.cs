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
            // �������� ���� ���������� �� ���� ������
            var candidates = await _context.Candidates.ToListAsync();

            if (candidates.Count == 0)
            {
                return NotFound();
            }

            // ���������� ����� � Word
            var report = GenerateWordReport(candidates);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "�����_��_����_����������.docx");
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

                    // ���������� ���������
                    var title = body.AppendChild(new Paragraph());
                    title.AppendChild(new Run(new Text("����� �� ����������")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // ���������� �������
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // ���������� ���������� ��������
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("���")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("�������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("��������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("���� ��������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("����� ��������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("�����")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Email")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("�����������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("����� �������� ������")))));

                    // ���� �� ���� ����������
                    foreach (var candidate in candidates)
                    {
                        // ���������� ������ � �������
                        var dataRow = table.AppendChild(new TableRow());
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.FirstName)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.LastName)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.Patronymic)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.DateOfBirth.ToString("dd.MM.yyyy"))))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.PhoneNumber)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.Address)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.Email)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.Citizenship)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.HasServed ? "��" : "���")))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(candidate.MilitaryTicketNumber ?? "-")))));
                    }

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
