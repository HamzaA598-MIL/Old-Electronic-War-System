using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElecWarSystem.Models
{
    [Table("HospitalDetails", Schema = "FileShare")]
    public class HospitalDetails : OutdoorDetail
    {
        public String Hospital { get; set; } = String.Empty;
        public String Diagnosis { get; set; } = String.Empty;
        public String Recommendations { get; set; } = String.Empty;
        
        public static bool operator ==(HospitalDetails hospitalDetails1 , HospitalDetails hospitalDetails2)
        {
            bool isEqual = false;
            isEqual = ((hospitalDetails1.PersonID == hospitalDetails2.PersonID) &&
                            (hospitalDetails1.DateFrom == hospitalDetails2.DateFrom));
            
            return isEqual;
        }

        public static bool operator !=(HospitalDetails hospitalDetails1, HospitalDetails hospitalDetails2)
        {
            bool isEqual = false;
            isEqual = ((hospitalDetails1.PersonID != hospitalDetails2.PersonID) &&
                            (hospitalDetails1.DateFrom != hospitalDetails2.DateFrom));

            return isEqual;
        }
    }
}