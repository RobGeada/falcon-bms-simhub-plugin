using System;
using System.Windows.Media;
using F4SharedMem;
using GameReaderCommon;
using SimHub.Plugins;

namespace RGeada.FalconBMS
{
    [PluginDescription("Falcon BMS Telemetry")]
    [PluginAuthor("Rob Geada")]
    [PluginName("Falcon BMS Data")]
    public class FalconBMS : IPlugin, IDataPlugin, IWPFSettingsV2
    {

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);

        /// <summary>
        /// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
        /// </summary>
        public string LeftMenuTitle => "Falcon BMS Telemetry";

        private static FlightData flightData;
        private static FlightData oldFlightData;
        private static F4SharedMem.Reader bmsReader;

        /// <summary>
        /// Called one time per game data update, contains all normalized game data,
        /// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        ///
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        ///
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including current and previous data frame.</param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            flightData = bmsReader.GetCurrentData();
            // Define the value of our property (declared in init)
            if (data.GameRunning && bmsReader.IsFalconRunning)
            {
                if (flightData != oldFlightData)
                {
                    this.SetProp("ownship.x", flightData.x);
                    this.SetProp("ownship.y", flightData.y);
                    this.SetProp("ownship.z", flightData.z);
                    this.SetProp("ownship.xDot", flightData.xDot);
                    this.SetProp("ownship.yDot", flightData.yDot);
                    this.SetProp("ownship.zDot", flightData.zDot);
                    this.SetProp("ownship.alpha", flightData.alpha);
                    this.SetProp("ownship.beta", flightData.beta);
                    this.SetProp("ownship.gamma", flightData.gamma);
                    this.SetProp("ownship.pitch", flightData.pitch);
                    this.SetProp("ownship.roll", flightData.roll);
                    this.SetProp("ownship.yaw", flightData.yaw);
                    this.SetProp("ownship.mach", flightData.mach);
                    this.SetProp("ownship.kias", flightData.kias);
                    this.SetProp("ownship.vt", flightData.vt);
                    this.SetProp("ownship.gs", flightData.gs);
                    this.SetProp("ownship.windOffset", flightData.windOffset);
                    this.SetProp("ownship.nozzlePos", flightData.nozzlePos);
                    this.SetProp("ownship.nozzlePos2", flightData.nozzlePos2);
                    this.SetProp("ownship.internalFuel", flightData.internalFuel);
                    this.SetProp("ownship.externalFuel", flightData.externalFuel);
                    this.SetProp("ownship.fuelFlow", flightData.fuelFlow);
                    this.SetProp("ownship.rpm", flightData.rpm);
                    this.SetProp("ownship.rpm2", flightData.rpm2);
                    this.SetProp("ownship.ftit", flightData.ftit);
                    this.SetProp("ownship.ftit2", flightData.ftit2);
                    this.SetProp("ownship.gearPos", flightData.gearPos);
                    this.SetProp("ownship.speedBrake", flightData.speedBrake);
                    this.SetProp("ownship.epuFuel", flightData.epuFuel);
                    this.SetProp("ownship.oilPressure", flightData.oilPressure);
                    this.SetProp("ownship.oilPressure2", flightData.oilPressure2);
                    this.SetProp("ownship.hydPressureA", flightData.hydPressureA);
                    this.SetProp("ownship.hydPressureB", flightData.hydPressureB);
                    this.SetProp("ownship.lightBits", flightData.lightBits);
                    this.SetProp("ownship.lefPos", flightData.lefPos);
                    this.SetProp("ownship.tefPos", flightData.tefPos);
                    this.SetProp("ownship.vtolPos", flightData.vtolPos);
                    this.SetProp("ownship.MainPower", flightData.MainPower);
                    this.SetProp("ownship.bumpIntensity", flightData.bumpIntensity);
                    this.SetProp("ownship.latitude", flightData.latitude);
                    this.SetProp("ownship.longitude", flightData.longitude);
                    this.SetProp("ownship.turnRate", flightData.turnRate);
                    this.SetProp("ownship.vehicleACD", flightData.vehicleACD);
                    this.SetProp("ownship.powerBits", flightData.powerBits);
                    this.SetProp("ownship.blinkBits", flightData.blinkBits);
                    this.SetProp("ownship.cmdsMode", flightData.cmdsMode);
                    this.SetProp("ownship.fuelFlow2", flightData.fuelFlow2);
                    this.SetProp("headInputData.headPitch", flightData.headPitch);
                    this.SetProp("headInputData.headRoll", flightData.headRoll);
                    this.SetProp("headInputData.headYaw", flightData.headYaw);
                    this.SetProp("lights.lightBits2", flightData.lightBits2);
                    this.SetProp("lights.lightBits3", flightData.lightBits3);
                    this.SetProp("chaffFlare.ChaffCount", flightData.ChaffCount);
                    this.SetProp("chaffFlare.FlareCount", flightData.FlareCount);
                    this.SetProp("landingGear.NoseGearPos", flightData.NoseGearPos);
                    this.SetProp("landingGear.LeftGearPos", flightData.LeftGearPos);
                    this.SetProp("landingGear.RightGearPos", flightData.RightGearPos);
                    this.SetProp("ADI_Values.AdiIlsHorPos", flightData.AdiIlsHorPos);
                    this.SetProp("ADI_Values.AdiIlsVerPos", flightData.AdiIlsVerPos);
                    this.SetProp("HSI_States.courseState", flightData.courseState);
                    this.SetProp("HSI_States.headingState", flightData.headingState);
                    this.SetProp("HSI_States.totalStates", flightData.totalStates);
                    this.SetProp("HSI_Values.courseDeviation", flightData.courseDeviation);
                    this.SetProp("HSI_Values.desiredCourse", flightData.desiredCourse);
                    this.SetProp("HSI_Values.distanceToBeacon", flightData.distanceToBeacon);
                    this.SetProp("HSI_Values.bearingToBeacon", flightData.bearingToBeacon);
                    this.SetProp("HSI_Values.currentHeading", flightData.currentHeading);
                    this.SetProp("HSI_Values.desiredHeading", flightData.desiredHeading);
                    this.SetProp("HSI_Values.deviationLimit", flightData.deviationLimit);
                    this.SetProp("HSI_Values.halfDeviationLimit", flightData.halfDeviationLimit);
                    this.SetProp("HSI_Values.localizerCourse", flightData.localizerCourse);
                    this.SetProp("HSI_Values.airbaseX", flightData.airbaseX);
                    this.SetProp("HSI_Values.airbaseY", flightData.airbaseY);
                    this.SetProp("HSI_Values.totalValues", flightData.totalValues);
                    this.SetProp("trim.TrimPitch", flightData.TrimPitch);
                    this.SetProp("trim.TrimRoll", flightData.TrimRoll);
                    this.SetProp("trim.TrimYaw", flightData.TrimYaw);
                    this.SetProp("HSI_Flags.hsiBits", flightData.hsiBits);
                    this.SetProp("DED_Lines.DEDLines", flightData.DEDLines);
                    this.SetProp("DED_Lines.Invert", flightData.Invert);
                    this.SetProp("PFL_Lines.PFLLines", flightData.PFLLines);
                    this.SetProp("PFL_Lines.PFLInvert", flightData.PFLInvert);
                    this.SetProp("tacan.UFCTChan", flightData.UFCTChan);
                    this.SetProp("tacan.AUXTChan", flightData.AUXTChan);
                    this.SetProp("tacan.UfcTacanIsAA", flightData.UfcTacanIsAA);
                    this.SetProp("tacan.AuxTacanIsAA", flightData.AuxTacanIsAA);
                    this.SetProp("tacan.UfcTacanIsX", flightData.UfcTacanIsX);
                    this.SetProp("tacan.AuxTacanIsX", flightData.AuxTacanIsX);
                    this.SetProp("tacan.tacanInfo", flightData.tacanInfo);
                    this.SetProp("RWR.RwrObjectCount", flightData.RwrObjectCount);
                    this.SetProp("RWR.RWRsymbol", flightData.RWRsymbol);
                    this.SetProp("RWR.bearing", flightData.bearing);
                    this.SetProp("RWR.missileActivity", flightData.missileActivity);
                    this.SetProp("RWR.missileLaunch", flightData.missileLaunch);
                    this.SetProp("RWR.selected", flightData.selected);
                    this.SetProp("RWR.lethality", flightData.lethality);
                    this.SetProp("RWR.newDetection", flightData.newDetection);
                    this.SetProp("RWR.RwrInfo", flightData.RwrInfo);
                    this.SetProp("fuel.fwd", flightData.fwd);
                    this.SetProp("fuel.aft", flightData.aft);
                    this.SetProp("fuel.total", flightData.total);
                    this.SetProp("fuel.VersionNum", flightData.VersionNum);
                    this.SetProp("fuel.VersionNum2", flightData.VersionNum2);
                    this.SetProp("headPosition.headX", flightData.headX);
                    this.SetProp("headPosition.headY", flightData.headY);
                    this.SetProp("headPosition.headZ", flightData.headZ);
                    this.SetProp("navigationBMS4.navMode", flightData.navMode);
                    this.SetProp("navigationBMS4.aauz", flightData.aauz);
                    this.SetProp("navigationBMS4.AltCalReading", flightData.AltCalReading);
                    this.SetProp("navigationBMS4.altBits", flightData.altBits);
                    this.SetProp("navigationBMS4.cabinAlt", flightData.cabinAlt);
                    this.SetProp("BUP.BupUhfPreset", flightData.BupUhfPreset);
                    this.SetProp("BUP.BupUhfFreq", flightData.BupUhfFreq);
                    this.SetProp("time.currentTime", flightData.currentTime);
                    this.SetProp("pilotInfo.pilotsOnline", flightData.pilotsOnline);
                    this.SetProp("pilotInfo.pilotsCallsign", flightData.pilotsCallsign);
                    this.SetProp("pilotInfo.pilotsStatus", flightData.pilotsStatus);
                    this.SetProp("VERSION_12.RTT_size", flightData.RTT_size);
                    this.SetProp("VERSION_12.RTT_area", flightData.RTT_area);
                    this.SetProp("VERSION_13.iffBackupMode1Digit1", flightData.iffBackupMode1Digit1);
                    this.SetProp("VERSION_13.iffBackupMode1Digit2", flightData.iffBackupMode1Digit2);
                    this.SetProp("VERSION_13.iffBackupMode3ADigit1", flightData.iffBackupMode3ADigit1);
                    this.SetProp("VERSION_13.iffBackupMode3ADigit2", flightData.iffBackupMode3ADigit2);
                    this.SetProp("VERSION_14.instrLight", flightData.instrLight);
                    this.SetProp("VERSION_15.bettyBits", flightData.bettyBits);
                    this.SetProp("VERSION_15.miscBits", flightData.miscBits);
                    this.SetProp("VERSION_15.RALT", flightData.RALT);
                    this.SetProp("VERSION_15.bingoFuel", flightData.bingoFuel);
                    this.SetProp("VERSION_15.caraAlow", flightData.caraAlow);
                    this.SetProp("VERSION_15.bullseyeX", flightData.bullseyeX);
                    this.SetProp("VERSION_15.bullseyeY", flightData.bullseyeY);
                    this.SetProp("VERSION_15.BMSVersionMajor", flightData.BMSVersionMajor);
                    this.SetProp("VERSION_15.BMSVersionMinor", flightData.BMSVersionMinor);
                    this.SetProp("VERSION_15.BMSVersionMicro", flightData.BMSVersionMicro);
                    this.SetProp("VERSION_15.BMSBuildNumber", flightData.BMSBuildNumber);
                    this.SetProp("SharedMemoryInfo.StringAreaSize", flightData.StringAreaSize);
                    this.SetProp("SharedMemoryInfo.StringAreaTime", flightData.StringAreaTime);
                    this.SetProp("SharedMemoryInfo.DrawingAreaSize//", flightData.DrawingAreaSize);

                    this.SetProp("magneticDeviation.magDeviationSystem", flightData.magDeviationSystem);
                    this.SetProp("magneticDeviation.magDeviationReal", flightData.magDeviationReal);
                    this.SetProp("magneticDeviation.ecmBits", flightData.ecmBits);

                    // intellivibe
                    this.SetProp("IntelliVibe.AAMissileFired", flightData.IntellivibeData.AAMissileFired);
                    this.SetProp("IntelliVibe.AGMissileFired", flightData.IntellivibeData.AGMissileFired);
                    this.SetProp("IntelliVibe.BombDropped", flightData.IntellivibeData.BombDropped);
                    this.SetProp("IntelliVibe.FlareDropped", flightData.IntellivibeData.FlareDropped);
                    this.SetProp("IntelliVibe.ChaffDropped", flightData.IntellivibeData.ChaffDropped);
                    this.SetProp("IntelliVibe.BulletsFired", flightData.IntellivibeData.BulletsFired);
                    this.SetProp("IntelliVibe.CollisionCounter", flightData.IntellivibeData.CollisionCounter);
                    this.SetProp("IntelliVibe.IsFiringGun", flightData.IntellivibeData.IsFiringGun);
                    this.SetProp("IntelliVibe.IsEndFlight", flightData.IntellivibeData.IsEndFlight);
                    this.SetProp("IntelliVibe.IsEjecting", flightData.IntellivibeData.IsEjecting);
                    this.SetProp("IntelliVibe.In3D", flightData.IntellivibeData.In3D);
                    this.SetProp("IntelliVibe.IsPaused", flightData.IntellivibeData.IsPaused);
                    this.SetProp("IntelliVibe.IsFrozen", flightData.IntellivibeData.IsFrozen);
                    this.SetProp("IntelliVibe.IsOverG", flightData.IntellivibeData.IsOverG);
                    this.SetProp("IntelliVibe.IsOnGround", flightData.IntellivibeData.IsOnGround);
                    this.SetProp("IntelliVibe.IsExitGame", flightData.IntellivibeData.IsExitGame);
                    this.SetProp("IntelliVibe.Gforce", flightData.IntellivibeData.Gforce);
                    this.SetProp("IntelliVibe.eyex", flightData.IntellivibeData.eyex);
                    this.SetProp("IntelliVibe.eyey", flightData.IntellivibeData.eyey);
                    this.SetProp("IntelliVibe.eyez", flightData.IntellivibeData.eyez);
                    this.SetProp("IntelliVibe.lastdamage", flightData.IntellivibeData.lastdamage);
                    this.SetProp("IntelliVibe.damageforce", flightData.IntellivibeData.damageforce);
                    this.SetProp("IntelliVibe.whendamage", flightData.IntellivibeData.whendamage);

                    bool notInPlane = flightData.IntellivibeData.IsPaused || flightData.IntellivibeData.IsFrozen || flightData.IntellivibeData.IsExitGame || flightData.IntellivibeData.IsEndFlight || !flightData.IntellivibeData.In3D;
                    this.AddProp("Utility.stopEffects", notInPlane && !bmsReader.IsFalconRunning);
                }
                oldFlightData = flightData;
            }
        }

        /// <summary>
        /// Called at plugin manager stop, close/dispose anything needed here !
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            // Save settings

        }

        /// <summary>
        /// Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return null;
        }

        /// <summary>
        /// Called once after plugins startup
        /// Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting FalconBMS plugin");

            bmsReader = new Reader();
            flightData = bmsReader.GetCurrentData();
            oldFlightData = flightData;

            this.AddProp("ownship.x", 0.0);
            this.AddProp("ownship.y", 0.0);
            this.AddProp("ownship.z", 0.0);
            this.AddProp("ownship.xDot", 0.0);
            this.AddProp("ownship.yDot", 0.0);
            this.AddProp("ownship.zDot", 0.0);
            this.AddProp("ownship.alpha", 0.0);
            this.AddProp("ownship.beta", 0.0);
            this.AddProp("ownship.gamma", 0.0);
            this.AddProp("ownship.pitch", 0.0);
            this.AddProp("ownship.roll", 0.0);
            this.AddProp("ownship.yaw", 0.0);
            this.AddProp("ownship.mach", 0.0);
            this.AddProp("ownship.kias", 0.0);
            this.AddProp("ownship.vt", 0.0);
            this.AddProp("ownship.gs", 0.0);
            this.AddProp("ownship.windOffset", 0.0);
            this.AddProp("ownship.nozzlePos", 0.0);
            this.AddProp("ownship.nozzlePos2", 0.0);
            this.AddProp("ownship.internalFuel", 0.0);
            this.AddProp("ownship.externalFuel", 0.0);
            this.AddProp("ownship.fuelFlow", 0.0);
            this.AddProp("ownship.rpm", 0.0);
            this.AddProp("ownship.rpm2", 0.0);
            this.AddProp("ownship.ftit", 0.0);
            this.AddProp("ownship.ftit2", 0.0);
            this.AddProp("ownship.gearPos", 0.0);
            this.AddProp("ownship.speedBrake", 0.0);
            this.AddProp("ownship.epuFuel", 0.0);
            this.AddProp("ownship.oilPressure", 0.0);
            this.AddProp("ownship.oilPressure2", 0.0);
            this.AddProp("ownship.hydPressureA", 0.0);
            this.AddProp("ownship.hydPressureB", 0.0);
            this.AddProp("ownship.lightBits", 0);
            this.AddProp("ownship.lefPos", 0.0);
            this.AddProp("ownship.tefPos", 0.0);
            this.AddProp("ownship.vtolPos", 0.0);
            this.AddProp("ownship.MainPower", 0);
            this.AddProp("ownship.bumpIntensity", 0.0);
            this.AddProp("ownship.latitude", 0.0);
            this.AddProp("ownship.longitude", 0.0);
            this.AddProp("ownship.turnRate", 0.0);
            this.AddProp("ownship.vehicleACD", 0);
            this.AddProp("ownship.powerBits", 0);
            this.AddProp("ownship.blinkBits", 0);
            this.AddProp("ownship.cmdsMode", 0);
            this.AddProp("ownship.fuelFlow2", 0.0);
            this.AddProp("headInputData.headPitch", 0.0);
            this.AddProp("headInputData.headRoll", 0.0);
            this.AddProp("headInputData.headYaw", 0.0);
            this.AddProp("lights.lightBits2", 0);
            this.AddProp("lights.lightBits3", 0);
            this.AddProp("chaffFlare.ChaffCount", 0.0);
            this.AddProp("chaffFlare.FlareCount", 0.0);
            this.AddProp("landingGear.NoseGearPos", 0.0);
            this.AddProp("landingGear.LeftGearPos", 0.0);
            this.AddProp("landingGear.RightGearPos", 0.0);
            this.AddProp("ADI_Values.AdiIlsHorPos", 0.0);
            this.AddProp("ADI_Values.AdiIlsVerPos", 0.0);
            this.AddProp("HSI_States.courseState", 0);
            this.AddProp("HSI_States.headingState", 0);
            this.AddProp("HSI_States.totalStates", 0);
            this.AddProp("HSI_Values.courseDeviation", 0.0);
            this.AddProp("HSI_Values.desiredCourse", 0.0);
            this.AddProp("HSI_Values.distanceToBeacon", 0.0);
            this.AddProp("HSI_Values.bearingToBeacon", 0.0);
            this.AddProp("HSI_Values.currentHeading", 0.0);
            this.AddProp("HSI_Values.desiredHeading", 0.0);
            this.AddProp("HSI_Values.deviationLimit", 0.0);
            this.AddProp("HSI_Values.halfDeviationLimit", 0.0);
            this.AddProp("HSI_Values.localizerCourse", 0.0);
            this.AddProp("HSI_Values.airbaseX", 0.0);
            this.AddProp("HSI_Values.airbaseY", 0.0);
            this.AddProp("HSI_Values.totalValues", 0.0);
            this.AddProp("trim.TrimPitch", 0.0);
            this.AddProp("trim.TrimRoll", 0.0);
            this.AddProp("trim.TrimYaw", 0.0);
            this.AddProp("HSI_Flags.hsiBits", 0);
            this.AddProp("DED_Lines.DEDLines", new string[0]);
            this.AddProp("DED_Lines.Invert", new string[0]);
            this.AddProp("PFL_Lines.PFLLines", new string[0]);
            this.AddProp("PFL_Lines.PFLInvert", new string[0]);
            this.AddProp("tacan.UFCTChan", 0);
            this.AddProp("tacan.AUXTChan", 0);
            this.AddProp("tacan.UfcTacanIsAA", false);
            this.AddProp("tacan.AuxTacanIsAA", false);
            this.AddProp("tacan.UfcTacanIsX", false);
            this.AddProp("tacan.AuxTacanIsX", false);
            this.AddProp("tacan.tacanInfo", new byte[0]);
            this.AddProp("RWR.RwrObjectCount", 0);
            this.AddProp("RWR.RWRsymbol", new int[0]);
            this.AddProp("RWR.bearing", new float[0]);
            this.AddProp("RWR.missileActivity", new uint[0]);
            this.AddProp("RWR.missileLaunch", new uint[0]);
            this.AddProp("RWR.selected", new uint[0]);
            this.AddProp("RWR.lethality", new float[0]);
            this.AddProp("RWR.newDetection", new uint[0]);
            this.AddProp("RWR.RwrInfo", new byte[0]);
            this.AddProp("fuel.fwd", 0.0);
            this.AddProp("fuel.aft", 0.0);
            this.AddProp("fuel.total", 0.0);
            this.AddProp("fuel.VersionNum", 0);
            this.AddProp("fuel.VersionNum2", 0);
            this.AddProp("headPosition.headX", 0.0);
            this.AddProp("headPosition.headY", 0.0);
            this.AddProp("headPosition.headZ", 0.0);
            this.AddProp("navigationBMS4.navMode", 0);
            this.AddProp("navigationBMS4.aauz", 0.0);
            this.AddProp("navigationBMS4.AltCalReading", 0);
            this.AddProp("navigationBMS4.altBits", 0);
            this.AddProp("navigationBMS4.cabinAlt", 0.0);
            this.AddProp("BUP.BupUhfPreset", 0);
            this.AddProp("BUP.BupUhfFreq", 0);
            this.AddProp("time.currentTime", 0);
            this.AddProp("pilotInfo.pilotsOnline", 0);
            this.AddProp("pilotInfo.pilotsCallsign", new string[0]);
            this.AddProp("pilotInfo.pilotsStatus", new byte[0]);
            this.AddProp("VERSION_12.RTT_size", new ushort[0]);
            this.AddProp("VERSION_12.RTT_area", new ushort[0]);
            this.AddProp("VERSION_13.iffBackupMode1Digit1", 0);
            this.AddProp("VERSION_13.iffBackupMode1Digit2", 0);
            this.AddProp("VERSION_13.iffBackupMode3ADigit1", 0);
            this.AddProp("VERSION_13.iffBackupMode3ADigit2", 0);
            this.AddProp("VERSION_14.instrLight", 0);
            this.AddProp("VERSION_15.bettyBits", 0);
            this.AddProp("VERSION_15.miscBits", 0);
            this.AddProp("VERSION_15.RALT", 0.0);
            this.AddProp("VERSION_15.bingoFuel", 0.0);
            this.AddProp("VERSION_15.caraAlow", 0.0);
            this.AddProp("VERSION_15.bullseyeX", 0.0);
            this.AddProp("VERSION_15.bullseyeY", 0.0);
            this.AddProp("VERSION_15.BMSVersionMajor", 0);
            this.AddProp("VERSION_15.BMSVersionMinor", 0);
            this.AddProp("VERSION_15.BMSVersionMicro", 0);
            this.AddProp("VERSION_15.BMSBuildNumber", 0);
            this.AddProp("SharedMemoryInfo.StringAreaSize", 0);
            this.AddProp("SharedMemoryInfo.StringAreaTime", 0);
            this.AddProp("SharedMemoryInfo.DrawingAreaSize//", 0);
            this.AddProp("magneticDeviation.magDeviationSystem", 0.0);
            this.AddProp("magneticDeviation.magDeviationReal", 0.0);
            this.AddProp("magneticDeviation.ecmBits", new uint[0]);
            this.AddProp("Radio.radio2_preset", 0);
            this.AddProp("Radio.radio2_frequency", 0);
            this.AddProp("IFF_Transponder.iffTransponderActiveCode2", 0);
            this.AddProp("IFF_Transponder.iffTransponderActiveCode3A", 0);
            this.AddProp("IFF_Transponder.iffTransponderActiveCodeC", 0);
            this.AddProp("IFF_Transponder.iffTransponderActiveCode4", 0);
            this.AddProp("VERSION_18.EWMULines", new string[0]);
            this.AddProp("VERSION_18.EWPILines", new string[0]);

            this.AddProp("IntelliVibe.AAMissileFired", 0);
            this.AddProp("IntelliVibe.AGMissileFired", 0);
            this.AddProp("IntelliVibe.BombDropped", 0);
            this.AddProp("IntelliVibe.FlareDropped", 0);
            this.AddProp("IntelliVibe.ChaffDropped", 0);
            this.AddProp("IntelliVibe.BulletsFired", 0);
            this.AddProp("IntelliVibe.CollisionCounter", 0);
            this.AddProp("IntelliVibe.IsFiringGun", false);
            this.AddProp("IntelliVibe.IsEndFlight", false);
            this.AddProp("IntelliVibe.IsEjecting", false);
            this.AddProp("IntelliVibe.In3D", false);
            this.AddProp("IntelliVibe.IsPaused", false);
            this.AddProp("IntelliVibe.IsFrozen", false);
            this.AddProp("IntelliVibe.IsOverG", false);
            this.AddProp("IntelliVibe.IsOnGround", false);
            this.AddProp("IntelliVibe.IsExitGame", false);
            this.AddProp("IntelliVibe.Gforce", 0.0);
            this.AddProp("IntelliVibe.eyex", 0.0);
            this.AddProp("IntelliVibe.eyey", 0.0);
            this.AddProp("IntelliVibe.eyez", 0.0);
            this.AddProp("IntelliVibe.lastdamage", 0);
            this.AddProp("IntelliVibe.damageforce", 0.0);
            this.AddProp("IntelliVibe.whendamage", 0);

            this.AddProp("Utility.inGame", false);

        }

        public void AddProp(string PropertyName, dynamic defaultValue) => PluginManager.AddProperty(PropertyName, GetType(), defaultValue);
        public void SetProp(string PropertyName, dynamic value) => PluginManager.SetPropertyValue(PropertyName, GetType(), value);
        public dynamic GetProp(string PropertyName) => PluginManager.GetPropertyValue("DataCorePlugin.GameRawData." + PropertyName);
    }
}