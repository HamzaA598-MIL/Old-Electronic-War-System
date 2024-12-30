using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ElecWarSystem.Models
{
    public class Utilites
    {
        public static string numbersE2A(string text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            String[] arabicNumbers = new String[] { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
            foreach (char c in text)
            { 
                if (Char.IsNumber(c) && !arabicNumbers.Contains(c.ToString()))
                {
                    stringBuilder.Append(arabicNumbers[int.Parse(c.ToString())].ToString());
                }
                else
                {
                    stringBuilder.Append(c.ToString());
                }
            }
            return stringBuilder.ToString();
        }
        public static string numbersA2E(string text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<String>  arabicNumbers = new List<String>() { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
            foreach (char c in text)
            {
                if (arabicNumbers.Contains(c.ToString()))
                {
                    int i = arabicNumbers.IndexOf(c.ToString());
                    stringBuilder.Append(i.ToString());
                }
                else
                {
                    stringBuilder.Append(c.ToString());
                }
            }
            return stringBuilder.ToString();
        }
    }
}