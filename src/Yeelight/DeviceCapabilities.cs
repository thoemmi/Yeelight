using System;

namespace Thoemmi.Yeelight {
    /// <summary>
    ///     Capabilities of devices.
    /// </summary>
    [Flags]
    public enum DeviceCapabilities {
        /// <summary>
        ///     The device supports no capabilites.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Can read current properties from device.
        /// </summary>
        GetProperties = 0x0001,

        /// <summary>
        /// Can change color temperature.
        /// </summary>
        SetColorTemperature = 0x0002,

        /// <summary>
        /// Can set RGB value.
        /// </summary>
        SetRGB = 0x0004,
        /// <summary>
        /// Can set HSV value.
        /// </summary>
        SetHSV = 0x0008,
        /// <summary>
        /// Can set brightness.
        /// </summary>
        SetBrightness = 0x0010,
        /// <summary>
        /// Can set power.
        /// </summary>
        SetPower = 0x0020,
        /// <summary>
        /// Can toggle.
        /// </summary>
        Toggle = 0x0040,
        /// <summary>
        /// Can toggle.
        /// </summary>
        SetDefault = 0x0080,
        /// <summary>
        /// Can start color flow.
        /// </summary>
        StartColorFlow = 0x0100,
        /// <summary>
        /// Can stop color flow.
        /// </summary>
        StopColorFlow = 0x0200,
        /// <summary>
        /// Can set scene.
        /// </summary>
        SetScene = 0x0400,
        /// <summary>
        /// Can start timer.
        /// </summary>
        StartTimer = 0x0800,
        /// <summary>
        /// Can get timer.
        /// </summary>
        GetTimer = 0x1000,
        /// <summary>
        /// Can delete timer.
        /// </summary>
        DeleteTimer = 0x2000,
        /// <summary>
        /// Can set adjust.
        /// </summary>
        SetAdjust = 0x4000,
        /// <summary>
        /// Can set music.
        /// </summary>
        SetMusic = 0x8000,
        /// <summary>
        /// Can set name.
        /// </summary>
        SetName = 0x10000,
        /// <summary>
        /// Can toggle main light and background light simultaneously.
        /// </summary>
        DevToggle = 0x20000,
    }
}