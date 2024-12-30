using ElecWarSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using Document = iTextSharp.text.Document;

namespace ElecWarSystem.ReportFactory
{
    public abstract class ReportGenerator<T>
    {
        private int Columns;
        private Document document;
        protected Font font;
        private PdfPTable pdfTable;
        private PdfPCell pdfPCell;
        private MemoryStream memoryStream = new MemoryStream();
        protected BaseColor baseColor;
        protected String title;
        protected DateTime tmamDate;
        public ReportGenerator(DateTime tmamDate, string title, int Columns,int headRow)
        {
            this.title = title;
            this.tmamDate = tmamDate;
            this.Columns = Columns;
            this.pdfTable = new PdfPTable(this.Columns);
            this.pdfTable.HeaderRows = headRow; 
        }
        //construct the paper properties ANd Paper Size
        public byte[] PrepareReport()
        {
            document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            document.SetPageSize(PageSize.A4);
            document.SetMargins(20f, 20f, 20f, 20f);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            pdfTable.WidthPercentage = 100;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            document.Open();
            float[] widths = new float[Columns];
            for (int i = 0; i < Columns; i++)
            {
                widths[i] = 50f;
            }
            pdfTable.SetWidths(widths);
            this.ReportHeader();
            this.ReportBody();
            //pdfTable.HeaderRows = 2;
            document.Add(pdfTable);
            document.Close();
            return memoryStream.ToArray();
        }

        private void ReportHeader()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD ^ Font.UNDERLINE);
            Phrase title = new Phrase("إدارة الحرب الإلكترونية\n مركز العمليات الرئيسى حرب إلكترونية", font);
            pdfPCell = new PdfPCell(title);
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.Colspan = this.Columns / 3;
            pdfPCell.Border = 0;
            pdfPCell.ExtraParagraphSpace = 0;
            pdfTable.AddCell(pdfPCell);

            // Get the absolute path to the image in the root of the project
            var rootPath = AppDomain.CurrentDomain.BaseDirectory; // Project root directory
            var imagePath = Path.Combine(rootPath, "ReportelecwarIcon.png");

            // Load the image from the relative path
            Image image = Image.GetInstance(imagePath);
            pdfPCell = new PdfPCell(image);
            pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            pdfPCell.Colspan = this.Columns - (this.Columns / 3);
            pdfPCell.Border = 0;
            pdfPCell.ExtraParagraphSpace = 0;
            pdfTable.AddCell(pdfPCell);

            this.CreateTitle($"تمام {this.title} لجميع وحدات الحرب الإلكترونية",
                fontSize: 16f,
                fontStyle: Font.BOLD ^ Font.UNDERLINE);

            String dateTitle = this.tmamDate.ToString("عن يوم dddd الموافق dd/MM/yyyy", new System.Globalization.CultureInfo("ar-AE"));
            this.CreateTitle($"{Utilites.numbersE2A(dateTitle)}",
                fontSize: 12f,
                fontStyle: Font.NORMAL);
        }

        protected void CreateTitle(String text, int border = 0, float fontSize = 10f, int fontStyle = Font.NORMAL, int align = Element.ALIGN_CENTER)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, fontSize, fontStyle);
            Phrase title = new Phrase(text, font);
            pdfPCell = new PdfPCell(title);
            pdfPCell.Colspan = Columns;
            pdfPCell.HorizontalAlignment = align;
            pdfPCell.Border = border;
            pdfPCell.ExtraParagraphSpace = 0;
            pdfTable.AddCell(pdfPCell);
            pdfTable.CompleteRow();
        }
        protected void CreateTitleWithBackgroundColor(String text, int border = 0, float fontSize = 10f, int fontStyle = Font.NORMAL, int align = Element.ALIGN_CENTER)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, fontSize, fontStyle);
            Phrase title = new Phrase(text, font);
            pdfPCell = new PdfPCell(title);
            pdfPCell.Colspan = Columns;
            pdfPCell.HorizontalAlignment = align;
            pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            pdfPCell.ExtraParagraphSpace = 5;
            pdfTable.AddCell(pdfPCell);
            pdfTable.CompleteRow();
        }
        protected void CreateCell(String text, int colSpan = 1, int rowSpan = 1)
        {
            Phrase pharse = new Phrase(text, font);
            pdfPCell = new PdfPCell(pharse);
            pdfPCell.Colspan = colSpan;
            pdfPCell.Rowspan = rowSpan;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.BackgroundColor = baseColor;
            pdfTable.AddCell(pdfPCell);
        }

        protected void CompleteTableRow()
        {
            this.pdfTable.CompleteRow(); 
        }

        protected virtual void ReportBody() { }
        protected virtual void CreateTableHead() { }
        protected virtual void CreateTableRow(int i, T obj) { }
    }
}