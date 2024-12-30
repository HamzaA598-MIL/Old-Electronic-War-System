using ElecWarSystem.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.ReportFactory
{
    public class OutOfCountriesReport : ReportGenerator<OutOfCountry>
    {
        private Dictionary<String, Dictionary<String, List<OutOfCountry>>> outOfCountriesList;
        public OutOfCountriesReport(Dictionary<String, Dictionary<String, List<OutOfCountry>>> outOfCountriesList,
            DateTime tmamDate, string title)
            : base(tmamDate, title, 29,4)
        {
            this.outOfCountriesList = outOfCountriesList;
        }

        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("م");
            this.CreateCell("الرتبة/الدرجة", 4);
            this.CreateCell("الإسم", 4);
            this.CreateCell("جهة السفر", 4);
            this.CreateCell("الغرض من السفر", 8);
            this.CreateCell("المدة من", 4);
            this.CreateCell("المدة إلى", 4);
        }

        protected override void CreateTableRow(int i, OutOfCountry outOfCountry)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            baseColor = BaseColor.WHITE;
            this.CreateCell($"{Utilites.numbersE2A(i.ToString())}");
            this.CreateCell(outOfCountry.OutOfCountryDetail.Person.Rank.RankName, 4);
            this.CreateCell(outOfCountry.OutOfCountryDetail.Person.FullName, 4);
            this.CreateCell(Utilites.numbersE2A(outOfCountry.OutOfCountryDetail.Country), 4);
            this.CreateCell(Utilites.numbersE2A(outOfCountry.OutOfCountryDetail.Puspose), 8);
            this.CreateCell(Utilites.numbersE2A(outOfCountry.OutOfCountryDetail.DateFrom.ToString("dd/MM/yyyy")), 4);
            this.CreateCell(Utilites.numbersE2A(outOfCountry.OutOfCountryDetail.DateTo.ToString("dd/MM/yyyy")), 4);

        }

        protected override void ReportBody()
        {
            this.CreateTableHead();
            int i = 1;
            foreach (var outOfCountrysPerZone in outOfCountriesList)
            {
                if (outOfCountrysPerZone.Value.Count > 0)
                {
                    this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {outOfCountrysPerZone.Key}",
                                    fontSize: 12f,
                                    fontStyle: Font.BOLD);
                    foreach (var outOfCountryPerUnit in outOfCountrysPerZone.Value)
                    {
                        if (outOfCountryPerUnit.Value.Count > 0)
                        {
                            this.CreateTitleWithBackgroundColor(Utilites.numbersE2A(outOfCountryPerUnit.Key),
                                    fontSize: 10f,
                                    fontStyle: Font.BOLD,
                                    align: Element.ALIGN_LEFT);
                            foreach (OutOfCountry outOfCountry in outOfCountryPerUnit.Value)
                            {
                                this.CreateTableRow(i, outOfCountry);
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}