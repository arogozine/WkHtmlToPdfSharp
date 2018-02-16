using System;
using System.ComponentModel;

namespace TuesPechkin
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ObjectSettings : IObject
    {
        /// <summary>
        /// Should the sections from this document be included in the outline and table of content?
        /// </summary>
        [WkhtmltoxSetting("includeInOutline")]
        public bool? IncludeInOutline { get; set; }

        /// <summary>
        /// Should we count the pages of this document, in the counter used for TOC, headers and footers?
        /// </summary>
        [WkhtmltoxSetting("pagesCount")]
        public bool? CountPages { get; set; }

        /// <summary>
        /// The URL or path of the web page to convert, if "-" input is read from stdin.
        /// </summary>
        [WkhtmltoxSetting("page")]
        public string PageUrl { get; set; }

        /// <summary>
        /// Should we turn HTML forms into PDF forms?
        /// </summary>
        [WkhtmltoxSetting("produceForms")]
        public bool? ProduceForms { get; set; }

        /// <summary>
        /// Should external links in the HTML document be converted into external pdf links?
        /// </summary>
        [WkhtmltoxSetting("useExternalLinks")]
        public bool? ProduceExternalLinks { get; set; }

        /// <summary>
        /// Should internal links in the HTML document be converted into pdf references?
        /// </summary>
        [WkhtmltoxSetting("useLocalLinks")]
        public bool? ProduceLocalLinks { get; set; }

        public FooterSettings FooterSettings
        {
            get
            {
                return this.footer;
            }
            set
            {
                this.footer = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public HeaderSettings HeaderSettings
        {
            get
            {
                return this.header;
            }
            set
            {
                this.header = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public string HtmlText
        {
            get
            {
                return System.Text.Encoding.UTF8.GetString(this.data);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.data = System.Text.Encoding.UTF8.GetBytes(value);
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

        [Browsable(false)]
        public byte[] RawData
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value ?? throw new ArgumentNullException(nameof(value));
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

        public byte[] GetData()
        {
            return RawData;
        }

        public static implicit operator ObjectSettings(string html)
        {
            return new ObjectSettings { HtmlText = html };
        }

        private byte[] data = new byte[0];

        private FooterSettings footer = new FooterSettings();

        private HeaderSettings header = new HeaderSettings();

        private LoadSettings load = new LoadSettings();

        private WebSettings web = new WebSettings();
    }
}