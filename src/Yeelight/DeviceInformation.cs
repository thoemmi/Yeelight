using System;

namespace Yeelight {
    public class DeviceInformation {
        public TimeSpan RefreshInterval { get; set; }
        public Uri EndPoint { get; set; }
        public string Id { get; set; }
        public string Model { get; set; }
        public string FirmwareVersion { get; set; }
        public string[] SupportedCommands { get; set; }
        public bool PoweredOn { get; set; }
        public int Brightness { get; set; }
        public LightMode LightMode { get; set; }
        public int ColorTemperature { get; set; }
        public int RGB { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public string Name { get; set; }
    }
}