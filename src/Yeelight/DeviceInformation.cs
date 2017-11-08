using System;

namespace Thoemmi.Yeelight {
    /// <summary>
    ///     Provides information about a device.
    /// </summary>
    public class DeviceInformation {
        /// <summary>
        ///     The interval wghen the device will send another advertisment message.
        /// </summary>
        public TimeSpan RefreshInterval { get; set; }

        /// <summary>
        ///     Specifies the service access point. The URI scheme will always be "Yeelight".
        /// </summary>
        public Uri EndPoint { get; set; }

        /// <summary>
        ///     The ID of the device.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The product model of the device.
        /// </summary>
        /// <remarks>
        ///     Current it can be "mono", "color", "stripe", "ceiling", "bslamp". For
        ///     "mono", it represents device that only supports brightness adjustment.
        ///     For "color", it represents device that support both color and color
        ///     temperature adjustment. "Stripe" stands for Yeelight smart LED stripe.
        ///     "Ceiling" stands for Yeelight Ceiling Light.
        ///     More values may be added in future.
        /// </remarks>
        public string Model { get; set; }

        /// <summary>
        ///     The device's firmware version.
        /// </summary>
        public string FirmwareVersion { get; set; }

        /// <summary>
        ///     The list of supported control methods.
        /// </summary>
        /// <remarks>
        ///     Users of the library can use this field to dynamically render the control
        ///     view to user if necessary. Any control request that invokes method that
        ///     is not included in this field will be rejected by the device.
        /// </remarks>
        public DeviceCapabilities Capabilities { get; set; }

        /// <summary>
        ///     Current power status.
        /// </summary>
        public bool PoweredOn { get; set; }

        /// <summary>
        ///     Current brightness, it's the percentage of maximum brightness. The range
        ///     of this value is 1 ~ 100.
        /// </summary>
        public int Brightness { get; set; }

        /// <summary>
        ///     The current light mode.
        /// </summary>
        public LightMode LightMode { get; set; }

        /// <summary>
        ///     The current color temperature value. The range of this value depends on product model.
        /// </summary>
        /// <remarks>
        ///     This field is valid only if <see cref="LightMode" /> is
        ///     <see cref="Yeelight.LightMode.ColorTemperature" />.
        /// </remarks>
        public int ColorTemperature { get; set; }

        /// <summary>
        ///     The current RGB value.
        /// </summary>
        /// <remarks>
        ///     This field is valid only if <see cref="LightMode" /> is
        ///     <see cref="Yeelight.LightMode.Color" />.
        /// </remarks>
        public int RGB { get; set; }

        /// <summary>
        ///     The current hue value. The range of this value is 0 to 359.
        /// </summary>
        /// <remarks>
        ///     This field is valid only if <see cref="LightMode" /> is
        ///     <see cref="Yeelight.LightMode.HSV" />.
        /// </remarks>
        public int Hue { get; set; }

        /// <summary>
        ///     The current saturation value. The range of this value is 0 to 100.
        /// </summary>
        /// <remarks>
        ///     This field is valid only if <see cref="LightMode" /> is
        ///     <see cref="Yeelight.LightMode.HSV" />.
        /// </remarks>
        public int Saturation { get; set; }

        /// <summary>
        ///     The name of the device.
        /// </summary>
        public string Name { get; set; }
    }
}