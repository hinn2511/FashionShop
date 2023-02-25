using System;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if(dob.Date > today.AddYears(-age)) age --;
            return age;
        }

        public static DateTime SetToNow(this DateTime dateCreated)
        {
            return DateTime.UtcNow;
        }

        public static int GetQuarter(this int month)
        {
            var result = month / 3;
            result++;
            if (result <= 0)
                result = 1;
            if (result > 4)
                result = 4;
            return result;
        }

    }
}