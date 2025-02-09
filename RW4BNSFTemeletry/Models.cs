using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RW4BNSFTelemetry
{

    [DataContract(Namespace = "")]
    [XmlRoot("WamMessage")]

    public class WamMessage
    {

        [XmlElement("Errors")]
        public string Errors { get; set; }

        [XmlElement("Messages")]
        public MessageData Messages { get; set; }

    }

    [DataContract]
    public class MessageData
    {
        [XmlElement("Message")]
        public Message Message { get; set; }
    }




    [DataContract]
    public class Control
    {
        [XmlElement("MsgID")]
        public string MsgID { get; set; }

        [XmlElement("MsgType")]
        public string MsgType { get; set; }

        [XmlElement("RcdSerialNum")]
        public string RcdSerialNum { get; set; }

        [XmlElement("RequestMsgID")]
        public string RequestMsgID { get; set; }

    }


    [DataContract]
    public class RcdStatus
    {

        [XmlElement("AcceptableACVoltage")]
        public bool AcceptableACVoltage { get; set; }

        [XmlElement("AcceptableBatteryVoltage")]
        public bool AcceptableBatteryVoltage { get; set; }

        [XmlElement("BatteryVoltage")]
        public double BatteryVoltage { get; set; }

        [XmlElement("DeviceTemp")]
        public double DeviceTemp { get; set; }

        [XmlElement("EventDtm")]
        public DateTime EventDtm { get; set; }

        [XmlElement("GeofenceDefinitionRevision")]
        public int GeofenceDefinitionRevision { get; set; }

        [XmlElement("GPSLastLock")]
        public int GPSLastLock { get; set; }

        [XmlElement("GPSLatitude")]
        public double? GPSLatitude { get; set; }

        [XmlElement("GPSLockState")]
        public string GPSLockState { get; set; }

        [XmlElement("GPSLongitude")]
        public double? GPSLongitude { get; set; }

        [XmlElement("GPSSatelliteCount")]
        public int GPSSatelliteCount { get; set; }

        [XmlElement("InsideGeofence")]
        public int InsideGeofence { get; set; }

        [XmlElement("Is3G")]
        public string Is3G { get; set; }

        [XmlElement("MCC")]
        public int MCC { get; set; }

        [XmlElement("MessageFormatRevision")]
        public int MessageFormatRevision { get; set; }

        [XmlElement("MNC")]
        public int MNC { get; set; }

        [XmlElement("OnWakeUp")]
        public bool OnWakeUp { get; set; }

        [XmlElement("PtiRunning")]
        public bool PtiRunning { get; set; }

        [XmlElement("RcdMoving")]
        public bool RcdMoving { get; set; }

        [XmlElement("RcdPowerSource")]
        public string RcdPowerSource { get; set; }

        [XmlElement("ReeferACPowerLevel")]
        public double ReeferACPowerLevel { get; set; }

        [XmlElement("ReeferClockInSync")]
        public int ReeferClockInSync { get; set; }

        [XmlElement("ReeferCommunicationEstablished")]
        public bool ReeferCommunicationEstablished { get; set; }

        [XmlElement("ReeferOemType")]
        public string ReeferOemType { get; set; }

        [XmlElement("ReeferPhysicallyConnected")]
        public bool ReeferPhysicallyConnected { get; set; }

        [XmlElement("ReeferSwitchOn")]
        public bool ReeferSwitchOn { get; set; }

        [XmlElement("RSSI")]
        public int RSSI { get; set; }

        [XmlElement("RSSIQualifier")]
        public string RSSIQualifier { get; set; }

        [XmlElement("RTDLOn")]
        public bool RTDLOn { get; set; }

        [XmlElement("TowerBaseStationId")]
        public long TowerBaseStationId { get; set; }

        [XmlElement("TowerLocalizationAreaCode")]
        public long TowerLocalizationAreaCode { get; set; }


        [XmlElement("ReeferComms")]
        public bool ReeferComms { get; set; }

        [XmlElement("ReeferCommsCounter")]
        public int ReeferCommsCounter { get; set; }

        [XmlElement("ReeferCommsDisconnectCounter")]
        public int ReeferCommsDisconnectCounter { get; set; }
    }

    [DataContract]
    public class ReeferProp
    {

        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("value")]
        public string value { get; set; }
    }


    [DataContract]
    public class Message
    {
        [XmlElement("Data")]
        public WamData Data { get; set; }

    }

    [DataContract]
    public class WamData
    {
        [XmlElement("IBndMsg")]
        public IBndMsg IBndMsg { get; set; }
        //[XmlElement("Data")]
        //public Data Data { get; set; }
    }

    [DataContract]
    public class Data
    {
        [XmlElement("IBndMsg")]
        public string MsgID { get; set; }
        [XmlElement("RcdStatus")]
        public RcdStatus RcdStatus { get; set; }

        [XmlElement("ReeferStatus")]
        public ReeferStatus ReeferStatus { get; set; }
    }


    [DataContract]
    public class ReeferStatus
    {
        [XmlElement("ReeferProps")]
        public ReeferProperties ReeferProps { get; set; }


    }


    [CollectionDataContract]
    public class ReeferProperties
    {
        [XmlElement("ReeferProp")]
        public List<ReeferProp> ReeferProp { get; set; }
    }

    [DataContract]
    public class IBndMsg
    {
        [XmlElement("Control")]
        public Control Control { get; set; }

        [XmlElement("Data")]
        public Data Data { get; set; }
    }


    public class EquipmentInfo
    {
        public string unitTypeCode { get; set; }
        public int? timeToDefrost { get; set; }
        public string softwareSubRevision { get; set; }
        public string softwareRevision { get; set; }
        public string operatingModeID { get; set; }
        public string modelNumber { get; set; }
        public string oemAlarmBlock { get; set; }
        public string alarmsList { get; set; }
        public string rotationMotion { get; set; }
        public int? rotationDuration { get; set; }
        public double? ventPositionSensorCMH { get; set; }
        public double usdA3Temp { get; set; }
        public double usdA2Temp { get; set; }
        public double usdA1Temp { get; set; }
        public int? totalCurrentDraw { get; set; }
        // public float? tfc { get; set; }
        public float? tc { get; set; }
        public float? t0 { get; set; }
        public double? supplyTemperatureLongAvg { get; set; }
        public double supplyTemp2 { get; set; }
        public double supplyTemp1 { get; set; }
        public double? supplyTemp { get; set; }
        public double? returnTempLongAvg { get; set; }
        public double? returnTemp2 { get; set; }
        public double returnTemp1 { get; set; }
        public double? o2Reading { get; set; }
        public string lowSpeedEvaporatorFanOutPut { get; set; }
        public string highSpeedEvaporatorFanOutPut { get; set; }
        public int lineFrequency { get; set; }
        public double humidity { get; set; }
        public int? hotGasValveOpening { get; set; }
        public int? heaterOnTime { get; set; }
        public double? freshAirType { get; set; }
        public double evaporatorTemp { get; set; }
        public int? evaporatorExpansionValveOpening { get; set; }
        public int economizerValveOpening { get; set; }
        public double dischargePressure { get; set; }
        public double? currentPhaseC { get; set; }
        public double currentPhaseB { get; set; }
        public double currentPhaseA { get; set; }
        public double? condenserPressure { get; set; }
        public string condenserFanOutPut { get; set; }
        public double compressorSuctionTemp { get; set; }
        public int? compressorFrequency { get; set; }
        public double compressorDischargeTemp { get; set; }
        public double compositeSuctionPressure { get; set; }
        public double? cO2Reading { get; set; }
        public double cargo4Temp { get; set; }
        public double? ambientTempLongAvg { get; set; }
        public double ambientTemp { get; set; }
        public double? lineVoltage3 { get; set; }
        public double? lineVoltage2 { get; set; }
        public double? lineVoltage1 { get; set; }
        public string displaySerial { get; set; }
        public string caSerial { get; set; }
        public string pwrSerial { get; set; }
        public string rhSerial { get; set; }
        public string o2Serial { get; set; }
        public string cO2Serial { get; set; }
        public string fcSerial { get; set; }
        public double setPoint { get; set; }
        public double? o2Setpoint { get; set; }
        public double? humiditySetpoint { get; set; }
        public int? defrostInterval { get; set; }
        public double? cO2Setpoint { get; set; }
        public int? shockAmplitude { get; set; }
        public string dataLog { get; set; }
        public string raw { get; set; }
        public double? suctionModulationValveOpening { get; set; }
        public string controllerSerialNumber { get; set; }
        public string faultCodes { get; set; }
        public double? compressorEvaporatorTempLongAvg { get; set; }
        public double? compositeSuctionPressureLongAvg { get; set; }
        public double? compressorSuctionTempLongAvg { get; set; }
        public double? dischargePressureLongAvg { get; set; }
        public double? compressorDischargeTempLongAvg { get; set; }
        public double? compressor2DischargeTemp { get; set; }
        public double? condenserTemp { get; set; }
        public double? condensorTempLongAvg { get; set; }
        public double? powerSupplyDCVoltage { get; set; }
        public string oemModelNumber { get; set; }
        public int pretripSubtestNumber { get; set; }
        public float? internalControllerTemperature { get; set; }
        public float? avgDischargePressure { get; set; }
        public float? avgSuctionTemp { get; set; }
        public float? avgSuctionPressure { get; set; }
        public int? numberOfAlarmsInQue { get; set; }
        public string isoFaultBlock { get; set; }
        public string controlModeID { get; set; }
        public string extendedOperatingMode { get; set; }
        public double? supplyTemp3 { get; set; }
        public bool? caEquipped { get; set; }
        public float? compressorHighSpeed { get; set; }
        public int? combO_HrdwrVer_DfrstIntrvl { get; set; }
        public string o2SetpointSupport { get; set; }
        public string o2ReadingSupport { get; set; }
        public string cO2ReadingSupport { get; set; }
        public string pretripTestStateID { get; set; }
        public string pretripTimeDisplay { get; set; }
        public int? pretripTestNumber { get; set; }
        public int? initiatePreTrip { get; set; }
        public int? terminatePreTrip { get; set; }
        public string tripComment { get; set; }
        public int? combO_OemAlarms_IsoFaults { get; set; }
        public int? combO_EV_CF_HF_HG_LSF { get; set; }
        public int? combO_CondFan_HLEvapFans { get; set; }
        public string tripHeader { get; set; }
        public int? combO_HI_LO_EvapFanOpt { get; set; }
        public string humiditySetpointStatus { get; set; }
        public string microVersion { get; set; }
        public bool? coldTreatmentActive { get; set; }
        public int? highestSeverityLevel { get; set; }
        public double? returnTemp { get; set; }
        public double? returnTemp3 { get; set; }
        public string runningMode { get; set; }
        public double? enterSetpoint { get; set; }
        public double? electricHeatOutPut { get; set; }
        public string hardwareVersion { get; set; }

        public decimal? returnTemperatureLongAvg { get; set; }
    }

    public class DeviceInfo
    {
        public string firmwareVersion { get; set; }
        public string bootloaderVersion { get; set; }
        public string meshFirmwareVersion { get; set; }
        public double reeferACPowerLevel { get; set; }
        public string powerSourceCd { get; set; }
        public double deviceTemp { get; set; }
        public double batteryVoltage { get; set; }
        public string cellularTech { get; set; }
        public int towerBaseStationID { get; set; }
        public int towerLocalizationAreaCode { get; set; }
        public int mcc { get; set; }
        public int mnc { get; set; }
        public int rssi { get; set; }
        public string gpsLockState { get; set; }
        public int isInsideGeofence { get; set; }
        public int gpsLastLock { get; set; }
        public int gpsSatelliteCount { get; set; }
        public double? gpsLongitude { get; set; }
        public double? gpsLatitude { get; set; }
        // public int imei { get; set; }
        public string iccid { get; set; }
        public string msisdn { get; set; }
        public long siM_IMSI { get; set; }
        public long timeOff { get; set; }
        public int commErrorCounter { get; set; }
        public int commDisconnectCounter { get; set; }
        public string networkRat { get; set; }
        public int geofenceRevision { get; set; }
        public string geofenceSiteCd { get; set; }
        public string simPayloadResetReason { get; set; }
        public int reeferClockInSync { get; set; }
        public string equipmentOemType { get; set; }
        public bool isEquipmentPhysicallyConnected { get; set; }
        public bool isEquipmentCommunicationEstablished { get; set; }
        public bool? isAcceptableACVoltage { get; set; }
        public bool eventLogError { get; set; }
        public bool realtimeDownloadOn { get; set; }
        public bool pretripState { get; set; }
        public bool equipmentSwitchOn { get; set; }
        public bool wakeupFromSleep { get; set; }
        public bool rcdIsMoving { get; set; }
        public bool geofenceSuspended { get; set; }
        public bool rfrCommsErr { get; set; }
        public int? rcdBatteryHoldOffInterval { get; set; }
        public int? versionForPC { get; set; }
        public int? versionForRCD { get; set; }
        public int? rssiQualifierID { get; set; }
        public int? rcdReportingInterval { get; set; }
        public bool? isAcceptableBatteryVoltage { get; set; }
        public int? hardwareVersion_D { get; set; }
        public string iS3G { get; set; }
    }

    public class ReeferDeviceTelemetryFeedDto
    {
        public string id { get; set; }
        public string deviceId { get; set; }
        public string deviceType { get; set; }
        public string equipmentId { get; set; }
        public DateTime telemetryDateTime { get; set; }
        public long telemetryDateTimeEpoch { get; set; }
        public DateTime platformReceivedDateTime { get; set; }
        public string telemetryEventType { get; set; }
        public string telemetryEventDetails { get; set; }
        public long serverTime { get; set; }
        public int formatVersion { get; set; }
        public string messageType { get; set; }
        public string equipmentAlertReason { get; set; }
        public EquipmentInfo equipmentInfo { get; set; }
        public DeviceInfo deviceInfo { get; set; }
        public CommandResponse commandResponse { get; set; }
    }

    public class WamUnitTelemetryData
    {
        public string ContinuationToken { get; set; }
        public List<ReeferDeviceTelemetryFeedDto> reeferDeviceTelemetryDto { get; set; }
    }

    public class CommandResponse
    {
        public string correlationId { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }

    public enum ReerferPro
    {
        SupplyTemperature1,
        SupplyTemperature1Qualifier,
        ReturnTemperature1,
        EvaporatorTemperature,
        EvaporatorTemperatureQualifier,
        AmbientTemperature,
        AmbientTemperatureQualifier,
        SupplyTemperature2,
        SupplyTemperature2Qualifier,
        VentPositionSensorCMH,
        VentPositionSensorCMHQualifier,
        Humidity,
        HumidityQualifier,
        ControllerSerialNumber,
        MicroVersion,
        SetPoint,
        SetPointQualifier,
        IsoFaultBlock,
        OperatingMode,
        USDA1Temperature,
        USDA1TemperatureQualifier,
        USDA2Temperature,
        USDA2TemperatureQualifier,
        USDA3Temperature,
        USDA3TemperatureQualifier,
        Cargo4Temperature,
        Cargo4TemperatureQualifier,
        HumiditySetpoint,
        HumiditySetpointStatus,
        SoftwareRevision,
        ReeferType,
        TimeToDefrost,
        SoftwareSubrevision,
        DynValReadCmdRev,
        O2Setpoint,
        O2SetpointSupport,
        O2Reading,
        O2ReadingSupport,
        CO2Setpoint,
        CO2SetpointSupport,
        CO2Reading,
        CO2ReadingSupport,
        OemAlarmBlock,
        SupplyTemperature,
        SupplyTemperatureQualifier,
        CompressorDischargeTemperature,
        CompressorSuctionTemperature,
        CompressorSuctionTemperatureQualifier,
        CompositeSuctionPressure,
        DischargePressure,
        LineVoltage1,
        LineVoltage2,
        LineVoltage3,
        CurrentPhaseA,
        CurrentPhaseB,
        CurrentPhaseC,
        LineFrequency,
        CompressorFrequency,
        CondenserFanOutput,
        HighSpeedEvaporatorFanOutput,
        LowSpeedEvaporatorFanOutput,
        HeaterOnTime,
        EvaporatorExpansionValveOpening,
        HotGasValveOpening,
        EconomizerValveOpening,
        SupplyTemperatureLongAvg,
        SupplyTemperatureLongAvgQualifier,
        ReturnTemperatureLongAvg,
        ReturnTemperatureLongAvgQualifier,
        AmbientTemperatureLongAvg,
        AmbientTemperatureLongAvgQualifier,
        CompressorSuctionTempLongAvg,
        CompressorSuctionTempLongAvgQualifier,
        CompositeSuctionPressureLongAvg,
        DischargePressureLongAvg,
        ContainerID,
        PretripTestState,
        AlarmsList
    }
}
