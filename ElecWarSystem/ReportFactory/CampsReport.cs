using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElecWarSystem.Models;

namespace ElecWarSystem.ReportFactory
{
    public class CampsReport : ReportGenerator<Camp>
    {
        private Dictionary<String, Dictionary<String, List<Camp>>> CampsList;
        public CampsReport(Dictionary<String, Dictionary<String, List<Camp>>> CampsList,
            DateTime tmamDate, string title)
            : base(tmamDate, title, 33,4)
        {
            this.CampsList = CampsList;
        }

        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("م");
            this.CreateCell("الرتبة/الدرجة", 4);
            this.CreateCell("الإسم", 4);
            this.CreateCell("مكان التمركز الحالى", 8);
            this.CreateCell("السبب", 8);
            this.CreateCell("المدة من", 4);
            this.CreateCell("المدة إلى", 4);
        }

        protected override void CreateTableRow(int i, Camp Camp)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            baseColor = BaseColor.WHITE;
            this.CreateCell($"{Utilites.numbersE2A(i.ToString())}");
            this.CreateCell(Camp.CampDetail.Person.Rank.RankName, 4);
            this.CreateCell(Camp.CampDetail.Person.FullName, 4);
            this.CreateCell(Utilites.numbersE2A(Camp.CampDetail.CurrentExistance), 8);
            this.CreateCell(Utilites.numbersE2A(Camp.CampDetail.Reason), 8);
            this.CreateCell(Utilites.numbersE2A(Camp.CampDetail.DateFrom.ToString("dd/MM/yyyy")), 4);
            this.CreateCell(Utilites.numbersE2A(Camp.CampDetail.DateTo.ToString("dd/MM/yyyy")), 4);

        }

        protected override void ReportBody()
        {
            this.CreateTableHead();
            int i = 1;
            foreach (var CampsPerZone in CampsList)
            {
                if (CampsPerZone.Value.Count > 0)
                {
                    this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {CampsPerZone.Key}",
                                    fontSize: 12f,
                                    fontStyle: Font.BOLD);
                    foreach (var CampPerUnit in CampsPerZone.Value)
                    {
                        if (CampPerUnit.Value.Count > 0)
                        {
                            this.CreateTitleWithBackgroundColor(Utilites.numbersE2A(CampPerUnit.Key),
                                    fontSize: 10f,
                                    fontStyle: Font.BOLD,
                                    align: Element.ALIGN_LEFT);
                            foreach (Camp Camp in CampPerUnit.Value)
                            {
                                this.CreateTableRow(i, Camp);
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}