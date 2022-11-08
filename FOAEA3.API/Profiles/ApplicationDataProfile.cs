using AutoMapper;
using FOAEA3.API.Model;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;

namespace FOAEA3.API.Profiles
{
    public class ApplicationDataProfile : Profile
    {
        public ApplicationDataProfile()
        {
            var applLifeStates = ReferenceData.Instance().ApplicationLifeStates;

            CreateMap<ApplicationData, ApplicationDataFriendly>()
                .ForMember(dest => dest.EnforcementServiceCode, opt => opt.MapFrom(src => src.Appl_EnfSrv_Cd.Trim()))
                .ForMember(dest => dest.ControlCode, opt => opt.MapFrom(src => src.Appl_CtrlCd.Trim()))
                .ForMember(dest => dest.SourceReferenceNumber, opt => opt.MapFrom(src => src.Appl_Source_RfrNr.Trim()))
                .ForMember(dest => dest.JusticeNumber, opt => opt.MapFrom(src => src.Appl_JusticeNr.Trim()))
                .ForMember(dest => dest.Submitter, opt => opt.MapFrom(src => src.Subm_SubmCd.Trim()))
                .ForMember(dest => dest.RecipientSubmitter, opt => opt.MapFrom(src => src.Subm_Recpt_SubmCd.Trim()))
                .ForMember(dest => dest.FormReceiptDate, opt => opt.MapFrom(src => src.Appl_Rcptfrm_Dte.ToString(DateTimeExtensions.FOAEA_DATE_FORMAT)))
                .ForMember(dest => dest.LegalDate, opt => opt.MapFrom(src => src.Appl_Lgl_Dte.ToString(DateTimeExtensions.FOAEA_DATE_FORMAT)))
                .ForMember(dest => dest.CreditorDateOfBirth, opt => opt.MapFrom(src => src.Appl_Crdtr_Brth_Dte.HasValue ? src.Appl_Crdtr_Brth_Dte.Value.ToString(DateTimeExtensions.FOAEA_DATE_FORMAT) : ""))
                .ForMember(dest => dest.DebtorDateOfBirth, opt => opt.MapFrom(src => src.Appl_Dbtr_Brth_Dte.HasValue ? src.Appl_Dbtr_Brth_Dte.Value.ToString(DateTimeExtensions.FOAEA_DATE_FORMAT) : ""))
                .ForMember(dest => dest.ApplicationLifeState, opt => opt.MapFrom(src => applLifeStates[src.AppLiSt_Cd].Description.Trim()))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Appl_CommSubm_Text ?? ""))
                .ForMember(dest => dest.AffidavitSubmitter, opt => opt.MapFrom(src => src.Subm_Affdvt_SubmCd ?? ""))
                .ForMember(dest => dest.AffidavitReceivedDate, opt => opt.MapFrom(src => src.Appl_RecvAffdvt_Dte.HasValue ? src.Appl_RecvAffdvt_Dte.Value.ToString(DateTimeExtensions.FOAEA_DATE_FORMAT) : ""))
                .ForMember(dest => dest.CreditorName, opt => opt.MapFrom(src => FormatName(src.Appl_Crdtr_FrstNme, src.Appl_Crdtr_MddleNme, src.Appl_Crdtr_SurNme)))
                .ForMember(dest => dest.DebtorName, opt => opt.MapFrom(src => FormatName(src.Appl_Dbtr_FrstNme, src.Appl_Dbtr_MddleNme, src.Appl_Dbtr_SurNme)))
                .ForMember(dest => dest.DebtorAddress, opt => opt.MapFrom(src => FormatAddress(src.Appl_Dbtr_Addr_Ln, src.Appl_Dbtr_Addr_Ln1, src.Appl_Dbtr_Addr_CityNme, src.Appl_Dbtr_Addr_PrvCd, src.Appl_Dbtr_Addr_CtryCd, src.Appl_Dbtr_Addr_PCd)))
                .ForMember(dest => dest.DebtorParentSurname, opt => opt.MapFrom(src => src.Appl_Dbtr_Parent_SurNme_Birth ?? ""))
                .ForMember(dest => dest.DebtorLanguage, opt => opt.MapFrom(src => src.Appl_Dbtr_LngCd))
                .ForMember(dest => dest.DebtorGender, opt => opt.MapFrom(src => src.Appl_Dbtr_Gendr_Cd))
                .ForMember(dest => dest.DebtorEnteredSIN, opt => opt.MapFrom(src => src.Appl_Dbtr_Entrd_SIN ?? ""))
                .ForMember(dest => dest.DebtorConfirmedSIN, opt => opt.MapFrom(src => src.Appl_Dbtr_Cnfrmd_SIN ?? ""))
                .ForMember(dest => dest.ApplicationCategory, opt => opt.MapFrom(src => src.AppCtgy_Cd))
                .ForMember(dest => dest.ActiveState, opt => opt.MapFrom(src => src.ActvSt_Cd))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Appl_Create_Dte.ToString(DateTimeExtensions.FOAEA_DATE_FORMAT) + " [" + src.Appl_Create_Usr.Trim() + "]"))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.Appl_LastUpdate_Dte.HasValue ? src.Appl_LastUpdate_Dte.Value.ToString(DateTimeExtensions.FOAEA_DATE_FORMAT) + " [" + src.Appl_LastUpdate_Usr.Trim() + "]" : ""))
                ;
        }

        private static string FormatName(string firstName, string middleName, string lastName)
        {
            string result = lastName.Trim();
            if (!string.IsNullOrEmpty(firstName))
            {
                result += ", " + firstName.Trim();
                if (!string.IsNullOrEmpty(middleName))
                    result += " " + middleName.Trim();
            }

            return result;
        }

        private static string FormatAddress(string line1, string line2, string cityName, string provinceCode,
                                            string countryCode, string postalCode)
        {
            string result = line1.Trim();

            if (!string.IsNullOrEmpty(line2))
                result += ", " + line2.Trim();

            result += $", {cityName.Trim()} {provinceCode.Trim()}, {countryCode.Trim()}, {postalCode.Trim()}";

            return result;
        }
    }
}
