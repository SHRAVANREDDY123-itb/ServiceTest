using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RW4OBDistributorProcess
{

    public class RailincCER
    {
        public long EventId { get; set; }
        public string FleetCd { get; set; }
        public string UnitNumber { get; set; }
        public string FileName { get; set; }
    }
    public enum Status
    {
        E,//Error       
        P, //Processed    
        S //Success
    }




    #region  WAM New Changes

    public class WamSubscriptionCreate
    {
        public List<WamSubscription> subscriptionsPost { get; set; }

    }



    public class WamSubscription
    {

        public string equipmentId { get; set; }
        public string subscriptionStartDate { get; set; }
        public string subscriptionEndDate { get; set; }
    }


    public class WamSubscriptionError
    {
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }

    }


    public class WamSubscriptionUpdate
    {

        public string subscriptionId { get; set; }
        public string subscriptionStartDate { get; set; }
        public string subscriptionEndDate { get; set; }
    }

    public class DataSubscriptionUpdate
    {

        public List<WamSubscriptionUpdate> subscriptionsPut { get; set; }
    }

    #endregion
}
