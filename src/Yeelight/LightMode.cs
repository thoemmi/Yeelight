namespace Thoemmi.Yeelight {
    /// <summary>
    ///     The light mode of a device.
    /// </summary>
    public enum LightMode {
        /// <summary>
        ///     Color mode, i.e. <see cref="DeviceInformation.RGB" /> are valid.
        /// </summary>
        Color = 1,

        /// <summary>
        ///     Color temperatur mode, i.e. <see cref="DeviceInformation.ColorTemperature" /> are valid.
        /// </summary>
        ColorTemperature = 2,

        /// <summary>
        ///     HSV Mode, i.e. <see cref="DeviceInformation.Hue" /> and <see cref="DeviceInformation.Saturation" /> are valid.
        /// </summary>
        HSV = 3
    }
}