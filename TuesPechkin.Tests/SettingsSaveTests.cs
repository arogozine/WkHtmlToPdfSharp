using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TuesPechkin.Tests
{
    [TestClass]
    public class SettingsSaveTests : TuesPechkinTests
    {
        private IToolset GetToolset()
        {
            IToolset toolset = GetNewToolset();
            toolset.Load();
            return toolset;
        }

        [TestMethod]
        [TestCategory(nameof(SettingsSaveTests))]
        public void CropBottomDoesntExist()
        {
            const string cropBottom = "crop.bottom";
            IToolset toolset = GetToolset();

            IntPtr objectSettings = toolset.CreateObjectSettings();

            string setting = toolset.GetObjectSetting(objectSettings, cropBottom);
            Assert.AreEqual(setting, string.Empty);

            int setCropBottom = toolset.SetObjectSetting(objectSettings, cropBottom, setting);
            Assert.AreEqual(setCropBottom, 0);
        }

        [TestMethod]
        [TestCategory(nameof(SettingsSaveTests))]
        public void OutputFormatDoesNotExist()
        {
            const string outputFormat = "outputFormat";
            IToolset toolset = GetToolset();

            IntPtr globalSettings = toolset.CreateGlobalSettings();

            string settingValue = toolset.GetGlobalSetting(globalSettings, outputFormat);
            Assert.AreEqual(settingValue, string.Empty);

            int setResult = toolset.SetGlobalSetting(globalSettings, outputFormat, settingValue);
            Assert.AreEqual(setResult, 0);

            setResult = toolset.SetGlobalSetting(globalSettings, outputFormat, ".pdf");
            Assert.AreEqual(setResult, 0);
        }

        [TestMethod]
        [TestCategory(nameof(SettingsSaveTests))]
        public void GlobalSettingsSave()
        {
            IToolset toolset = GetToolset();

            IntPtr globalSettings = toolset.CreateGlobalSettings();
            var globalSettingOptions = GetSettings<GlobalSettings>();

            foreach (var (key, type) in globalSettingOptions)
            {
                switch (key)
                {
                    // Doesn't work until 0.12.5
                    case "outlineDepth":
                        continue;
                }

                string settingValue = toolset.GetGlobalSetting(globalSettings, key);
                int setResult = toolset.SetGlobalSetting(globalSettings, key, settingValue);
                Assert.AreEqual(setResult, 1);
            }
        }

        [TestMethod]
        [TestCategory(nameof(SettingsSaveTests))]
        public void ObjectSettingsSave()
        {
            IToolset toolset = GetToolset();

            IntPtr objectSettings = toolset.CreateObjectSettings();

            foreach (var (key, type) in ReflectObjectSettings())
            {
                string settingValue = toolset.GetObjectSetting(objectSettings, key);
                int setResult = toolset.SetObjectSetting(objectSettings, key, settingValue);
                Assert.AreEqual(setResult, 1);
            }
        }

        [TestMethod]
        [TestCategory(nameof(SettingsSaveTests))]
        public void GetReturnsSetValue()
        {
            IToolset toolset = GetToolset();
            IntPtr objectSettings = toolset.CreateObjectSettings();

            int setResult = toolset.SetObjectSetting(objectSettings, "header.fontName", "Courier New");
            Assert.AreEqual(setResult, 1);

            string fontName = toolset.GetObjectSetting(objectSettings, "header.fontName");
            Assert.AreEqual(fontName, "Courier New");

            IntPtr globalSettings = toolset.CreateGlobalSettings();
            setResult = toolset.SetGlobalSetting(globalSettings, "documentTitle", "Test 1234");
            Assert.AreEqual(setResult, 1);

            string documentTitle = toolset.GetGlobalSetting(globalSettings, "documentTitle");
            Assert.AreEqual(documentTitle, "Test 1234");
        }

        private static IEnumerable<(string, Type)> ReflectObjectSettings()
        {
            var settings =
                GetSettings<ObjectSettings>()
                .Union(GetSettings<FooterSettings>())
                .Union(GetSettings<HeaderSettings>())
                .Union(GetSettings<LoadSettings>());

            foreach (var value in settings)
            {
                yield return value;
            }
        }

        private static IEnumerable<(string, Type)> GetSettings<T>()
        {
            return from prop in typeof(ObjectSettings).GetProperties()
                   let attrs = prop.GetCustomAttributes(typeof(WkhtmltoxSettingAttribute), true)
                   where attrs.Length != 0
                   let name = ((WkhtmltoxSettingAttribute)attrs[0]).Name
                   select (name, prop.PropertyType);
        }
    }
}
