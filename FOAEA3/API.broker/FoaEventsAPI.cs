using FOAEA3.Data.Base;
using FOAEA3.Model;

namespace FOAEA3.API.broker
{
    public class FoaEventsAPI : BaseAPI
    {
        internal FoaEventDataDictionary GetFoaEvents()
        {
            var result = GetDataAsync<FoaEventDataDictionary>($"api/v1/FoaEvents").Result;

            if (result.Messages.Count > 0)
                ReferenceData.Instance().Messages.AddRange(result.Messages);

            return result;

        }
    }
}
