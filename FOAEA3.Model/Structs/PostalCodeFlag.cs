namespace FOAEA3.Model.Structs
{
    public readonly struct PostalCodeFlag
    {
        public bool IsPostalCodeValid { get; init; }
        public bool IsProvinceValid { get; init; }
        public bool IsCityNameValid { get; init; }
    }
}
