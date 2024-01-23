using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConstants
{
    public enum ErrSeverity
    {
        Default = 0,
        Information = 1,
        Warning = 2,
        Critical = 3
    }

    public class UserExportPreferenceType
    {
        public const string AdvancedSearch = "S";
        public const string Export = "E";
    }

    public class YesNo
    {
        public const string Yes = "Y";
        public const string No = "N";
    }

    
    public class PassFail
    {
        public const string Pass = "P";
        public const string Fail = "F";
    }
    
    public class ExportFileType
    {
        public const string Excel = "E";
        public const string PDF = "P";
    }

    public enum FWEventEntity
    {
        CompanyEvents,
        CompanyAccessEvent,
        CompanyUserEvent
      
    }
    public class RG_SysService
    {
        public const string CurrentStatus_Cd = "CurrentStatusCd";
        public const string LastStarted_DtTm = "LastStartedDtTm";
        public const string LastStopped_DtTm = "LastStoppedDtTm";
        public const string ReLoad_Flg = "ReLoadFlg";
        public const string RequestedStatus_Cd = "RequestedStatusCd";
        public const string RestingTime_Tm = "RestingTimeTm";
        public const string SysService_Cd = "SysServiceCd";
        public const string SysService_Id = "SysServiceId";
        public const string TableName = "R_SysService";


    }
    public class RG_SysServiceThreads
    {
        public const string CurrentProcessingStart_DtTm = "CurrentProcessingStartDtTm";
        public const string CurrentProcessingStop_DtTm = "CurrentProcessingStopDtTm";
        public const string CurrentStatus_Cd = "CurrentStatusCd";
        public const string IsSuccesful = "IsSuccesful";
        public const string LastStarted_DtTm = "LastStartedDtTm";
        public const string LastStopped_DtTm = "LastStoppedDtTm";
        public const string Method_Nm = "MethodNm";
        public const string ReLoad_Flg = "ReLoadFlg";
        public const string RequestedStatus_Cd = "RequestedStatusCd";
        public const string Retries = "Retries";
        public const string SysService_Id = "SysServiceId";
        public const string SysServiceThread_Id = "SysServiceThreadId";
        public const string TableName = "R_SysServiceThreads";
        public const string Task_Tm = "TaskTm";
        public const string ThreadSleep_Tm = "ThreadSleepTm";
        public const string ThreadType = "ThreadType";

    }

    
    public class CompanyType
    {
        public const string PSP = "P";
        public const string SA = "S";
    }

    public class PSPSAConstants
    {
        public const string SASOSvcSourceCd = "UIF";
    }
    public class SOTypeCd
    {
        public const string ServiceOrder = "S";
        public const string HotSheet = "H";
        public const string PSPAdmin = "A";
    }

    public class MobileParam
    {
        public const string MobileVersion = "MOBAPPVer";
        //public const string MobileURL = "MOBAPPUrl";
        public const string MOBSASOSvcSourceCd = "MOB";
        public const string MobileImagePath = "MOBImagePath";
        public const string MobileDownloadURL = "MOBAPPUrl";
    }
    public class ExternalServiceParam
    {
       
        public const string EXTSASOSvcSourceCd = "SVC";
        public const string EXTReferenceFilePath = "EXTSRVREFPATH"; 
    }

    public class FacilityStatusCode
    {
        public const string Origin = "O";
        public const string Destination = "D";
        public const string Midpoint = "M";
    }
    public class ConfigParamCd
    {
        public const string SASvcSystemParamCode = "SASvcPending";
        public const string OTHolidayLaRateParamCode = "OTHolidayLaRate";
        public const string FuelRateByLineParamCode = "FuelRateByLine";
        public const string InspectionType = "InspType";
        public const string SABillingFooter = "BillingFooter";
    }
    public class ChargeItemTypes
    {
        public const string Fuel = "F";
        public const string Parts = "P";
        public const string Service = "S";
        public const string Tax = "T";
    }

    public class PreNoteTypes
    {
        public const string Trip = "T";
        public const string CY = "C";
    }

    public class FacilityTypes
    {
        public const string UnKnown = "99999";
    }
    public class PNStatus
    {
        public const string Pending = "P";
        public const string Active = "A";
        public const string Completed = "C";
        public const string Error = "E";
        public const string Inactive = "I";
        public const string Cancelled = "CAN";
    }

    public class ServiceCategory
    {
        public const string AncillaryServices = "A";
        public const string Fuel = "F";
        public const string Inspection = "I";
        public const string Labor = "L";
        public const string MaintenanceRepair = "M";
        public const string PSPAdminServices = "P";
        public const string EDISvcs = "E";
    }
    public class ServiceOrderStatus
    {
        public const string Arrived = "APR";
        public const string Cancelled = "CCL";
        public const string Confirmed = "CNF";
        public const string Departed = "DEP";
        public const string InProgress = "PRG";
        public const string UnConfirmed = "UCF";
        public const string PendingPSPApproval = "PAP";
        public const string PendingPSPConfirmation = "PCF";
        public const string ActivateSO = "ACT";
        public const string DeActivateSO = "DEA";
        public const string Closed = "CLS";
    }

    public class ApplicationParam
    {
        public const string EDIUpdateException = "EDIUpdateException";
        public const string EDIETSAndIssueDt = "EDIETSAndIssueDt";
    }
    public class InspectionTypes
    {
        public const string OnTrain = "ONR";
        public const string OnGround = "ONG";
        public const string Fuel_MNR = "FOL";
        public const string Recheck = "RCK";
        public const string PSPServices = "PSP";
    }
    public class UnitRemarksCategoryCodes
    {
        public const string CustomerRemarks = "C";
        public const string InternalRemarks = "I";
        public const string VendorRemarks = "V";
    }

    public class LineConfigs
    {
        public const string FOISISO = "1";
        public const string FOTempUOM = "F";
        public const string FOPNSeed = "1";
        public const string FOPNCurrent = "0";
        public const string FOUnitTypeCD = "CT";
        public const string FOGensetTypeCd = "UMT";
        public const int ContractExpYrs = 4;

    }

    public class LineRegions
    {
        public const string RegionUS = "US";
        public const string RegionCA = "CA";
    }

    public class AppNames
    {
        public const string RW = "RW";
        public const string IMS = "IMS";
    }

    public class SvcTypeDsc
    {
        public const string AdditionalCharges = "Mobile Fuel Service Charge";
        public const string ADCH = "ADCH";
    }
}
