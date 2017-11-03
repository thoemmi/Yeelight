using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Yeelight {
    public class DeviceListener {
        private const int Port = 1982;

        private const string SsdpMessage = "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1982\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb";

        private static readonly IPEndPoint _anyEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private static readonly IPEndPoint _localEndPoint = new IPEndPoint(GetLocalIPAddress(), 0);
        private static readonly IPEndPoint _multicastEndPoint = new IPEndPoint(IPAddress.Parse("239.255.255.250"), Port);
        private static readonly byte[] _dgram = Encoding.ASCII.GetBytes(SsdpMessage);

        private static readonly char[] _delimiters = "\r\n".ToCharArray();


        private Socket _ssdpSocket;

        public async Task StartListening() {
            // create socket
            _ssdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {
                Blocking = false,
                Ttl = 1,
                UseOnlyOverlappedIO = true,
                MulticastLoopback = false
            };

            var localIpAddress = GetLocalIPAddress();

            Console.WriteLine("Mon ip: " + localIpAddress);

            _ssdpSocket.Bind(new IPEndPoint(localIpAddress, 0));

            _ssdpSocket.SetSocketOption(
                SocketOptionLevel.IP,
                SocketOptionName.AddMembership,
                new MulticastOption(_multicastEndPoint.Address)
            );

            // start listening
#pragma warning disable 4014
            Listen();
#pragma warning restore 4014

            await _ssdpSocket.SendToAsync(_dgram, 0, _dgram.Length, SocketFlags.None, _multicastEndPoint);
        }

        private async Task Listen() {
            var buffer = new ArraySegment<byte>(new byte[4096]);

            SocketReceiveFromResult result;
            while ((result = await _ssdpSocket.ReceiveFromAsync(buffer, SocketFlags.None, _anyEndPoint)).ReceivedBytes > 0) {
                var message = Encoding.ASCII.GetString(buffer.Array, 0, result.ReceivedBytes);
                HandleResponse(result.RemoteEndPoint, message);
            }
        }

        private void HandleResponse(EndPoint sender, string response) {
            string[] lines = response.Split(_delimiters, StringSplitOptions.RemoveEmptyEntries);
            Reason reason;
            if (lines[0] == "HTTP/1.1 200 OK") {
                reason = Reason.Discovery;
            } else if (lines[0] == "NOTIFY * HTTP/1.1") {
                reason = Reason.Advertisement;
            } else {
                throw new ProtocolViolationException();
            }

            var device = new DeviceInformation();
            foreach (var line in lines.Skip(1)) {
                var pos = line.IndexOf(':');
                if (pos <= 0) {
                    continue;
                }

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
                        break;
                    default:
                        throw new ProtocolViolationException($"Unknown key {key}");
                }
            }

            DeviceInformationReceived?.Invoke(this, new DeviceInformationReceivedEventArgs(reason, device));
        }

        public event EventHandler<DeviceInformationReceivedEventArgs> DeviceInformationReceived;

        private static IPAddress GetLocalIPAddress() {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()) {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                    var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                    if (addr != null && !addr.Address.ToString().Equals("0.0.0.0")) {
                        foreach (var ip in ni.GetIPProperties().UnicastAddresses) {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {
                                return ip.Address;
                            }
                        }
                    }
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}