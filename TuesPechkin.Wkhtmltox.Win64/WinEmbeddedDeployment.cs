using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using TuesPechkin.Properties;
using SysPath = System.IO.Path;

namespace TuesPechkin
{
    [Serializable]
    public class Win64EmbeddedDeployment : EmbeddedDeployment
    {
        public Win64EmbeddedDeployment(IDeployment physical) : base(physical) { }

        public override string Path
        {
            get
            {
                return SysPath.Combine(
                    base.Path,
                    GetType().Assembly.GetName().Version.ToString());
            }
        }

        protected override (string, Stream) GetContents()
        {
            var key = WkhtmltoxBindings.DLLNAME;
            var value = new GZipStream(
                        new MemoryStream(Resources.wkhtmltox_64_dll),
                        CompressionMode.Decompress);

            return (key, value);
        }
    }
}
