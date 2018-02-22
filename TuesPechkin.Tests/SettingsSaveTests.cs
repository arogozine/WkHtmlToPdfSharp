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
            var globalSettingOptions = ReflectGlobalSettings();

            foreach (var setting in globalSettingOptions)
            {
                switch (setting.Key)
                {
                    // Doesn't work until 0.12.5
                    case "outlineDepth":
                        continue;
                }

                string settingValue = toolset.GetGlobalSetting(globalSettings, setting.Key);
                int setResult = toolset.SetGlobalSetting(globalSettings, setting.Key, settingValue);
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

        private static Dictionary<string, Type> ReflectGlobalSettings()
        {
            var globalSettings = from prop in typeof(GlobalSettings).GetProperties()
                                 let attrs = prop.GetCustomAttributes(typeof(WkhtmltoxSettingAttribute), true)
                                 where attrs.Length != 0
                                 select new
                                 {
                                     ((WkhtmltoxSettingAttribute)attrs[0]).Name,
                                     prop.PropertyType
                                 };
            var dict = new Dictionary<string, Type>();
            foreach (var setting in globalSettings)
            {
                dict.Add(setting.Name, setting.PropertyType);
            }
            return dict;
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
