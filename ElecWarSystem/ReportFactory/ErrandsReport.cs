using ElecWarSystem.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.ReportFactory
{
    public class ErrandsReport : ReportGenerator<Errand>
    {
        private Dictionary<String, Dictionary<String, List<Errand>>> errandReportData;
        public ErrandsReport(Dictionary<String, Dictionary<String, List<Errand>>> errandReportData,
            DateTime tmamDate, string title)
            : base(tmamDate, title, 17,4)
        {
            this.errandReportData = errandReportData;
        }

        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("م");
            this.CreateCell("الرتبة", 2);
            this.CreateCell("الإسم", 4);
            this.CreateCell("جهة المأمورية", 3);
            this.CreateCell("الآمر بالمأمورية", 3);
            this.CreateCell("الفترة من", 2);
            this.CreateCell("الفترة إلى", 2);
        }

        protected override void CreateTableRow(int i, Errand errand)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            baseColor = BaseColor.WHITE;
            this.CreateCell($"{Utilites.numbersE2A(i.ToString())}");
            this.CreateCell(errand.ErrandDetail.Person.Rank.RankName, 2);
            this.CreateCell(errand.ErrandDetail.Person.FullName, 4);
            this.CreateCell(Utilites.numbersE2A(errand.ErrandDetail.ErrandPlace), 3);
            this.CreateCell(Utilites.numbersE2A(errand.ErrandDetail.ErrandCommandor), 3);
            this.CreateCell(Utilites.numbersE2A(errand.ErrandDetail.DateFrom.ToString("dd/MM/yyyy")), 2);
            this.CreateCell(Utilites.numbersE2A(errand.ErrandDetail.DateTo.ToString("dd/MM/yyyy")), 2);
        }

        protected override void ReportBody()
        {
            this.CreateTableHead();
            int i = 1;
            foreach (var errandsPerZone in errandReportData)
            {
                if (errandsPerZone.Value.Count > 0)
                {
                    this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {errandsPerZone.Key}",
                                    fontSize: 12f,
                                    fontStyle: Font.BOLD);
                    foreach (var errandPerUnit in errandsPerZone.Value)
                    {
                        if (errandPerUnit.Value.Count > 0)
                        {
                            this.CreateTitleWithBackgroundColor(Utilites.numbersE2A(errandPerUnit.Key),
                                    fontSize: 10f,
                                    fontStyle: Font.BOLD,
                                    align: Element.ALIGN_LEFT);
                            foreach (Errand errand in errandPerUnit.Value)
                            {
                                this.CreateTableRow(i, errand);
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}