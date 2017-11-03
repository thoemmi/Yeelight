using System;

namespace Thoemmi.Yeelight {
    public class DeviceInformationReceivedEventArgs : EventArgs {
        public DeviceInformationReceivedEventArgs(Reason reason, DeviceInformation device) {
            Reason = reason;
            Device = device;
        }

        public Reason Reason { get; }

        public DeviceInformation Device { get; }
    }
}