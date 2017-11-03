namespace Thoemmi.Yeelight {
    /// <summary>
    ///     Possible reasons why a device send a message.
    /// </summary>
    public enum Reason {
        /// <summary>
        ///     The device sent its details because of a discovery broadcast.
        /// </summary>
        Discovery,

        /// <summary>
        ///     The dveice sent an advertisement message.
        /// </summary>
        Advertisement
    }
}