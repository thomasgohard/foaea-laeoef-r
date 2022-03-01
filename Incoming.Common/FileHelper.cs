using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incoming.Common
{
    public static class FileHelper
    {
        public static string RemoveCycleFromFilename(string fileName)
        {
            int lastPeriod = fileName.LastIndexOf('.');
            if (lastPeriod > 0)
                return fileName.Substring(0, lastPeriod);
            else
                return fileName;
        }

        public static int GetCycleFromFilename(string fileName)
        {
            int lastPeriod = fileName.LastIndexOf('.');
            if (lastPeriod > 0)
                return int.TryParse(fileName[(lastPeriod + 1)..], out int cycle) ? cycle : -1;
            else
                return -1;
        }

    }
}
