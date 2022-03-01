using System;
using System.Collections.Generic;
using System.Text;

namespace FileBroker.Model.Interfaces
{
    public interface IFlatFileSpecificationRepository
    {
        List<FlatFileSpecificationData> GetFlatFileSpecificationsForFile(int processId);
    }
}
