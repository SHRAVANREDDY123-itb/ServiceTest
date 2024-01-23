using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities
{
    namespace DBConstants
    {


        public class LaborTypes
        {
            public const string Fuel = "F";
            public const string Labour = "L";
        }



        public class OptMode
        {
            public const string Cool = "C";
            public const string Defrost = "D";
            public const string Fail = "F";
            public const string No = "N";
            public const string Pass = "P";
            public const string Heat = "W";
            public const string Yes = "Y";
        }
        public class OptModeStatus
        {
            public const string Cool = "Cool";
            public const string Defrost = "Defrost";
            public const string Fail = "Fail";
            public const string No = "No";
            public const string Pass = "Pass";
            public const string Heat = "Heat";
            public const string Yes = "Yes";
        }

        public class TTEvents
        {
            public const string AlerHandled = "TTAlert";
            public const string HotsheetCreated = "TATCH";
            public const string Commentsrecorded = "TATCCom";
            public const string UnitArrived = "TATCA";
            public const string UnitDeparted = "TATCD";
            public const string UnitGensetAssociationChanged = "UGNChngd";
            public const string UnitPrenoteInformationUpdate = "UPNpdate";

            public const string Tripcomplete = "TripCmp";
            public const string Tripcompleteserviceorderunconfirmed = "TrpCmpSOUn";
            public const string ResendTrace = "TATReq";





        }

        public class TTAlerts
        {
            public const string CarbCompliant = "CGC001";
            public const string MissingInspec = "CGC002";
            public const string DwellingOrigin = "CGC003";
            public const string DwellingDest = "CGC004";
            public const string NotReporting = "CGC005";

            public const string RefueledGPS018 = "GPS018";
            public const string RefueledGPS034 = "GPS034";
            public const string RefueledGPS523 = "GPS523";
        }

        public class TTAlertStatus
        {
            public const string Active = "A";
            public const string Expired = "E";
            public const string Handled = "H";
            public const string InActive = "I";
        }

        public class PSPSOEvents
        {
            public const string Serviceorderadded = "SOAdd";
            public const string ServiceRequestDeparted = "SRDept";
            public const string ServiceRequestArrival = "SRArrl";
            public const string ServiceRequestConfirm = "SRConf";
            public const string ServiceRequestUnConfirm = "SRUnconf";
            public const string ServiceOrderHeaderUpdated = "EPSPSOH";
            public const string ServiceOrderCancelled = "SOCan";
            public const string ServiceOrderClosed = "SOCls";
            public const string SOCancelledNotification = "SOCanNote";
            public const string ServiceOrderActionUpdated = "SOActUpd";
            public const string ServiceOrderUnConfirmed = "SOUnconf";
            public const string ServiceOrderConfirmed = "SOconf";

            public const string ServiceOrderDeparted = "SODept";
            public const string ServiceOrderArrival = "SOArrl";


            public const string ServiceRequestChanged = "SRChange";
            public const string RequestedServicesActivated = "SRACT";
            public const string RequestedServicesDeactivated = "SRDEACT";
            public const string RequestedServicesAdded = "SRADD";
            public const string Hotsheetcreated = "SOHot";
            public const string TripCompletedServiceOrderCancelled = "TCSOCan";

            public const string SOResendotification = "SOResndNot";
            public const string HotsheetServiceOrderCancelled = "HSSOCan";


        }
        public class PSPSOActionEvents
        {
            public const string ServicesAccepted = "SVCA_ACPT";
            public const string ServicesCRQ = "SVCA_CRQ";
            public const string ServicesPending = "SVCA_PND";
            public const string ServicesRejected = "SVCA_RJT";
            public const string ServicesRejected_IMS = "SVC_RJ_IMS";
            public const string ServicesSubmited = "SVCA_SUB";

            public const string InspectionAutoAcceptFail = "AcceptFail";
        }

        public enum EventEntity
        {
            CompanyEvents,
            CompanyAccessEvent,
            CompanyUserEvent,
            PSPFOContractEvents,
            PSPSAContractEvents,
            PSPSAFacilityEvents,
            RouteEvents,
            SubscriptionEvents,
            PreNoteEvents,
            PreNoteUnitEvents,
            UnitEvents,
            SOEvents,
            HotsheetEvent,
            TrackAndTrcaEvents,
            TrackAndTracetriptEvents,
            TrackAndTraceUpdateTripEvents,
            TariffEvents,
            BillEvents
        }



        public class SysParamCd
        {
            public const string TPCD = "TPCD";
            public const string MessageCD = "MessageCD";
            public const string EDIFileINLocation = "EDIFileINLoc";
            public const string EDIFileOBLocation = "EDIFileOBLoc";
            public const string PNArchiveLocation = "PNArchiveLoc";
            public const string PNErrorLocation = "PNErrorLoc";
            public const string PNTargetLocation = "PNTargetLoc";
            public const string PNMappedLocation = "PNMappedLoc";
            public const string EDIMappingCls = "EDIMappingCls";
            public const string MinIdentifier = "MinIdentifier";
            public const string MaxIdentifier = "MaxIdentifier";
            public const string CurrIdentifier = "CurrIdentifier";
            public const string MsgStausCD = "MsgStausCD";
            public const string Idntfr = "Idntfr";
            public const string LastReadDt = "LastReadDt";
            public const string SASvcSystemParamCode = "SASvcPending";
            public const string FTPLogin = "FTPLogin";
            public const string FTPPassword = "FTPPswd";
        }

        public class PrenoteType
        {
            public const string CY = "C";
            public const string General = "G";
            public const string Trip = "T";
        }
        public class PrenoteAddRequestFrom
        {
            public const string UI = "U";
            public const string SO = "S";
            public const string EDI = "E";
            public const string PSP = "P";
            public const string System = "S";
        }
        public class PrenoteStatus
        {
            public const string Pending = "P";
            public const string Active = "A";
            public const string Completed = "C";
            public const string Error = "E";
            public const string Inactive = "I";
            public const string Cancelled = "CAN";
        }
        public class PreNoteUnitStatus
        {
            public const string Active = "ACT";
            public const string Cancelled = "CCL";
            public const string Pending = "PDG";
            public const string ApprovedforInvoice = "AFI";
            public const string TripCompleted = "TPC";
            public const string ReadyforInvoice = "RFI";
        }
        public class PrenoteUnitEvents
        {
            public const string SubmitPrenoteUnit = "PNU";
            public const string UpdatePrenoteUnit = "PNUNUPD";
            public const string CancelPrenoteUnit = "PNUNDEL";
            public const string UnitArrivedOrigin = "UAAO";
            public const string PrenoteUnitComplete = "PNUnitComp";
            public const string UnitApprovedForInvoice = "UAFI";

            public const string RFIDataMigrationSuccess = "URFIMS";
            public const string RFIDataMigrationError = "URFIME";
            public const string RFIProcessingSuccess = "URFIPS";
            public const string RFIProcessingError = "URFIME";
            public const string PrenoteUnitHistoryCreated = "PNUHistCrt";
            public const string EDIPrenoteUnitCancel = "EDIPNUCREQ";
        }
        public class UnitEvents
        {
            public const string TripCompleted = "TATCTP";
        }
        public class UnitType
        {
            public const string COFC = "CT";
            public const string PowerPack = "PP";
            public const string TOFC = "TR";

        }
        public class AssocInfo
        {
            public const string TL = "L";
            public const string TE = "E";

        }
        public class PrenoteEvents
        {
            public const string SubmitPrenote = "PNA";
            public const string UpdatePrenoteHeader = "PNM";
            public const string UpdatePrenoteUnits = "PNUNUPD";
            public const string UpdatePrenoteFacility = "PNFACUPD";
            public const string SubmitePrenoteFacilities = "PNFACADD";
            public const string RemovePrenoteFacilitySvcType = "PNSVCDEL";
            public const string RemovePrenoteFacility = "PNFACDEL";
            public const string EdiUpdateReject = "EDIUPDREJ";
            public const string EdiUpdateAccept = "EDIUPDACP";
            public const string CancelPrenote = "PNCAN";
            public const string UpdateAdminServices = "ADSVCUPD";
            public const string SubmitNewEdiUpdate = "EDINEWUPD";

            public const string PrenoteDurationChange = "PNDTMOD";
            public const string PrenoteRouteChange = "PNRouteMOD";

            public const string ConfirmSOAdd = "PNSON";
            public const string UnconfirmSOAdd = "PNUSOADD";
            public const string ConfirmSOCancel = "PNSONCAN";
            public const string UnConfirmSOCancel = "PNUSOCAN";
            public const string PrenoteActivate = "PNACT";
            public const string PrenoteTripcomplete = "TripCmp";

            public const string FacilityActivated = "PNFACACT";
            public const string FacilityDeActivated = "PNFACDCT";
            public const string FacilityDeActivatedConfirmSO = "FACDCTCSO";
            public const string FacilityDeActivatedUnconfirmSO = "FACDCTUSO";

            public const string FacilitySvcActivated = "FACSVCACT";
            public const string FacilitySvcDeActivated = "FACSVCDCT";
            public const string FacilitySvcDeActivatedConfirmSO = "FACSVCDCSO";
            public const string FacilitySvcDeActivatedUnconfirmSO = "FACSVCDUSO";

            public const string PrenoteReviewPending = "PNPRP";
            public const string PrenoteResolveRoute = "PNPRR";

            public const string SubmitSecondLegEdiUpdate = "PN2LUPDATE";
            public const string SubmitNewSecondLegEdiUpdate = "EDINEW2UPD";
            public const string EdiSecondLegUpdateReject = "EDI2UPDREJ";
            public const string EdiSecondLegUpdateAccept = "EDI2UPDACP";
            public const string CacnelEDIPrenoteReject = "EDICANREJ";
            public const string CacnelEDIPrenoteAccept = "EDICANACC";

            public const string EDICancelRequest = "EDICANREQ";

        }

        public class SubscriptionEvents
        {
            public const string NewSubscription = "SNAdd";
            public const string EditSubscription = "SNEdit";
            public const string DeactivateSubscription = "SNDct";
            public const string ActivateSubscription = "SNAct";
            public const string DeactivateMessage = "SNDctMsg";
            public const string ActivateMessage = "SNActMsg";
            public const string EditMessage = "SNEdMsg";
            public const string AddMessage = "SNAddMsg";
            public const string DeactivateRecipient = "SNDctRep";
            public const string ActivateRecipient = "SNActRep";
            public const string EditRecipient = "SNEdRep";
            public const string AddRecipient = "SNAddRep";


        }
        public class SASOEvents
        {
            public const string Service_order_added = "SOAdd";
            public const string New_service_performed = "SVCAdd";
            public const string New_ServiceAction_added = "SvcActiAdd";
            public const string ServicePerformedEdited = "SOPudt";
            public const string ServicePerformed = "SvcPerf";
            public const string ServiceSubmit = "SvcSubmit";
            public const string ServiceRecalled = "SVCA_RECAL";
            public const string ServiceEdited = "SvcEdit";
            public const string ServicesRolled = "SvcRoll";
            //IMS
            public const string ServicesSubmitIMS = "SvcSubIms";
            public const string ServicesEditIMS = "SvcEditIms";

        }
        public class SVCSource
        {
            public const string Mobile = "MOB";
            public const string Service = "SVC";
            public const string UserInterface = "UIF";
        }

        public class InspectionType
        {
            public const string OnTrain = "ONR";
            public const string OnGround = "ONG";
            public const string Fuel_MNR = "FOL";
            public const string Recheck = "RCK";
            public const string PSPServices = "PSP";
        }

        public class SOStatus
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


        public class SOSvcLogStatus
        {
            //public const string Accepted = "A";
            //public const string Billed = "B";
            //public const string CorrectionRequired = "C";
            //public const string Invoiced = "I";
            //public const string Pending = "P";
            //public const string Rejected = "R";
            //public const string Submitted = "S";

            public const string Accepted = "A";
            public const string Billed = "B";
            public const string CorrectionRequired = "C";
            public const string Error = "E";
            public const string Invoiced = "I";
            public const string Pending = "P";
            public const string Rejected = "R";
            public const string Submitted = "S";
            public const string Recalled = "X";
            public const string ReadyForBilling = "F";
            public const string Billedstatus = "Billed";
        }

        public class ActionStatus
        {
            public const string Pending = "P";
            public const string Completed = "C";
            public const string Closed = "CLS";
        }
        public class ActionCode
        {
            public const string EDIUpdate = "PNEDI";
            public const string SOConfirmation = "PNSO";
            public const string ArrivalLogMissing = "ALM";
            public const string DepartureLogMissing = "DLM";
            public const string PrenoteReviewPending = "PRP";
            public const string PrenoteResolveRoute = "PRR";
            public const string EDI2LegUpdate = "PN2LG";
            public const string CancelEDIPrenote = "PNECN";
        }
        public class FacilityType
        {
            public const string Origin = "O";
            public const string Destination = "D";
            public const string Midpoint = "M";
            public const string InTransit = "I";
            public const string CYUnits = "C";
        }

        public class UnitTrasitionStateCd
        {
            public const string Origin = "O";
            public const string Destination = "D";
            public const string Midpoint = "M";
            public const string InTransit = "I";
            public const string CYUnits = "C";
        }

        public class ComoanyCodes
        {
            public const string AZ = "Alliance";
            public const string Railinc = "Railinc";
            public const string Prime = "Prime";
        }

        public class TriggeringAlerts
        {
            public const string UnitArrival = "A";
            public const string UnitDeparture = "D";
            public const string JunctionArrDep = "J";
        }

        public class RouteEvents
        {
            public const string NewFacilitySubmit = "RoFacIns";
            public const string RouteUpdate = "RouteUpdt";
            public const string RouteInactive = "RouteInAct";
            public const string RouteActive = "RouteAct";
            public const string NewRoute = "RouteAdd";
            public const string RouteFacilityUpdate = "RoFacUpd";
            public const string AdminServicesUpdate = "AdminSvcUp";
        }
        public class EDIEvent
        {
            public const string FileRecv = "FileRecv";
            public const string TranSucc = "TranSucc";
            public const string TranFail = "TranFail";
            public const string FileProcessed = "FileProc";
            public const string FileFailed = "FileFail";
            public const string MsgSuccuss = "MsgSucc";
            public const string MsgFailed = "MsgFail";
            public const string MsgFileNotFound = "MsgFileNt";
            public const string Event = "Event";
            public const string Fac_Not_Res = "FACNOTRES";

        }

        public class EDIMessageStatus
        {
            public const string Success = "S";
            public const string Failed = "F";
        }

        public class UnitRunning
        {
            public const string Yes = "Yes";
            public const string No = "No";
        }
        public class UnitMode
        {
            public const string Pass = "Pass";
            public const string Fail = "Fail";
        }
        public class GensetRunning
        {
            public const string Yes = "Yes";
            public const string No = "No";
        }
        public class GensetStatus
        {
            public const string Pass = "Pass";
            public const string Fail = "Fail";
        }
        public class ServiceOrderTypeSONbr
        {
            public const string PSP = "P";
            public const string SA = "S";

        }
        public class EDIUser
        {
            public const string EDIUserCd = "EDIUserId";
        }
        public class Params
        {
            public const string ApplicationURL = "AppURL";
        }

        public class CompanyCodes
        {
            public const string RailInc = "Railinc";
            public const string AZ = "AZ";
            public const string MT = "MT";
            public const string PA = "PA";
            public const string CR = "CR";
            public const string BN = "BN";
            public const string WAM = "WM";

            public const string OL = "OL";
            public const string HZ = "HZ";
            public const string MAEU = "MAEU";
            public const string IB = "IB";

            public const string WAMSourceCd = "WAM";


        }

        public class TrackTrackEventCode
        {
            public const string Prenote_Unit_Activated = "PNUnitAct";
            public const string Prenote_Unit_Completed = "PNUnitComp";
            public const string Unit_trip_completed = "UNITTPC";
        }

        public class TrackTraceLookup
        {
            public const string TPEDILookUpField = "CompanyCd";
        }

        #region Data Processing

        public class BatteryStatus
        {
            public const string LOW = "LOW";
            public const string NORMAL = "NORMAL";
            public const string STBY = "STBY";
        }

        public class CarbState
        {
            public const string Yes = "2";
            public const string No = "1";
        }

        public class GateStatus
        {
            public const string GateStatus_0 = "0";
            public const string GateStatus_1 = "1";
            public const string Current = "CURRENT";
            public const string LastKnown = "LAST KNOWN";
        }

        public class GensetState
        {
            public const string NotAvailable = "-1";
            public const string GensetOFF = "0";
            public const string GensetON = "1";
            public const string GensetON_Running = "2";
            public const string GensetOn_Running_ReeferConnected = "3";
            public const string GensetOn_Running_ReeferConnectedON = "4";
            public const string GensetOn_Running_ReeferConnectedON5 = "5";
            public const string Error = "6";
            public const string NA = "6";
        }

        public class GPSSwitch
        {
            public const string OFF = "OFF";
            public const string OFF_M = "OFF (M)";
            public const string OFF_L = "OFF (L)";
            public const string OFF_R = "OFF (R)";
            public const string ON = "ON";
            public const string ON_M = "ON (M)";
            public const string ON_L = "ON (L)";
            public const string ON_R = "ON (R)";
        }

        public class MountType
        {
            public const string MountType_0 = "0";
            public const string MountType_1 = "1";
            public const string MountType_2 = "2";
            public const string ClipOn = "Clip-On";
        }

        public class OPMode
        {
            public const string AlarmShutdown = "AlarmShutdown";
            public const string Cooling = "Cooling";
            public const string CoolMax = "CoolMax";
            public const string CoolReduced = "CoolReduced";
            public const string Defrost = "Defrost";
            public const string Disabled = "Disabled";
            public const string FanReduction = "FanReduction";
            public const string FunctionTest = "FunctionTest";
            public const string Heating = "Heating";
            public const string HeatMax = "HeatMax";
            public const string HeatReduced = "HeatReduced";
            public const string Idle = "Idle";
            public const string Manual = "Manual";
            public const string Modulation = "Modulation";
            public const string PowerOff = "PowerOff";
            public const string Pretrip = "Pre-trip";
            public const string Pulse = "Pulse";
            public const string Startup = "Startup";
            public const string UnitOff = "Unit Off";
        }


        public class OPStatus
        {
            public const string AlarmShutdown = "Alarm Shutdown";
            public const string ConfigMode = "Config Mode";
            public const string Continuous = "Continuous";
            public const string Cycle_Sentry = "Cycle Sentry";
            public const string CycleSentry = "CycleSentry";
            public const string Idle = "Idle";
            public const string NA = "NA";
            public const string on = "on";
            public const string StartStop = "Start/Stop";
            public const string StartStop_Off_Cycles = "Start/Stop Off Cycle";
        }

        public class CarrierCompanyCodes
        {
            public const string Carrier = "C";
        }

        #endregion


        #region "EDI Process Constants"

        public class EDIProcessingConstants
        {
            public const string GlobalView = "Global View";
            public const string FOTempUOM = "FOTempUOM";
            public const string DefaultUOM = "C";

        }

        #endregion


        public class UOMCategory
        {
            public const string Temperature = "T";

        }
        public class TariffEvents
        {
            public const string NewFuelTariff = "FTAdd";
            public const string EditFuelTariff = "FTEdit";
            public const string NewServiceTariff = "STAdd";
            public const string EditServiceTariff = "STEdit";
            public const string NewTaxTariff = "TTAdd";
            public const string EditTaxTariff = "TTEdit";
            public const string TaxExpired = "TTExp";
            public const string TExpChanged = "TExpChange";
            public const string ServiceExpired = "STExp";
            public const string SExpChanged = "SExpChange";
            public const string FTExpired = "FTExp";
            public const string FTExpChanged = "FExpChange";
            public const string NewInvServiceTariff = "ISTAdd";
            public const string EditInvServiceTariff = "ISTEdit";
            public const string InvServiceExpired = "ISTExp";
            public const string InvSvcExpChanged = "ISExpChnge";
        }

        public class BillEvents
        {
            public const string AddBill = "BAdd";
            public const string EditBill = "BEdit";
            public const string CancelBill = "BCancel";
            public const string CompletReview = "BComReview";
            public const string ConfirmBill = "BConfirm";
            public const string ReconcileBill = "BRecon";
            public const string SubmitBill = "BSubmit";
            public const string ReconcileFinished = "BReDone";

            public const string BillCancelled = "BCanceled";
            public const string BillReviewed = "BReveiwed";
            public const string BillConfirmed = "BConfirmed";
        }

        public class SoSvcLogEvents
        {
            public const string ReadyForBilling = "RFBill";
            public const string ServiceBilled = "SvcBilled";
            public const string CancelledBill = "ClBill";
        }

        public class BillSOEvents
        {
            //saved Event Category Code is "SO"
            public const string VendorBillCancelled = "VBCanceled";
            public const string VendorBillReviewed = "VBReviewed";
            public const string VendorBillConfirmed = "VBConfrmed";
        }

        public class YesNoFlag
        {
            public const string Yes = "Y";
            public const string No = "N";
        }

        public class UnitRemarksCategory
        {
            public const string CustomerRemarks = "C";
            public const string InternalRemarks = "I";
            public const string VendorRemarks = "V";
            public const string Question = "Q";
        }
        public class SvcCategory
        {
            public const string AncillaryServices = "A";
            public const string Fuel = "F";
            public const string Inspection = "I";
            public const string Labor = "L";
            public const string MaintenanceRepair = "M";
            public const string PSPAdminServices = "P";
            public const string EDISvcs = "E";
        }
        public class ServiceRateType
        {
            public const string Holiday = "H";
            public const string Overtime = "O";
            public const string Regular = "R";
        }

        public class ContactType
        {
            public const string Hotsheet = "H";
            public const string PSPAdmin = "PSPA";
            public const string Support = "S";
        }
        public class SourceType
        {
            public const string Telemetry = "E";
            public const string PrenoteUI = "P";
            public const string Tracing = "T";
            public const string UI = "U";
        }

        public class CCCategory
        {
            public const string Inspection = "I";
            public const string SystemCategory = "S";
            public const string UserCategory = "U";
            public const string PreNote = "P";
            public const string TrackTrace = "T";
            public const string PrenoteRoute = "R";
        }
        public class TempUOM
        {
            public const string celsius = "C";
            public const string fahrenheit = "F";
        }

        public class TankSize
        {
            public const decimal Gallons120 = 120;
        }
        public class RateType
        {
            public const string FuelTax = "PGL";
        }
        public class TariffTypes
        {
            public const string FuelTariff = "BFL";
            public const string ServiceTariff = "BSV";
            public const string TaxTariff = "TAX";
        }

        public class EDIAction
        {
            public const string strCreate = "CREATE";
            public const string strUpdate = "UPDATE";
            public const string strCancel = "CANCEL";
        }
        public class CCRules
        {
            public const string UnitTemperatureConversion = "UTMPC";
            // public const string EventDatetimeConversion = "EDTMC";
            public const string UnitFuelConversion = "UTFLC";
        }
        public class SoSvcType
        {
            public const string Inspection = "Inspection";
            public const string Fueling = "Fueling";
            public const string PumpFee = "PUMPF";
            public const string FuelFee = "FLFEE";
            public const string HFuelFee = "HOFF";
            public const string OFuelFee = "OTFLF";
        }
        public class ChargeItemType
        {
            public const string Fuel = "F";
            public const string Parts = "P";
            public const string Service = "S";
            public const string Tax = "T";
        }

        public class ChargeItemCd
        {
            public const string PumpFee = "CH00011";
            public const string FuelFee = "CH00018";
            public const string HFuelFee = "CH00028";
            public const string OFuelFee = "CH00037";
        }

        public class IMSSync
        {
            public const string SyncInsertTxt = "Create inspection Data Sync fail";
            public const string SyncEdiTxt = "Edit inspection Data Sync fail";
            public const string SynUnitStatus = "AFI";
            public const string SyncSALTxt = "SAL Data Sync fail";
            public const string SyncPnFacTxt = "Edit prenote facility Data Sync fail";

        }
        public class LocationEvents
        {
            public const string AddLocation = "LOCADD";
            public const string EditLocation = "LOCUPD";
        }
        public class SOType
        {
            public const string Admin = "A";
            public const string ServiceOrder = "S";
        }
        public class Line
        {
            public const long ONE = 1322;
        }
        public class EDIFileType
        {
            public const string PnCancel = "998";
        }

        public class SvcInstructionType
        {
            public const string InspectOnce = "Inspect Once";
            public const string InspectDaily = "Inspect Daily";
            public const string InspectEveryOtherDay = "Inspect Every Other Day";

        }

    }
}
