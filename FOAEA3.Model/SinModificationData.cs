namespace FOAEA3.Model
{
    public class SinModificationData
    {
        public SinModificationData()
        {
                
        }

        public SinModificationData(string oldSIN, string newSIN)
        {
            OldSIN = oldSIN;
            NewSIN = newSIN;
        }

        public string OldSIN { get; set; }
        public string NewSIN { get; set; }
    }
}
