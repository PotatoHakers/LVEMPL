using AutEmplAcc.DbContexts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutEmplAcc.Pages.Branches
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

        public AutEmplAcc.Models.Branch Branch { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Branch = await _context.Branches
                .Include(b => b.Employees) // �������� ����������� ��� ������
                .FirstOrDefaultAsync(b => b.Id == Id);

            if (Branch == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Branch = await _context.Branches
                .Include(b => b.Employees)
                .FirstOrDefaultAsync(b => b.Id == Id);

            if (Branch == null)
            {
                return NotFound();
            }

            var report = GenerateWordReport(Branch);

            return File(report, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"�����_{Branch.Name}.docx");
        }

        private byte[] GenerateWordReport(AutEmplAcc.Models.Branch branch)
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
                    title.AppendChild(new Run(new Text("����� �� ����� ������ ��� �������")));
                    title.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" });

                    // ���������� ������� � �������
                    var section = body.AppendChild(new Paragraph());
                    section.AppendChild(new Run(new Text($"�������� �������: {branch.Name}")));
                    section.AppendChild(new Run(new Text($"�����: {branch.Address}")));

                    // ���������� ������� � ������� �� �����������
                    var table = body.AppendChild(new Table());
                    table.AppendChild(new TableProperties(new TableStyle { Val = "TableGrid" }));

                    // ��������� ��������
                    var headerRow = table.AppendChild(new TableRow());
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("���")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("�������")))));
                    headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("���������")))));

                    // ���� �� ����������� �������
                    foreach (var employee in branch.Employees)
                    {
                        var dataRow = table.AppendChild(new TableRow());
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.Candidate.FirstName)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.Candidate.LastName)))));
                        dataRow.AppendChild(new TableCell(new Paragraph(new Run(new Text(employee.Position.Name)))));
                    }

                    wordDocument.Save();
                }

                return memoryStream.ToArray();
            }
        }
    }
}
