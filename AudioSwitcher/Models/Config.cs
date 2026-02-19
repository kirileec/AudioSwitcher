namespace AudioSwitcher.Models
{
    public class DeviceConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AppConfig
    {
        public DeviceConfig Device1 { get; set; }
        public DeviceConfig Device2 { get; set; }
        public bool AutoStart { get; set; }
    }
}
