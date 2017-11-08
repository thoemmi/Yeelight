using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Thoemmi.Yeelight {
    /// <summary>
    /// Helper class to parse messages sent by a device.
    /// </summary>
    public static class MessageParser {
        private static readonly char[] _delimiters = "\r\n".ToCharArray();

        /// <summary>
        /// Parses a message received from a device.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <returns>A tuple specifiying the reason of the message and information about the sending device.</returns>
        public static (Reason reason, DeviceInformation device) Parse(string message) {
            string[] lines = message.Split(_delimiters, StringSplitOptions.RemoveEmptyEntries);
            Reason reason;
            switch (lines[0]) {
                case "HTTP/1.1 200 OK":
                    reason = Reason.Discovery;
                    break;
                case "NOTIFY * HTTP/1.1":
                    reason = Reason.Advertisement;
                    break;
                default:
                    throw new ProtocolViolationException();
            }

            var device = new DeviceInformation();
            foreach (var line in lines.Skip(1)) {
                var pos = line.IndexOf(':');
                Debug.Assert(pos > 0);
                var key = line.Substring(0, pos).Trim();
                var value = line.Substring(pos + 1).Trim();

                switch (key) {
                    case "Cache-Control":
                        var s = value.Substring(value.IndexOf('=') + 1);
                        device.RefreshInterval = TimeSpan.FromSeconds(int.Parse(s));
                        break;
                    case "Location":
                        device.EndPoint = new Uri(value);
                        break;
                    case "id":
                        device.Id = value;
                        break;
                    case "model":
                        device.Model = value;
                        break;
                    case "fw_ver":
                        device.FirmwareVersion = value;
                        break;
                    case "support":
                        device.Capabilities = DecodeCapabilities(value);
                        break;
                    case "power":
                        device.PoweredOn = value == "on";
                        break;
                    case "bright":
                        device.Brightness = int.Parse(value);
                        break;
                    case "color_mode":
                        device.LightMode = (LightMode)int.Parse(value);
                        break;
                    case "ct":
                        device.ColorTemperature = int.Parse(value);
                        break;
                    case "rgb":
                        device.RGB = int.Parse(value);
                        break;
                    case "hue":
                        device.Hue = int.Parse(value);
                        break;
                    case "sat":
                        device.Saturation = int.Parse(value);
                        break;
                    case "name":
                        device.Name = value;
                        break;
                    case "Date":
                    case "Ext":
                    case "Server":
                    case "Host":
                    case "NTS":
                        break;
                    default:
                        throw new ProtocolViolationException($"Unknown key {key}");
                }
            }

            return (reason, device);
        }

        private static DeviceCapabilities DecodeCapabilities(string value) {
            var capabilities = DeviceCapabilities.None;
            foreach (var cap in value.Split(' ')) {
                switch (cap) {
                    case "get_prop":
                        capabilities |= DeviceCapabilities.GetProperties;
                        break;
                    case "set_ct_abx":
                        capabilities |= DeviceCapabilities.SetColorTemperature;
                        break;
                    case "set_rgb":
                        capabilities |= DeviceCapabilities.SetRGB;
                        break;
                    case "set_hsv":
                        capabilities |= DeviceCapabilities.SetHSV;
                        break;
                    case "set_bright":
                        capabilities |= DeviceCapabilities.SetBrightness;
                        break;
                    case "set_power":
                        capabilities |= DeviceCapabilities.SetPower;
                        break;
                    case "toggle":
                        capabilities |= DeviceCapabilities.Toggle;
                        break;
                    case "set_default":
                        capabilities |= DeviceCapabilities.SetDefault;
                        break;
                    case "start_cf":
                        capabilities |= DeviceCapabilities.StartColorFlow;
                        break;
                    case "stop_cf":
                        capabilities |= DeviceCapabilities.StopColorFlow;
                        break;
                    case "set_scene":
                        capabilities |= DeviceCapabilities.SetScene;
                        break;
                    case "cron_add":
                        capabilities |= DeviceCapabilities.StartTimer;
                        break;
                    case "cron_get":
                        capabilities |= DeviceCapabilities.GetTimer;
                        break;
                    case "cron_del":
                        capabilities |= DeviceCapabilities.DeleteTimer;
                        break;
                    case "set_adjust":
                        capabilities |= DeviceCapabilities.SetAdjust;
                        break;
                    case "set_music":
                        capabilities |= DeviceCapabilities.SetMusic;
                        break;
                    case "set_name":
                        capabilities |= DeviceCapabilities.SetName;
                        break;
                    case "dev_toggle":
                        capabilities |= DeviceCapabilities.DevToggle;
                        break;
                    default:
                        throw new ArgumentException($"Unsupported capability {cap}");
                }
            }
            return capabilities;
        }
    }
}