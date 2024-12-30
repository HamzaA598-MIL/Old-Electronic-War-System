using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace ElecWarSystem.Models
{
    [Table("Unit", Schema = "FileShare")]
    public class Unit
    {
        [Key]
        [Display(Name = "الرقم التعريفي")] // اسم مخصص
        public int ID { get; set; }

        [Display(Name = " المنطقة")] // اسم مخصص
        public int zoneID { get; set; }

        [ForeignKey("zoneID")]
        public Zone zone { get; set; }

        [StringLength(450)]
        [Index(IsUnique = true)]
        [Display(Name = "اسم الوحدة")] // اسم مخصص
        public string UnitName { get; set; }

        [Display(Name = "حجم التخزين المسموح")] // اسم مخصص
        public long AllowedStrogeSize { get; set; } = 5368709120;

        [Display(Name = "حجم التخزين المستخدم")] // اسم مخصص
        public long UsedStrogeSize { get; set; } = 0;

        [Display(Name = "الترتيب")] // اسم مخصص
        public int Order { get; set; } = 0;

        [Display(Name = "هل يوجد قائد بديل؟")] // اسم مخصص
        public bool AltComExist { get; set; } = true;

        [Display(Name = "معرف قائد الوحدة")] // اسم مخصص
        public long? UCID { get; set; }

        [ForeignKey("UCID")]
        [Display(Name = "قائد الوحدة")] // اسم مخصص
        public Person UnitCommandor { get; set; }

        [Display(Name = "معرف رئيس العمليات")] // اسم مخصص
        public long? UOCHID { get; set; }

        [ForeignKey("UOCHID")]
        [Display(Name = "رئيس العمليات")] // اسم مخصص
        public Person UnitOperationsChief { get; set; }

        [Display(Name = "الوحدات الصغيرة")] // اسم مخصص
        public List<SmallUnit> SmallUnits { get; set; }
    }
}
