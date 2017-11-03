using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Yeelight {
    public static class SocketExtensions {
        public static Task<int> SendToAsync(this Socket socket, byte[] buffer, int offset, int count, SocketFlags flags, EndPoint endPoint) {
            return Task<int>.Factory.FromAsync(
                socket.BeginSendTo(buffer, offset, count, flags, endPoint, null, null),
                socket.EndSendTo
            );
        }
    }
}