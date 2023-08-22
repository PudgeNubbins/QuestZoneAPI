using BepInEx;

namespace QuestZoneAPI
{
    [BepInPlugin(Helpers.Constants.pluginGuid, Helpers.Constants.pluginName, Helpers.Constants.pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("Starting");

            new Patches.GameWorldPatch().Enable();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {Helpers.Constants.pluginName} is loaded!");
        }
    }

    public enum ZoneType
    {
        PlaceItem,
        Visit
    }

    public class ZoneTransform
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }

        public ZoneTransform(string x, string y, string z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

    public class ZoneClass
    {
        public string ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string ZoneLocation { get; set; }
        public string ZoneType { get; set; }
        public ZoneType ZoneTypeEnum { get; set; }
        public ZoneTransform Position { get; set; }
        public ZoneTransform Rotation { get; set; } = new ZoneTransform("0", "0", "0");
        public ZoneTransform Scale { get; set; } = new ZoneTransform("1", "1", "1");
    }
}
