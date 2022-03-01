using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Helpers
{
    static public class SubjectHelper
    {
        static public bool IsPasswordExpired(SubjectData subject)
        {
            if (subject.PasswordExpiryDate.HasValue)
            {
                if (subject.PasswordExpiryDate.Value.Date > DateTime.Now.Date)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
