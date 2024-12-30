using ElecWarSystem.Data;
using ElecWarSystem.Models;
using ElecWarSystem.Models.OutDoorDetails;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElecWarSystem.ViewModel
{
    public class LeaderTmamView
    {
        private long tmamID, personID;
        private TmamEnum status;
        private AppDBContext AppDBContext = new AppDBContext();
        private PersonService personService = new PersonService();
        private PersonStatusService PersonStatusService = new PersonStatusService();
        private Dictionary<int, string> statusToTmam = new Dictionary<int, string>()
        {
            { 0 , "موجود" },
            { 1 , "أجازة" },
            { 2 , "أجازة مرضي" },
            { 3 , "مأمورية" },
            { 4 , "سجن" },
            { 5 , "غياب" },
            { 6 , "مستشفى" },
            { 7 , "خ البلاد" },
            { 8 , "تدريب خارجى" },
            { 10 , "فرقة" },
        };

        public string Tmam { get; set; }
        public OutdoorDetail OutdoorDetail { get; set; }
        public LeaderTmamView(long TmamID, long PersonID)
        {
            this.tmamID = TmamID;
            this.personID = PersonID;

            this.status = PersonStatusService.getPersonStatus(this.tmamID, this.personID);//personService.GetStatus(personID);
            

            Tmam = statusToTmam[(int)this.status];
            
            OutdoorDetail = GetOutdoorDetail();
            
            OutdoorDetail.PersonID = this.personID;
        }
        private OutdoorDetail GetOutdoorDetail()
        {
            OutdoorDetail outdoorDetail = new OutdoorDetail();
            switch (this.status)
            {
                case TmamEnum.Exist:
                    outdoorDetail = new OutdoorDetail();

                    break;
                case TmamEnum.Vacation:
                    VacationDetail VacationDetails = AppDBContext.Vacations.Include("VacationDetail")
                        .FirstOrDefault(row => row.TmamID == this.tmamID && row.VacationDetail.PersonID == this.personID)
                        .VacationDetail;

                    Tmam = VacationDetails.VacationType;
                    outdoorDetail = VacationDetails;
                    break;
                case TmamEnum.SickLeave:
                    var sickLeave = AppDBContext.SickLeaves
                        .Include("SickLeaveDetail")
                        .FirstOrDefault(row => row.TmamID == this.tmamID && row.SickLeaveDetail.PersonID == this.personID);

                    if (sickLeave != null && sickLeave.SickLeaveDetail != null)
                    {
                        outdoorDetail = sickLeave.SickLeaveDetail;
                    }
                    else
                    {
                        // التعامل مع الحالة عندما لا يتم العثور على البيانات
                        // يمكنك إما تسجيل خطأ أو إعادة قيمة افتراضية أو القيام بأي إجراء آخر
                        outdoorDetail = null; // أو أي قيمة مناسبة
                    }
                    break;

                case TmamEnum.Errand:
                    outdoorDetail = AppDBContext.Errands.Include("ErrandDetail")
                        .FirstOrDefault(row => row.TmamID == this.tmamID && row.ErrandDetail.PersonID == this.personID)?
                        .ErrandDetail;
                    break;
                case TmamEnum.Hospital:
                    outdoorDetail = AppDBContext.Hospitals.Include("HospitalDetails")
                        .FirstOrDefault(row => row.TmamID == this.tmamID && row.HospitalDetails.PersonID == this.personID)
                        .HospitalDetails;
                    break;
                case TmamEnum.Prison:
                    outdoorDetail = AppDBContext.Prisons.Include("PrisonDetails")
                        .FirstOrDefault(row => row.TmamID == this.tmamID && row.PrisonDetails.PersonID == this.personID)
                        .PrisonDetails;
                    break;
                case TmamEnum.Absence:
                    outdoorDetail = AppDBContext.Absences.Include("AbsenceDetail")
                        .FirstOrDefault(row => row.TmamID == this.tmamID && row.AbsenceDetail.PersonID == this.personID)
                        .AbsenceDetail;
                    break;
                case TmamEnum.OutOfCountry:
                    outdoorDetail = AppDBContext.OutOfCountries.Include("OutOfCountryDetail")
                        .FirstOrDefault(row => row.TmamID == this.tmamID && row.OutOfCountryDetail.PersonID == this.personID)
                        .OutOfCountryDetail;
                    break;
                case TmamEnum.Camp:
                    outdoorDetail = AppDBContext.Camps.Include("CampDetail")
                        .FirstOrDefault(row => row.TmamID == this.tmamID && row.CampDetail.PersonID == this.personID)
                        .CampDetail;
                    break;
                case TmamEnum.Course:
                    CourseDetails CourseDetails = AppDBContext.Courses.Include("CourseDetails")
                        .FirstOrDefault(row => row.TmamID == this.tmamID && row.CourseDetails.PersonID == this.personID)?.CourseDetails;
                    Tmam = CourseDetails?.CourseName;
                    outdoorDetail = CourseDetails;
                    break;
                default:
                    outdoorDetail = new OutdoorDetail();
                    break;

            }
            return outdoorDetail ?? new OutdoorDetail();
        }
    }
}