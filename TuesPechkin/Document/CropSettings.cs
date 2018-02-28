namespace TuesPechkin
{
    public class CropSettings : ISettings
    {
        /// <summary>
        /// left/x coordinate of the window to capture in pixels
        /// </summary>
        [WkhtmltoxSetting("crop.left")]
        public double? Left { get; set; }

        /// <summary>
        /// top/y coordinate of the window to capture in pixels
        /// </summary>
        [WkhtmltoxSetting("crop.top")]
        public double? Top { get; set; }

        /// <summary>
        /// Width of the window to capture in pixels
        /// </summary>
        [WkhtmltoxSetting("crop.width")]
        public double? Width { get; set; }

        /// <summary>
        /// Height of the window to capture in pixels
        /// </summary>
        [WkhtmltoxSetting("crop.height")]
        public double? Height { get; set; }
    }
}
