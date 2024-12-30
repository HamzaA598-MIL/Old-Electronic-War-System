using ElecWarSystem.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElecWarSystem.ViewModel;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using ElecWarSystem.Serivces;
using System.Text;

namespace ElecWarSystem.ReportFactory
{
    public class LeadersTmamReport : ReportGenerator<Dictionary<String, LeaderTmamView>>
    {
        private Dictionary<String, Dictionary<String, Dictionary<String, LeaderTmamView>>> leadersTmamList;
        private Dictionary<String, Dictionary<String, Person>> altCommandors;
        private String zone, unit;
        public LeadersTmamReport(
            Dictionary<String, Dictionary<String, Dictionary<String, LeaderTmamView>>> leadersTmamList, 
            DateTime tmamDate, 
            string title)
            : base(tmamDate, title, 40,6)
        {
            this.leadersTmamList = leadersTmamList;
        }
        public void SetAltCommandors(Dictionary<String, Dictionary<String, Person>>  altCommandors)
        {
            this.altCommandors = altCommandors;
        }
        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("القائد", 4);
            this.CreateCell("ر.ع", 4);
            for (int i = 1; i <= 3; i++)
            {
                this.CreateCell($"قا ك{Utilites.numbersE2A(i.ToString())}", 4);
                this.CreateCell($"ر.ع ك{Utilites.numbersE2A(i.ToString())}", 4);
            }
            this.CreateCell("منوب عمليات", 8);

            for (int i = 1; i <= 8; i++)
            {
                this.CreateCell("التمام", 4);
            }
            this.CreateCell("", 8);

            for (int i = 1; i <= 8; i++)
            {
                this.CreateCell("من", 2);
                this.CreateCell("إلى", 2);
            }
            this.CreateCell("الرتبة", 2);
            this.CreateCell("الإسم", 6);
            CompleteTableRow(); 
        }

        protected override void CreateTableRow(int i, Dictionary<String, LeaderTmamView> leaderTmam)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            baseColor = BaseColor.WHITE;
            foreach (string key in leaderTmam.Keys)
            {
                this.CreateCell(leaderTmam[key].Tmam, 4);
            }
            for(int j = 0; j < 8 - leaderTmam.Keys.Count; j++)
            {
                this.CreateCell("-", 4);
            }
            String unitName = Utilites.numbersA2E(unit);
            try
            {
                // تحقق أولاً من أن altCommandors ليس null وأنه يحتوي على المفتاح المطلوب للـ zone
                if (altCommandors != null && altCommandors.ContainsKey(zone))
                {
                    // تحقق من أن altCommandors[zone] يحتوي على المفتاح المطلوب للـ unitName
                    if (altCommandors[zone].ContainsKey(unitName))
                    {
                        this.CreateCell(altCommandors[zone][unitName].Rank.RankName, 2);
                        this.CreateCell(altCommandors[zone][unitName].FullName, 6);
                    }
                    else
                    {
                        // إذا كان unitName غير موجود، استخدم قيمًا افتراضية
                        this.CreateCell("-", 2);
                        this.CreateCell("-", 6);
                    }
                }
                else
                {
                    // إذا كان zone غير موجود أو altCommandors يساوي null، استخدم قيمًا افتراضية
                    this.CreateCell("-", 2);
                    this.CreateCell("-", 6);
                }
            }
            catch (Exception e)
            {
                // في حالة وجود خطأ غير متوقع، استخدم قيمًا افتراضية
                this.CreateCell("-", 2);
                this.CreateCell("-", 6);
            }

            foreach (string key in leaderTmam.Keys)
            {
                if (leaderTmam[key].Tmam.Equals("موجود"))
                {
                    this.CreateCell("-", 2);
                    this.CreateCell("-", 2);
                }
                else
                {
                    String dateFrom = Utilites.numbersE2A(leaderTmam[key].OutdoorDetail.DateFrom.ToString("dd/MM"));
                    String dateTo = Utilites.numbersE2A(leaderTmam[key].OutdoorDetail.DateTo.ToString("dd/MM"));
                    this.CreateCell(dateFrom, 2);
                    this.CreateCell(dateTo, 2);
                }
            }
            for (int j = 0; j < 8 - leaderTmam.Keys.Count; j++)
            {
                this.CreateCell("-", 2);
                this.CreateCell("-", 2);
            }
            this.CreateCell("", 8);
        }
        
        
        
        protected override void ReportBody()
        {
            this.CreateTableHead();
            foreach (var leadersTmamPerZone in this.leadersTmamList)
            {
                zone = leadersTmamPerZone.Key;
                this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {zone}",
                                fontSize: 12f,
                                fontStyle: Font.BOLD);
                foreach (var leadersTmamPerUnit in leadersTmamPerZone.Value)
                {
                    unit = Utilites.numbersE2A(leadersTmamPerUnit.Key);
                    this.CreateTitleWithBackgroundColor($"{unit}",
                                fontSize: 10f,
                                fontStyle: Font.BOLD,
                                align: Element.ALIGN_LEFT);
                    this.CreateTableRow(0, leadersTmamPerUnit.Value);

                }
                
            }
        }
    }
}