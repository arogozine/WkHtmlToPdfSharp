using System;
using System.ComponentModel;

namespace TuesPechkin
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ObjectSettings : IObject
    {
        [WkhtmltoxSetting("includeInOutline")]
        public bool? IncludeInOutline { get; set; }

        [WkhtmltoxSetting("pagesCount")]
        public bool? CountPages { get; set; }

        [WkhtmltoxSetting("page")]
        public string PageUrl { get; set; }

        [WkhtmltoxSetting("produceForms")]
        public bool? ProduceForms { get; set; }

        [WkhtmltoxSetting("useExternalLinks")]
        public bool? ProduceExternalLinks { get; set; }

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