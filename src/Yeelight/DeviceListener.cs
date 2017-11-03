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

                var (reason, device) = MessageParser.Parse(message);

                DeviceInformationReceived?.Invoke(this, new DeviceInformationReceivedEventArgs(reason, device));
            }
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