using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TuesPechkin
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LoadSettings : ISettings
    {
        public LoadSettings()
        {
            this.Cookies = new Dictionary<string, string>();
            this.CustomHeaders = new Dictionary<string, string>();
            this.PostItems = new List<PostItem>();
        }

        public enum ContentErrorHandling
        {
            /// <summary>
            /// Abort the convertion process
            /// </summary>
            Abort,

            /// <summary>
            /// Do not add the object to the final output
            /// </summary>
            Skip,

            /// <summary>
            /// Try to add the object to the final output.
            /// </summary>
            Ignore
        }

        /// <summary>
        /// Disallow local and piped files to access other local files.
        /// Must be either "true" or "false".
        /// </summary>
        [WkhtmltoxSetting("load.blockLocalFileAccess")]
        public bool? BlockLocalFileAccess { get; set; }

        [WkhtmltoxSetting("load.cookies")]
        public Dictionary<string, string> Cookies { get; private set; }

        [WkhtmltoxSetting("load.customHeaders")]
        public Dictionary<string, string> CustomHeaders { get; private set; }

        /// <summary>
        /// Forward javascript warnings and errors to the warning callback.
        /// </summary>
        [WkhtmltoxSetting("load.debugJavascript")]
        public bool? DebugJavascript { get; set; }

        /// <summary>
        /// How should we handle obejcts that fail to load.
        /// </summary>
        [WkhtmltoxSetting("load.loadErrorHandling")]
        public ContentErrorHandling? ErrorHandling { get; set; }

        /// <summary>
        /// The password to used when logging into a website, E.g. "elbarto"
        /// </summary>
        [WkhtmltoxSetting("load.password")]
        public string Password { get; set; }

        [WkhtmltoxSetting("load.post")]
        public IList<PostItem> PostItems { get; private set; }

        [WkhtmltoxSetting("load.proxy")]
        public string Proxy { get; set; }

        /// <summary>
        /// The mount of time in milliseconds to wait after a page has done loading until it is actually printed. E.g. "1200".
        /// We will wait this amount of time or until, javascript calls window.print().
        /// </summary>
        [WkhtmltoxSetting("load.jsdelay")]
        public int? RenderDelay { get; set; }

        /// <summary>
        /// Should the custom headers be sent all elements loaded instead of only the main page?
        /// </summary>
        [WkhtmltoxSetting("load.repeatCustomHeaders")]
        public bool? RepeatCustomHeaders { get; set; }

        /// <summary>
        /// Stop slow running javascript.
        /// </summary>
        [WkhtmltoxSetting("load.stopSlowScript")]
        public bool? StopSlowScript { get; set; }

        /// <summary>
        /// The user name to use when loging into a website, E.g. "bart"
        /// </summary>
        [WkhtmltoxSetting("load.username")]
        public string Username { get; set; }

        [WkhtmltoxSetting("load.windowStatus")]
        public string WindowStatus { get; set; }

        /// <summary>
        /// How much should we zoom in on the content?
        /// E.g. "2.2".
        /// </summary>
        [WkhtmltoxSetting("load.zoomFactor")]
        public double? ZoomFactor { get; set; }

        // TODO: load.runScript 
    }
}