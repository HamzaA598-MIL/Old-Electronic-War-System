using ElecWarSystem.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElecWarSystem.Models.OutDoor;

namespace ElecWarSystem.ReportFactory
{
    public class CoursesReport : ReportGenerator<Course>
    {
        private Dictionary<String, Dictionary<String, List<Course>>> CoursesList;
        public CoursesReport(Dictionary<String, Dictionary<String, List<Course>>> CoursesList,
            DateTime tmamDate, string title)
            : base(tmamDate, title, 30,4)
        {
            this.CoursesList = CoursesList;
        }

        protected override void CreateTableHead()
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 10f, Font.BOLD);
            baseColor = BaseColor.LIGHT_GRAY;
            this.CreateCell("م");
            this.CreateCell("الرتبة/الدرجة", 2);
            this.CreateCell("الإسم", 4);
            this.CreateCell("الفرقة/الدورة", 6);
            this.CreateCell("مكان إنعقاد الفرقة/الدورة", 6);
            this.CreateCell("المدة من", 3);
            this.CreateCell("المدة إلى", 3);
            this.CreateCell("بند الأوامر", 5);

        }

        protected override void CreateTableRow(int i, Course Course)
        {
            font = FontFactory.GetFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, 8f, Font.NORMAL);
            baseColor = BaseColor.WHITE;
            this.CreateCell($"{Utilites.numbersE2A(i.ToString())}");
            this.CreateCell(Course.CourseDetails.Person.Rank.RankName, 2);
            this.CreateCell(Course.CourseDetails.Person.FullName, 4);
            this.CreateCell(Utilites.numbersE2A(Course.CourseDetails.CourseName), 6);
            this.CreateCell(Utilites.numbersE2A(Course.CourseDetails.CoursePlace), 6);
            this.CreateCell(Utilites.numbersE2A(Course.CourseDetails.DateFrom.ToString("dd/MM/yyyy")), 3);
            this.CreateCell(Utilites.numbersE2A(Course.CourseDetails.DateTo.ToString("dd/MM/yyyy")), 3);
            this.CreateCell(Utilites.numbersE2A(Course.CourseDetails.CommandItem.Number.ToString()), 2);
            this.CreateCell(Utilites.numbersE2A(Course.CourseDetails.CommandItem.Date.ToString("dd/MM/yyyy")), 3);

        }

        protected override void ReportBody()
        {
            this.CreateTableHead();
            int i = 1;
            foreach (var CoursesPerZone in CoursesList)
            {
                if (CoursesPerZone.Value.Count > 0)
                {
                    this.CreateTitleWithBackgroundColor($"وحدات فى نطاق {CoursesPerZone.Key}",
                                    fontSize: 12f,
                                    fontStyle: Font.BOLD);
                    foreach (var CoursePerUnit in CoursesPerZone.Value)
                    {
                        if (CoursePerUnit.Value.Count > 0)
                        {
                            this.CreateTitleWithBackgroundColor(Utilites.numbersE2A(CoursePerUnit.Key),
                                    fontSize: 10f,
                                    fontStyle: Font.BOLD,
                                    align: Element.ALIGN_LEFT);
                            foreach (Course Course in CoursePerUnit.Value)
                            {
                                this.CreateTableRow(i, Course);
                                i++;
                            }
                        }
                    }
                }
            }
        }
    }
}