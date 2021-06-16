using System;

namespace Api.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dateOfBrith)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBrith.Year;
            if (dateOfBrith.Date >= today.AddYears(-age)) age--;
            return age;
        }
    }
}