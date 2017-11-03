using System;

namespace Thoemmi.Yeelight {
    /// <summary>
    ///     Provides additional information about the <see cref="DeviceListener.DeviceInformationReceived" /> event.
    /// </summary>
    public class DeviceInformationReceivedEventArgs : EventArgs {
        internal DeviceInformationReceivedEventArgs(Reason reason, DeviceInformation device) {
            Reason = reason;
            Device = device;
        }

        /// <summary>
        ///     Specifies the reason for the event.
        /// </summary>
        public Reason Reason { get; }

        /// <summary>
        ///     Provides information about a device.
        /// </summary>
        public DeviceInformation Device { get; }
    }
}