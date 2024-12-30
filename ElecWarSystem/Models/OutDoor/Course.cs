using ElecWarSystem.Models.OutDoorDetails;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElecWarSystem.Models.OutDoor
{
    [Table("Course", Schema = "FileShare")]

    public class Course : Outdoor
    {
        public long CourseDetailsID { get; set; }
        [ForeignKey("CourseDetailsID")]
        public CourseDetails CourseDetails { get; set; }
        public void CleanNav()
        {
            Tmam = null;
        }

        public object Clone()
        {
            Course course = new Course();
            course.CourseDetailsID = this.CourseDetailsID;
            course.TmamID = this.TmamID;
            return course;
        }

        public bool IsDateLogic()
        {
            bool result = CourseDetails.DateFrom <= Tmam.Date &&
                                CourseDetails.DateTo >= Tmam.Date;
            return result;
        }
    }
}