namespace FOAEA3.Model.Enums
{
    public enum ApplicationState : short
    {
        UNDEFINED = -1,
        INITIAL_STATE_0 = 0,
        INVALID_APPLICATION_1 = 1,
        AWAITING_VALIDATION_2 = 2,
        SIN_CONFIRMATION_PENDING_3 = 3,
        SIN_CONFIRMED_4 = 4,
        SIN_NOT_CONFIRMED_5 = 5,
        PENDING_ACCEPTANCE_SWEARING_6 = 6,
        VALID_AFFIDAVIT_NOT_RECEIVED_7 = 7,
        APPLICATION_REJECTED_9 = 9,
        APPLICATION_ACCEPTED_10 = 10,
        APPLICATION_REINSTATED_11 = 11, // only used for tracing event -- not for application
        PARTIALLY_SERVICED_12 = 12,
        FULLY_SERVICED_13 = 13,
        MANUALLY_TERMINATED_14 = 14,
        EXPIRED_15 = 15,
        FINANCIAL_TERMS_VARIED_17 = 17,
        AWAITING_DOCUMENTS_FOR_VARIATION_19 = 19,
        APPLICATION_SUSPENDED_35 = 35,
        INVALID_VARIATION_SOURCE_91 = 91,
        INVALID_VARIATION_FINTERMS_92 = 92,
        VALID_FINANCIAL_VARIATION_93 = 93
    }
}

/*
AppLiSt_Cd	AppList_Txt_E
0	Waiting for validation                            
1	Duplicate / invalid application                   
2	Awaiting validation                               
3	SIN confirmation pending                          
4	SIN confirmed                                     
5	SIN not confirmed                                 
6	Pending acceptance / swearing                     
7	Valid affidavit not received                      
8	Affidavit accepted                                
9	Application rejected                              
10	Application accepted                              
11	Application re-instated                           
12	Partially serviced                                
13	Fully serviced                                    
14	Manually terminated                               
15	Expired                                           
17	Financial terms varied                            
19	Awaiting documents for variation                  
29	Transfer requested                                
32	Rejected from BF                                  
35	Application suspended                             
80	Inbound Clear                                     
81	Invalid source holdback                           
82	Invalid financial terms                           
83	Valid financial terms                             
84	Errors in Licence Suspension                      
85	Valid Licence Suspension                          
86	Valid L01                                         
87	Valid L03                                         
88	L03 cancellation                                  
90	Archived                                          
91	Invalid Variation (Source)                        
92	Invalid Variation (Fin. Terms)                    
93	Valid Financial Variation                         
99	Removed                                            
 */