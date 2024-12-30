using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElecWarSystem.Models;

namespace ElecWarSystem.ReportFactory
{
    public class PrisonsReport : ReportGenerator<Prison>
    {
        private Dictionary<String, Dictionary<String, List<Prison>>> prisonReportData;
        public PrisonsReport(Dictionary<String, Dictionary<String, List<Prison>>> prisonReportData,
            DateTime tmamDate, string title)
            : base(tmamDate, title, 23,4)
        {
            this.prisonReportData = prisonReportData;
        }

        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("م");
            this.CreateCell("الرتبة/الدرجة", 2);
            this.CreateCell("الإسم", 4);
            this.CreateCell("الجريمة", 3);
            this.CreateCell("العقوبة", 3);
            this.CreateCell("الآمر بالعقوبة", 2);
            this.CreateCell("المدة من", 2);
            this.CreateCell("المدة إلى", 2);
            this.CreateCell("بند الأوامر", 4);
        }

        protected override void CreateTableRow(int i, Prison prison)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            baseColor = BaseColor.WHITE;
            this.CreateCell(Utilites.numbersE2A(i.ToString()));
            this.CreateCell(prison.PrisonDetails.Person.Rank.RankName, 2);
            this.CreateCell(prison.PrisonDetails.Person.FullName, 4);
            this.CreateCell(Utilites.numbersE2A(prison.PrisonDetails.Crime), 3);
            this.CreateCell(Utilites.numbersE2A(prison.PrisonDetails.Punishment), 3);
            this.CreateCell(Utilites.numbersE2A(prison.PrisonDetails.Punisher), 2);
            this.CreateCell(Utilites.numbersE2A(prison.PrisonDetails.DateFrom.ToString("dd/MM/yyyy")), 2);
            this.CreateCell(Utilites.numbersE2A(prison.PrisonDetails.DateTo.ToString("dd/MM/yyyy")), 2);
            this.CreateCell(Utilites.numbersE2A(prison.PrisonDetails.CommandItem.Number.ToString()), 2);
            this.CreateCell(Utilites.numbersE2A(prison.PrisonDetails.CommandItem.Date.ToString("dd/MM/yyyy")), 2);
        }

        protected override void ReportBody()
        {
            this.CreateTableHead();
            int i = 1;
            foreach (var prisonsPerZone in prisonReportData)
            {
                if (prisonsPerZone.Value.Count > 0)
                {
                    this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {prisonsPerZone.Key}",
                                    fontSize: 12f,
                                    fontStyle: Font.BOLD);
                    foreach (var prisonPerUnit in prisonsPerZone.Value)
                    {
                        if (prisonPerUnit.Value.Count > 0)
                        {
                            this.CreateTitleWithBackgroundColor(Utilites.numbersE2A(prisonPerUnit.Key),
                                    fontSize: 10f,
                                    fontStyle: Font.BOLD,
                                    align: Element.ALIGN_LEFT);
                            foreach (Prison prison in prisonPerUnit.Value)
                            {
                                this.CreateTableRow(i, prison);
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}