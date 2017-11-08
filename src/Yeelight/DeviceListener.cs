using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Thoemmi.Yeelight {
    /// <summary>
    ///     Provides the functionality to discover devices.
    /// </summary>
    public class DeviceListener {
        private const int Port = 1982;

        private const string SsdpMessage = "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1982\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb";
        private static readonly byte[] _dgram = Encoding.ASCII.GetBytes(SsdpMessage);

        private static readonly IPAddress _multicastaddress = IPAddress.Parse("239.255.255.250");
        private static readonly IPEndPoint _multicastEndPoint = new IPEndPoint(_multicastaddress, Port);

        /// <summary>
        ///     Starts the listener for device advertisements and starts discovery of devices asynchronously.
        /// </summary>
        /// <returns>An asynchronous task that completes when the device discovery has started.</returns>
        public void StartListening() {
            var client = new UdpClient();

            client.ExclusiveAddressUse = false;

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            var localEndPoint = new IPEndPoint(GetLocalIPAddress(), 0);
            client.Client.Bind(localEndPoint);

            client.JoinMulticastGroup(_multicastaddress);

            Console.WriteLine("Listening this will never quit so you will need to ctrl-c it");

            Task.Factory.StartNew(() => {
                while (true) {
                    byte[] data = client.Receive(ref localEndPoint);
                    var message = Encoding.ASCII.GetString(data);
                    if (message == SsdpMessage) {
                        // don't handle search requests (may even be ourselves)
                        continue;
                    }

                    var (reason, device) = MessageParser.Parse(message);
                    DeviceInformationReceived?.Invoke(this, new DeviceInformationReceivedEventArgs(reason, device));
                }
            });

            client.Send(_dgram, _dgram.Length, _multicastEndPoint);
        }

        /// <summary>
        ///     This event is raised when either a device was discovered or a device send an advertisement.
        /// </summary>
        public event EventHandler<DeviceInformationReceivedEventArgs> DeviceInformationReceived;

        private static IPAddress GetLocalIPAddress() {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()) {
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr != null && !addr.Address.ToString().Equals("0.0.0.0")) {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
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