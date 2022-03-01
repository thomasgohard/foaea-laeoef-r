using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;

namespace FOAEA3.API.broker
{
    public class ApplicationCommentsAPI : BaseAPI
    {
        internal DataList<ApplicationCommentsData> GetApplicationComments()
        {
            var data = GetDataAsync<DataList<ApplicationCommentsData>>($"api/v1/applicationComments").Result;

            if (data.Messages.Count > 0)
                ReferenceData.Instance().Messages.AddRange(data.Messages);

            return data;
        }
    }
}
