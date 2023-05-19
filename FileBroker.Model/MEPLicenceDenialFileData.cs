using System.Collections.Generic;

namespace FileBroker.Model
{
    public struct MEPLicenceDenial_RecType01
    {
        public string RecType;
        public string Cycle;
        public string FileDate;
        public string TermsAccepted;
    }

    public struct MEPLicenceDenial_RecTypeBase
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;
        public string dat_Appl_Source_RfrNr;
        public string dat_Appl_EnfSrvCd;
        public string dat_Subm_Rcpt_SubmCd;
        public string dat_Appl_Lgl_Dte;
        public string dat_Appl_Dbtr_SurNme;
        public string dat_Appl_Dbtr_FrstNme;
        public string dat_Appl_Dbtr_MddleNme;
        public string dat_Appl_Dbtr_Brth_Dte;
        public string dat_Appl_Dbtr_Gendr_Cd;
        public string dat_Appl_Dbtr_Entrd_SIN;
        public string dat_Appl_Dbtr_Parent_SurNme_Birth;
        public string dat_Appl_CommSubm_Text;
        public string dat_Appl_Rcptfrm_dte;
        public string dat_Appl_AppCtgy_Cd;
        public string dat_Appl_Group_Batch_Cd;
        public string dat_Appl_Medium_Cd;
        public string dat_Appl_Affdvt_Doc_TypCd;
        public string dat_Appl_Reas_Cd;
        public string dat_Appl_Reactv_Dte;
        public string dat_Appl_LiSt_Cd;
        public string Maintenance_ActionCd;
        public string dat_New_Owner_RcptSubmCd;
        public string dat_New_Owner_SubmCd;
        public string dat_Update_SubmCd;
    }

    public struct MEPLicenceDenial_RecType31
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;

        public string dat_Appl_Dbtr_LngCd;
        public string dat_LicSup_NoticeSntTDbtr_Dte;
        public string dat_LicSup_Dbtr_Brth_CtryCd;
        public string dat_LicSup_Dbtr_Brth_CityNme;
        public string dat_LicSup_Dbtr_EyesColorCd;
        public string dat_LicSup_Dbtr_HeightUOMCd;
        public string dat_LicSup_Dbtr_HeightQty;
        public string dat_LicSup_Dbtr_PhoneNumber;
        public string dat_LicSup_Dbtr_EmailAddress;
        public string dat_Appl_Dbtr_Addr_Ln;
        public string dat_Appl_Dbtr_Addr_Ln1;
        public string dat_Appl_Dbtr_Addr_CityNme;
        public string dat_Appl_Dbtr_Addr_PrvCd;
        public string dat_Appl_Dbtr_Addr_CtryCd;

        public string dat_Appl_Dbtr_Addr_PCd;
        public string dat_LicSup_Dbtr_EmplNme;
        public string dat_LicSup_Dbtr_EmplAddr_Ln;
        public string dat_LicSup_Dbtr_EmplAddr_Ln1;
        public string dat_LicSup_Dbtr_EmplAddr_CtyNme;
        public string dat_LicSup_Dbtr_EmplAddr_PrvCd;
        public string dat_LicSup_Dbtr_EmplAddr_CtryCd;
        public string dat_LicSup_Dbtr_EmplAddr_PCd;

        public string dat_LicSup_SupportOrder_Dte;
        public string dat_LicSup_CourtNme;
        public string dat_Appl_Crdtr_SurNme;
        public string dat_Appl_Crdtr_FrstNme;
        public string dat_Appl_Crdtr_MddleNme;
        public string dat_LicSup_PymPr_Cd;
        public string dat_LicSup_NrOfPymntsInDefault;
        public string dat_LicSup_AmntOfArrears;
        public string dat_LicSup_Declaration;
    }

    public struct MEPLicenceDenial_RecType41
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;

        public string dat_Appl_Dbtr_Last_Addr_Ln;
        public string dat_Appl_Dbtr_Last_Addr_Ln1;
        public string dat_Appl_Dbtr_Last_Addr_CityNme;
        public string dat_Appl_Dbtr_Last_Addr_PrvCd;
        public string dat_Appl_Dbtr_Last_Addr_CtryCd;
        public string dat_Appl_Dbtr_Last_Addr_PCd;

        public string RefSusp_Issuing_SubmCd;
        public string RefSusp_Appl_CtrlNr;
    }

    public struct MEPLicenceDenial_RecType99
    {
        public string RecType;
        public string ResponseCnt;
    }

    public struct MEPLicenceDenial_LicenceDenialDataSet
    {
        public MEPLicenceDenial_RecType01 LICAPPIN01;
        public List<MEPLicenceDenial_RecTypeBase> LICAPPIN30;
        public List<MEPLicenceDenial_RecType31> LICAPPIN31;
        public List<MEPLicenceDenial_RecTypeBase> LICAPPIN40;
        public List<MEPLicenceDenial_RecType41> LICAPPIN41;
        public MEPLicenceDenial_RecType99 LICAPPIN99;
    }

    public struct MEPLicenceDenial_LicenceDenialDataSetSingle
    {
        public MEPLicenceDenial_RecType01 LICAPPIN01;
        public MEPLicenceDenial_RecTypeBase LICAPPIN30;
        public MEPLicenceDenial_RecType31 LICAPPIN31;
        public MEPLicenceDenial_RecTypeBase LICAPPIN40;
        public MEPLicenceDenial_RecType41 LICAPPIN41;
        public MEPLicenceDenial_RecType99 LICAPPIN99;
    }


    public class MEPLicenceDenialFileData
    {
        public MEPLicenceDenial_LicenceDenialDataSet NewDataSet;

        public MEPLicenceDenialFileData()
        {
            NewDataSet.LICAPPIN30 = new List<MEPLicenceDenial_RecTypeBase>();
            NewDataSet.LICAPPIN31 = new List<MEPLicenceDenial_RecType31>();
            NewDataSet.LICAPPIN40 = new List<MEPLicenceDenial_RecTypeBase>();
            NewDataSet.LICAPPIN41 = new List<MEPLicenceDenial_RecType41>();
        }
    }

    public class MEPLicenceDenialFileDataSingle
    {
        public MEPLicenceDenial_LicenceDenialDataSetSingle NewDataSet;
    }
}
