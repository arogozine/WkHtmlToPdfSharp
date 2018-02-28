using System;

namespace TuesPechkin
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class WkhtmltoxSettingAttribute : Attribute
    {
        public string Name { get; private set; }

        public WkhtmltoxSettingAttribute(string name)
        {
            Name = name;
        }
    }
}
