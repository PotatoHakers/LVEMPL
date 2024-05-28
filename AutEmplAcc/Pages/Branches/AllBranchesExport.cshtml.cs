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
            // �������� ��� ������� �� ���� ������
            var branches = await _context.Branches.ToListAsync();

            if (branches.Count == 0)
            {
                return NotFound();
            }

            // ���������� ����� � Word
            var report = GenerateWordReport(branches);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "�����_��_����_��������.docx");
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

                    // ���������� ���������
                    var title = body.AppendChild(new Paragraph());
                    title.AppendChild(new Run(new Text("����� �� ��������")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // ���������� �������
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // ���������� ���������� ��������
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("�������� �������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("�����")))));

                    // ���� �� ���� ��������
                    foreach (var branch in branches)
                    {
                        // ���������� ������ � �������
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