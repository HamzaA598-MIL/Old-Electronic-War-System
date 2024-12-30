using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models.OutDoorDetails
{
    [Table("CourseDetails", Schema = "FileShare")]
    public class CourseDetails : OutdoorDetail
    {
        public String CourseName { get; set; } = String.Empty;
        public String CoursePlace { get; set; } = String.Empty;
        public long CommandItemID { get; set; }
        [ForeignKey("CommandItemID")]
        public CommandItem CommandItem { get; set; }

    }
}