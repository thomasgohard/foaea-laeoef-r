namespace FOAEA3.Common.Helpers
{
    public class FormHelper
    {
        public static string ConvertTaxFormFullNameToAbbreviation(string formName)
        {
            return formName.ToUpper() switch
            {
                "SCHEDULE 1" => "S1",
                "SCHEDULE 2" => "S2",
                "SCHEDULE 3" => "S3",
                "SCHEDULE 4" => "S4",
                "SCHEDULE 5" => "S5",
                "SCHEDULE 6" => "S6",
                "SCHEDULE 7" => "S7",
                "SCHEDULE 8" => "S8",
                "SCHEDULE 9" => "S9",
                "SCHEDULE 10" => "S10",
                "SCHEDULE 11" => "S11",
                "SCHEDULE 12" => "S12",
                "SCHEDULE 13" => "S13",
                "SCHEDULE 14" => "S14",
                "SCHEDULE 1A" => "S1A",
                "SCHEDULE SA" => "SA",
                "SCHEDULE SB" => "SB",
                "SCHEDULE SC" => "SC",
                _ => formName,
            };
        }
    }
}
