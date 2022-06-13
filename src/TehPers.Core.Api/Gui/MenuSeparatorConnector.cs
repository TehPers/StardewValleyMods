namespace TehPers.Core.Api.Gui
{
    /// <summary>
    /// A T-connector on the end of a menu separator.
    /// </summary>
    public enum MenuSeparatorConnector
    {
        /// <summary>
        /// Connect to a menu border with a pin.
        /// </summary>
        PinMenuBorder,

        /// <summary>
        /// Smoothly connect to a menu border.
        /// </summary>
        MenuBorder,

        /// <summary>
        /// Connect to another separator.
        /// </summary>
        Separator,

        /// <summary>
        /// No connector.
        /// </summary>
        None,
    }
}
