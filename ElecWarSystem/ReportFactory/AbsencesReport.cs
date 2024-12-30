using ElecWarSystem.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.ReportFactory
{
    public class AbsencesReport : ReportGenerator<Absence>
    {
        //
        private Dictionary<String, Dictionary<String, List<Absence>>> absenceReportData;
        public AbsencesReport(Dictionary<String, Dictionary<String, List<Absence>>> absenceReportData,
            DateTime tmamDate, string title)
            : base(tmamDate, title, 17,4)
        {
            this.absenceReportData = absenceReportData;
        }

        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("م");
            this.CreateCell("الرتبة/الدرجة", 2);
            this.CreateCell("الإسم", 4);
            this.CreateCell("تاريخ الغياب", 4);
            this.CreateCell("دفعة الغياب", 2);
            this.CreateCell("بند الأوامر", 4);
        }
        
        //i sent from the for function and the object we wanna write to Pdf
        protected override void CreateTableRow(int i, Absence absence)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            baseColor = BaseColor.WHITE;
            this.CreateCell($"{Utilites.numbersE2A(i.ToString())}");
            this.CreateCell(absence.AbsenceDetail.Person.Rank.RankName, 2);
            this.CreateCell(absence.AbsenceDetail.Person.FullName, 4);
            this.CreateCell(Utilites.numbersE2A(absence.AbsenceDetail.DateFrom.ToString("dd/MM/yyyy")), 4);
            this.CreateCell(Utilites.numbersE2A(absence.AbsenceDetail.AbsenceTimes.ToString()), 2);
            this.CreateCell(Utilites.numbersE2A(absence.AbsenceDetail.commandItem.Number.ToString()), 2);
            this.CreateCell(Utilites.numbersE2A(absence.AbsenceDetail.commandItem.Date.ToString("dd/MM/yyyy")), 2);
        }

        protected override void ReportBody()
        {
            this.CreateTableHead();
            int i = 1;
            foreach (var absencesPerZone in absenceReportData)
            {
                if (absencesPerZone.Value.Count > 0)
                {
                    this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {absencesPerZone.Key}",
                                    fontSize: 12f,
                                    fontStyle: Font.BOLD);
                    foreach (var absencePerUnit in absencesPerZone.Value)
                    {
                        if (absencePerUnit.Value.Count > 0)
                        {
                            this.CreateTitleWithBackgroundColor(Utilites.numbersE2A(absencePerUnit.Key),
                                    fontSize: 10f,
                                    fontStyle: Font.BOLD,
                                    align: Element.ALIGN_LEFT);
                            foreach (Absence absence in absencePerUnit.Value)
                            {
                                this.CreateTableRow(i, absence);
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}