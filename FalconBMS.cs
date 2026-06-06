using System;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;
using F4SharedMem;
using GameReaderCommon;
using SimHub.Plugins;
using System.Net.NetworkInformation;
using SimHub.Plugins.Devices.Registry.Impl.TurtleBeach.Packets;
using F4SharedMem.Headers;

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


        private object GetDefaultValue(Type t)
        {
            if (t == typeof(string)) return "";
            if (t == typeof(bool)) return false;
            if (t == typeof(int)) return 0;
            if (t == typeof(uint)) return (uint)0;
            if (t == typeof(long)) return 0L;
            if (t == typeof(ulong)) return (ulong)0;
            if (t == typeof(float)) return 0f;
            if (t == typeof(double)) return 0.0;
            if (t == typeof(decimal)) return 0m;
            if (t.IsValueType) return Activator.CreateInstance(t);

            return "";
        }

        // Generic helper for any enum + uint bitfield
        void ExportEnum<T>(string prefix, uint value, bool declare) where T : Enum
        {
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                // Example property: lights.MasterCaution = true
                if (declare)
                {
                    this.AddProp($"{prefix}.{name}", false);
                } else
                {
                    var enumValue = (uint)Convert.ToUInt32(Enum.Parse(typeof(T), name));
                    bool isSet = (value & enumValue) != 0;
                    this.SetProp($"{prefix}.{name}", isSet);
                }

            }
        }

        void ExportSimpleFields(string prefix, object obj)
        {
            foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var type = field.FieldType;

                // Skip nested structs
                if (type.IsValueType && !type.IsPrimitive && !type.IsEnum)
                    continue;

                // Skip enums (lightbits, powerbits, blinkbits—handled separately)
                if (type.IsEnum)
                    continue;

                this.SetProp($"{prefix}.{field.Name}", field.GetValue(obj));
            }
        }

        void DeclareSimpleFields(string prefix, Type t)
        {
            foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                Type type = field.FieldType;

                // Skip nested structs
                if (type.IsValueType && !type.IsPrimitive && !type.IsEnum)
                    continue;

                // Skip enums (lightbits, powerbits, blinkbits—handled separately)
                if (type.IsEnum)
                    continue;

                // Declare property *once* with an appropriate default
                this.AddProp($"{prefix}.{field.Name}", GetDefaultValue(type));
            }
        }

        private void ExportStringData(F4SharedMem.Headers.StringData sd)
        {
            foreach (var entry in sd.data)
            {
                // Try to cast the numeric strId to the enum
                string propName;
                if (Enum.IsDefined(typeof(F4SharedMem.Headers.StringIdentifier), entry.strId))
                {
                    var idEnum = (F4SharedMem.Headers.StringIdentifier)entry.strId;
                    propName = $"stringdata.{idEnum}";
                }
                else
                {
                    // fallback: unknown string ID
                    propName = $"stringdata.Unknown_{entry.strId}";
                }


                var value = System.Text.Encoding.ASCII.GetString(entry.strData).TrimEnd('\0');
                this.SetProp(propName, value);
            }
        }

        private void DeclareStringProps()
        {
            foreach (var value in Enum.GetValues(typeof(F4SharedMem.Headers.StringIdentifier)))
            {
                var id = (F4SharedMem.Headers.StringIdentifier)value;

                // Skip the special _DIM / sentinel entry if present
                if (id.ToString().Contains("DIM"))
                    continue;

                // Property name in SimHub
                string propName = $"stringdata.{id}";

                // Initialize with empty string
                this.AddProp(propName, "");
            }
        }



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
                    ExportSimpleFields("flightData", flightData);
                    ExportSimpleFields("IntelliVibe", flightData.IntellivibeData);

                    // bits
                    ExportEnum<F4SharedMem.Headers.AltBits>("alt", flightData.altBits, false);
                    ExportEnum<F4SharedMem.Headers.PowerBits>("power", flightData.powerBits, false);
                    ExportEnum<F4SharedMem.Headers.BlinkBits>("blink", flightData.blinkBits, false);
                    ExportEnum<F4SharedMem.Headers.BettyBits>("betty", flightData.bettyBits, false);
                    ExportEnum<F4SharedMem.Headers.MiscBits>("misc", flightData.miscBits, false);
                    ExportEnum<F4SharedMem.Headers.LightBits>("lights1", flightData.lightBits, false);
                    ExportEnum<F4SharedMem.Headers.LightBits2>("lights2", flightData.lightBits2, false);
                    ExportEnum<F4SharedMem.Headers.LightBits3>("lights3", flightData.lightBits3, false);
                    ExportEnum<F4SharedMem.Headers.HsiBits>("hsi", flightData.hsiBits, false);


                    this.AddProp("ecm", flightData.ecmBits);


                    // string data
                    ExportStringData(flightData.StringData);

                    // computed
                    bool notInPlane = flightData.IntellivibeData.IsPaused || flightData.IntellivibeData.IsFrozen || flightData.IntellivibeData.IsExitGame || flightData.IntellivibeData.IsEndFlight || !flightData.IntellivibeData.In3D;
                    bool noData = flightData.latitude == 0 && flightData.longitude == 0 && flightData.currentTime == 0 && flightData.vehicleACD == 0;
                    this.AddProp("Utility.stopEffects", notInPlane || !bmsReader.IsFalconRunning || noData);
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

            DeclareSimpleFields("flightData", typeof(FlightData));
            DeclareSimpleFields("IntelliVibe", typeof(F4SharedMem.Headers.IntellivibeData));
            ExportEnum<F4SharedMem.Headers.AltBits>("alt", 0, true);
            ExportEnum<F4SharedMem.Headers.PowerBits>("power", 0, true);
            ExportEnum<F4SharedMem.Headers.BlinkBits>("blink", 0, true);
            ExportEnum<F4SharedMem.Headers.BettyBits>("betty", 0, true);
            ExportEnum<F4SharedMem.Headers.MiscBits>("misc", 0, true);
            ExportEnum<F4SharedMem.Headers.LightBits>("lights1", 0, true);
            ExportEnum<F4SharedMem.Headers.LightBits2>("lights2", 0, true);
            ExportEnum<F4SharedMem.Headers.LightBits3>("lights3", 0, true);
            ExportEnum<F4SharedMem.Headers.HsiBits>("hsi", 0, true);

            DeclareStringProps();

            this.AddProp("ecm", 0);
            this.AddProp("FlyState.flystate", 0x0);
            this.AddProp("Utility.stopEffects", true);

        }

        public void AddProp(string PropertyName, dynamic defaultValue) => PluginManager.AddProperty(PropertyName, GetType(), defaultValue);
        public void SetProp(string PropertyName, dynamic value) => PluginManager.SetPropertyValue(PropertyName, GetType(), value);
        public dynamic GetProp(string PropertyName) => PluginManager.GetPropertyValue("DataCorePlugin.GameRawData." + PropertyName);
    }
}