using ElecWarSystem.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.ReportFactory
{
    public class SickLeaveReport : ReportGenerator<SickLeave>
    {
        private Dictionary<String, Dictionary<String, List<SickLeave>>> sickLeaveReportData;
        public SickLeaveReport(Dictionary<String, Dictionary<String, List<SickLeave>>> sickLeaveReportData,
            DateTime tmamDate, string title) 
            : base(tmamDate, title, 19,4)
        {
            this.sickLeaveReportData = sickLeaveReportData;
        }

        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("م");
            this.CreateCell("الرتبة/الدرجة", 2);
            this.CreateCell("الإسم", 4);
            this.CreateCell("المستشفى", 2);
            this.CreateCell("تاريخ دخول المستشفى", 2);
            this.CreateCell("التشخيص",4);
            this.CreateCell("بدء الأجازة", 2);
            this.CreateCell("عودة الأجازة", 2);
        }

        protected override void CreateTableRow(int i, SickLeave sickLeave)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            baseColor = BaseColor.WHITE;
            this.CreateCell(Utilites.numbersE2A(i.ToString()));
            this.CreateCell(sickLeave.SickLeaveDetail.Person.Rank.RankName, 2);
            this.CreateCell(sickLeave.SickLeaveDetail.Person.FullName, 4);
            this.CreateCell(Utilites.numbersE2A(sickLeave.SickLeaveDetail.Hospital), 2);
            this.CreateCell(Utilites.numbersE2A(sickLeave.SickLeaveDetail.HospitalDate.ToString("dd/MM/yyyy")), 2);
            this.CreateCell(Utilites.numbersE2A(sickLeave.SickLeaveDetail.Diagnosis), 4);
            this.CreateCell(Utilites.numbersE2A(sickLeave.SickLeaveDetail.DateFrom.ToString("dd/MM/yyyy")), 2);
            this.CreateCell(Utilites.numbersE2A(sickLeave.SickLeaveDetail.DateTo.ToString("dd/MM/yyyy")), 2);
        }

        protected override void ReportBody()
        {
            this.CreateTableHead();
            int i = 1;
            foreach (var sickLeavesPerZone in sickLeaveReportData)
            {
                if (sickLeavesPerZone.Value.Count > 0)
                {
                    this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {sickLeavesPerZone.Key}",
                                    fontSize: 12f,
                                    fontStyle: Font.BOLD);
                    foreach (var sickLeavePerUnit in sickLeavesPerZone.Value)
                    {
                        if (sickLeavePerUnit.Value.Count > 0)
                        {
                            this.CreateTitleWithBackgroundColor(Utilites.numbersE2A(sickLeavePerUnit.Key),
                                    fontSize: 10f,
                                    fontStyle: Font.BOLD,
                                    align: Element.ALIGN_LEFT);
                            foreach (SickLeave sickLeave in sickLeavePerUnit.Value)
                            {
                                this.CreateTableRow(i, sickLeave);
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}