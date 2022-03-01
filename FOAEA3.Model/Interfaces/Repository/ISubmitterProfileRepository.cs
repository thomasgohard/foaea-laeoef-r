using FOAEA3.Model;
namespace FOAEA3.Model.Interfaces
{
    public interface ISubmitterProfileRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        SubmitterProfileData GetSubmitterProfile(string submitterCode);
    }
}
