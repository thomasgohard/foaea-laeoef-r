using System;

namespace FOAEA3.Model
{
    public class PersonData
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Appl_Dbtr_Parent_SurNme_Birth { get; set; }
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public AddressData Address { get; set; }

        public int GetAge(DateTime asOf)
        {
            var today = DateTime.Today;

            // Calculate the age
            var age = asOf.Year - Birthdate.Year;

            // Go back to the year the person was born in case of a leap year or different month of the year
            if (Birthdate.Date > today.AddYears(-age))
                age--;

            return age;
        }

        public int GetAge() => GetAge(DateTime.Today);
    }
}
