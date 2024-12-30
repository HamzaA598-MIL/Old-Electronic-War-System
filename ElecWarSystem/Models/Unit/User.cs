using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("Users", Schema = "FileShare")]
    public class User
    {
        [Key]
        [Display(Name = "ID")]
        public int ID { get; set; }
        [StringLength(45)]
        [Index(IsUnique = true)]
        public String UserName { get; set; }
        public string Password { get; set; }
        public bool isActive { get; set; } = false;
        public int UnitID { get; set; }
        [ForeignKey("UnitID")]
        public Unit Unit { get; set; }
        [Display(Name ="الصلاحيات")]
        public UserRoles Roles { get; set; }
    }
}