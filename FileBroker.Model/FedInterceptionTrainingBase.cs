using System;
using System.Collections.Generic;

namespace FileBroker.Model
{
    public struct FedInterceptionTraining_RecType01
    {
        public int RecType;
        public int Cycle;
        public DateTime FileDate;
        public string EnfSrv_Src_Cd;
        public string EnfSrv_Loc_Cd;
        public string Batch_Identifier;
        public string Environment;
        public string Bypass_FT_Ind;
    }

    public struct FedInterceptionTraining_RecType02
    {
        public int RecType;
        public string TransactionType;
        public string DebtorId;
        public string PaymentIdentifier;
        public DateTime PayableDate;
        public int? AvailableOrTransferredAmt;
        public string EnfSrv_Loc_Cd;
        public string EnfSrv_SubLoc_Cd;
        public string Lctr_Cd;
        public string XRefSin;
    }

    public struct FedInterceptionTraining_RecType99
    {
        public int RecType;
        public int ResponseCnt;
        public int AmountTotal;
        public int SINHashTotal;
    }

    public class FedInterceptionTrainingBase
    {
        public FedInterceptionTraining_RecType01 TRIN01;
        public List<FedInterceptionTraining_RecType02> TRIN02;
        public FedInterceptionTraining_RecType99 TRIN99;

        public FedInterceptionTrainingBase()
        {
            TRIN02 = new List<FedInterceptionTraining_RecType02>();
        }
    }
}
