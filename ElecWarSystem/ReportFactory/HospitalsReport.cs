using ElecWarSystem.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.ReportFactory
{
    public class HospitalsReport : ReportGenerator<Hospital>
    {
        private Dictionary<String, Dictionary<String, List<Hospital>>> hospitalReportData;
        public HospitalsReport(Dictionary<String, Dictionary<String, List<Hospital>>> hospitalReportData,
            DateTime tmamDate, string title)
            : base(tmamDate, title, 17,4)
        {
            this.hospitalReportData = hospitalReportData;
        }

        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("م");
            this.CreateCell("الرتبة/الدرجة", 2);
            this.CreateCell("الإسم", 4);
            this.CreateCell("إسم المستشفى", 2);
            this.CreateCell("تاريخ دخول المستشف", 2);
            this.CreateCell("التشخيص", 3);
            this.CreateCell("التوصيات الممنوحة", 3);
        }

        protected override void CreateTableRow(int i, Hospital hospital)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            baseColor = BaseColor.WHITE;
            this.CreateCell($"{Utilites.numbersE2A(i.ToString())}");
            this.CreateCell(hospital.HospitalDetails.Person.Rank.RankName, 2);
            this.CreateCell(hospital.HospitalDetails.Person.FullName, 4);
            this.CreateCell(Utilites.numbersE2A(hospital.HospitalDetails.Hospital), 2);
            this.CreateCell(Utilites.numbersE2A(hospital.HospitalDetails.DateFrom.ToString("dd/MM/yyyy")), 2);
            this.CreateCell(Utilites.numbersE2A(hospital.HospitalDetails.Diagnosis), 3);
            this.CreateCell(Utilites.numbersE2A(hospital.HospitalDetails.Recommendations), 3);
        }

        protected override void ReportBody()
        {
            this.CreateTableHead();
            int i = 1;
            foreach (var hospitalsPerZone in hospitalReportData)
            {
                if (hospitalsPerZone.Value.Count > 0)
                {
                    this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {hospitalsPerZone.Key}",
                                    fontSize: 12f,
                                    fontStyle: Font.BOLD);
                    foreach (var hospitalPerUnit in hospitalsPerZone.Value)
                    {
                        if (hospitalPerUnit.Value.Count > 0)
                        {
                            this.CreateTitleWithBackgroundColor(Utilites.numbersE2A(hospitalPerUnit.Key),
                                    fontSize: 10f,
                                    fontStyle: Font.BOLD,
                                    align: Element.ALIGN_LEFT);
                            foreach (Hospital hospital in hospitalPerUnit.Value)
                            {
                                this.CreateTableRow(i, hospital);
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}