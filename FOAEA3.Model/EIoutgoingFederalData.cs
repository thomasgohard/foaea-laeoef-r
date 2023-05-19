namespace FOAEA3.Model
{
    public class EIoutgoingFederalData
    {
        public string Appl_Dbtr_Cnfrmd_SIN { get; set; }
        public string Dbtr_Id { get; set; }
        public string Appl_JusticeNrSfx { get; set; }
        public int Debt_Percentage { get; set; }
        public decimal Arrears_Balance { get; set; }
        public decimal FeeOwedTtl_Money { get; set; }
        public decimal? Debtor_Fixed_Amt { get; set; }
        public decimal? Amount_Per_Payment { get; set; }
        public string EnfOff_Fin_VndrCd { get; set; }
        public bool Fixed_Amt_Flag { get; set; }
    }
}
