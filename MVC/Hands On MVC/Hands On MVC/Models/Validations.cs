using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hands_On_MVC.Models
{
    public class DOBRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime dob)
            {
                var today = DateTime.Today;
                var age = today.Year - dob.Year;
                if (dob > today.AddYears(-age)) age--;
                return age >= 21 && age < 25;
            }
            return false;
        }
    }

    public class DOJValidAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime doj)
            {
                return doj <= DateTime.Today;
            }
            return false;
        }
    }

    public class PasswordFormatAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string password)
            {
                return Regex.IsMatch(password, @"^[A-Z][0-9].{5}$");
            }
            return false;
        }
    }
}
