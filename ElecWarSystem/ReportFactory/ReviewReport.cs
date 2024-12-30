using ElecWarSystem.Models;
using ElecWarSystem.ViewModel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElecWarSystem.ReportFactory
{
    public class ReviewReport : ReportGenerator<Tmam>
    {
        private readonly Tmam tmam;
        private readonly Dictionary<string, LeaderTmamView> leaderTmam;
        private DateTime date;
        private readonly string title;
        public ReviewReport(Tmam tmam, Dictionary<string, LeaderTmamView> leaderTmam, DateTime date, string title)
        : base(date, title, 40,6)
        {
            this.tmam = tmam;
            this.leaderTmam = leaderTmam;
            this.date = date;
            this.title = title;
        }

        protected override void CreateTableHead()
        {
            int numOfSegmants = 1 + tmam.Unit.SmallUnits.Count;
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("القائد", 4);
            this.CreateCell("ر.ع", 4);
            for (int i = 1; i <= numOfSegmants - 1; i++)
            {
                this.CreateCell($"قا ك{Utilites.numbersE2A(i.ToString())}", 4);
                this.CreateCell($"ر.ع ك{Utilites.numbersE2A(i.ToString())}", 4);
            }
            this.CreateCell("منوب عمليات", 8);

            for (int i = 1; i <= numOfSegmants * 2; i++)
            {
                this.CreateCell("التمام", 4);
            }
            this.CreateCell("", 8);

            for (int i = 1; i <= numOfSegmants * 2; i++)
            {
                this.CreateCell("من", 2);
                this.CreateCell("إلى", 2);
            }
            this.CreateCell("الرتبة", 2);
            this.CreateCell("الإسم", 6);
        }

        protected override void CreateTableRow(int i, Tmam obj)
        {
            base.CreateTableRow(i, obj);
        }

        protected override void ReportBody()
        {
            base.ReportBody();
        }
    }
}