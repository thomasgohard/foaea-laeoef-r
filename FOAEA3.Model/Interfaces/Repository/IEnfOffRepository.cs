using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IEnfOffRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<EnfOffData> GetEnfOff(string enfOffName = null, string enfOffCode = null, string province = null, string enfServCode = null);
    }
}
