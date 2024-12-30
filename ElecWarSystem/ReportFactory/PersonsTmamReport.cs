using ElecWarSystem.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.ReportFactory
{
    public class PersonsTmamReport : ReportGenerator<TmamDetail>
    {
        private Dictionary<String, List<TmamDetail>> officerTmamList;
        public PersonsTmamReport(Dictionary<String, List<TmamDetail>> officerTmamList, DateTime tmamDate, string title)
            : base(tmamDate, title, 18,4)
        {
            this.officerTmamList = officerTmamList;
        }

        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("م");
            this.CreateCell("الوحدة", 4);
            this.CreateCell("القوة");
            this.CreateCell("موجود");
            this.CreateCell("خارج");
            this.CreateCell("أجازة");
            this.CreateCell("أجازة مرضية");
            this.CreateCell("فرقة");
            this.CreateCell("مأمورية");
            this.CreateCell("سجن");
            this.CreateCell("غياب");
            this.CreateCell("مستشفى");
            this.CreateCell("خ البلاد");
            this.CreateCell("م تد خارجى");
            this.CreateCell("نسبة الخوارج");
        }

        protected override void CreateTableRow(int i, TmamDetail tmamdetail)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.BOLD);
            this.CreateCell(Utilites.numbersE2A(i.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.Tmam.Unit.UnitName), 4);
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            this.CreateCell(Utilites.numbersE2A(tmamdetail.totalPower.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.GetExisting().ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.GetOutting().ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.vacation.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.sickLeave.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.course.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.errand.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.prison.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.absence.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.hospital.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.outOfCountry.ToString()));
            this.CreateCell(Utilites.numbersE2A(tmamdetail.outdoorCamp.ToString()));
            this.CreateCell($"{Utilites.numbersE2A(tmamdetail.GetOuttingPrecetage().ToString())}%");
        }

        protected override void ReportBody()
        {
            this.CreateTableHead();
            int i = 1;
            foreach (var officersTmamInZone in this.officerTmamList)
            {
                this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {officersTmamInZone.Key}",
                    fontSize: 12f,
                    fontStyle: Font.BOLD);

                font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
                baseColor = BaseColor.WHITE;
                foreach (TmamDetail tmamDetail in officersTmamInZone.Value)
                {
                    this.CreateTableRow(i, tmamDetail);
                    i++;
                }
            }
        }
    }
}