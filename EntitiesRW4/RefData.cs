using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    namespace RefData
    {
        public class R_CompanyStatus
        {
            public const string Active = "A";
            public const string In_Active = "I";
        }
        public class R_EventCategory
        {
            public const string Fleet_Owner = "FO";
            public const string Prenote = "PN";
            public const string Service_Order = "SO";
            public const string Facility_Management = "FA";
            public const string SA_Management = "SA";
        }
        public class R_EventMaster
        {
            public const string Facility_Added = "FAADD";
            public const string Facility_Activated = "FAENA";
            public const string Facility_Deactivated = "FADIS";
            public const string Service_Agent_Activated = "SAENA";
            public const string Service_Agent_Deactivated = "SADIS";
            public const string Service_Agent_Added = "SAADD";
            public const string Service_Agent_Sequence_Changed = "SASEQ";
            public const string Services_Added = "SVCADD";
            public const string Services_Modified = "SVCUPD";
            public const string Line_Preference_changed = "LPREFCH";

            public const string Facility_contact_associated = "FCASSC";
            public const string Facility_contact_removed = "FCASSCREM";

            //SA Events 

            public const string SAContractAdded = "ContractAd";
            public const string SAContractUpdated = "ContractUp";
            public const string SAContactAdded = "ContactAd";
            public const string SAContactUpdated = "ContactUp";
            public const string SAConfigUpdated = "ConfigUp";
            public const string SAContactActivated = "ContactAct";
            public const string SAContactDeActivated = "ContactDct";
            public const string SAFacilityAdded = "FacAdd";
            public const string SAFacilityContactUpdated = "FacCntUp";
            public const string SAFacilityActivated = "FacAct";
            public const string SAFacilityDeActivated = "FacDct";
            public const string SAContractActivated = "ContrAct";
            public const string SAContractDeActivated = "ContrDct";

        }
        public class ServiceStatus
        {
            public const string Reconfigure = "C";
            public const string Running = "R";
            public const string Sleep = "P";
            public const string Start = "T";
            public const string Stop = "S";


        }
        public class Flag
        {
            public const string NO = "N";
            public const string YES = "Y";
        }

        public class MessageStatus
        {
            public const string Pending = "P";
            public const string Failed = "F";
            public const string InProgress = "I";
            public const string Success = "S";
        }
        public class FileStatus
        {
            public const string ProcessingFailed = "PF";
            public const string PendingProcessing = "PP";
            public const string Processing = "PR";
            public const string Processed = "PS";
            public const string PendingTranslation = "PT";
            public const string TranslationFailed = "TF";
        }
        public class R_RouteStatus
        {
            public const string Active = "A";
            public const string Inactive = "I";
        }
        public class R_SOStatus
        {
            public const string Arrived = "APR";
            public const string Confirmed = "CNF";
            public const string Departed = "DEP";
            public const string InProgress = "PRG";
            public const string UnConfirmed = "UCF";
        }
        public class R_SubscriptionMsgTypes
        {
            public const string NotificationPerReeder = "R";
            public const string NotificationBatchAllEventsAndReefer = "B";
        }
    }
}