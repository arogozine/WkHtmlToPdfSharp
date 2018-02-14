using System;
using System.Collections.Generic;
using System.Text;

namespace TuesPechkin
{
    public class CropSettings : ISettings
    {
        /// <summary>
        /// top/y coordinate of the window to capture in pixels
        /// </summary>
        [WkhtmltoxSetting("crop.top")]
        public double? Top { get; set; }

        [WkhtmltoxSetting("crop.bottom")]
        public double? Bottom { get; set; } // ?

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

        // TODO:
        // left/x coordinate of the window to capture in pixels
        // crop.left
    }
}
