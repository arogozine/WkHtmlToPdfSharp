using System;
using System.Collections.Generic;
using System.Text;

namespace TuesPechkin
{
    public class HtmlToImageDocument : IDocument
    {
        [WkhtmltoxSetting("screenHeight")]
        public double ScreenHeight { get; set; } // TODO

        /// <summary>
        /// The with of the screen used to render is pixels, e.g "800"
        /// </summary>
        [WkhtmltoxSetting("screenWidth")]
        public double? ScreenWidth { get; set; }

        /// <summary>
        /// The compression factor to use when outputting a JPEG image
        /// </summary>
        [WkhtmltoxSetting("quality")]
        public double? Quality { get; set; }

        /// <summary>
        /// The output format to use, must be either "", "jpg", "png", "bmp" or "svg".
        /// </summary>
        [WkhtmltoxSetting("fmt")] // TODO
        public string Format { get; set; }

        /// <summary>
        /// The path of the output file, if "-" stdout is used,
        /// if empty the content is stored to a internalBuffer.
        /// </summary>
        [WkhtmltoxSetting("out")]
        public string Out { get; set; }

        /// <summary>
        /// The URL or path of the input file,
        /// if "-" stdin is used. E.g. "http://google.com"
        /// </summary>
        [WkhtmltoxSetting("in")]
        public string In { get; set; }

        /// <summary>
        /// When outputting a PNG or SVG, make the white background transparent.
        /// </summary>
        [WkhtmltoxSetting("transparent")]
        public bool? Transparent { get; set; }

        public CropSettings CropSettings
        {
            get
            {
                return this.crop;
            }
            set
            {
                this.crop = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public LoadSettings LoadSettings
        {
            get
            {
                return this.load;
            }
            set
            {
                this.load = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public WebSettings WebSettings
        {
            get
            {
                return this.web;
            }
            set
            {
                this.web = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public IEnumerable<IObject> GetObjects()
        {
            return new IObject[0];
        }

        private CropSettings crop = new CropSettings();

        private LoadSettings load = new LoadSettings();

        private WebSettings web = new WebSettings();
    }
}
