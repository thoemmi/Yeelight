using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Thoemmi.Yeelight {
    public static class MessageParser {
        private static readonly char[] _delimiters = "\r\n".ToCharArray();

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
                        device.SupportedCommands = value.Split('_');
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
    }
}