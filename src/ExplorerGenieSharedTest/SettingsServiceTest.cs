using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace ExplorerGenieSharedTest
{
    [TestClass]
    public class SettingsServiceTest
    {
        private const string RegistryPath = @"SOFTWARE\MartinStoeckli\UnitTest";

        [TestInitialize]
        public void Initialize()
        {
            Registry.CurrentUser.DeleteSubKey(RegistryPath, false);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Registry.CurrentUser.DeleteSubKey(RegistryPath, false);
        }

        [TestMethod]
        public void LoadSettingsOrDefault_LoadsDefaultWhenEmpty()
        {
            ISettingsService service = new SettingsService(RegistryPath);
            SettingsModel settings = service.LoadSettingsOrDefault();

            SettingsModel defaultSettings = new SettingsModel();
            Assert.AreEqual(defaultSettings, settings);
        }

        [TestMethod]
        public void LoadSettingsOrDefault_LoadsWhenMissingValuesExist()
        {
            RegistryKey registry = Registry.CurrentUser.CreateSubKey(RegistryPath);
            registry.SetValue("unused", 0);

            ISettingsService service = new SettingsService(RegistryPath);
            SettingsModel settings = service.LoadSettingsOrDefault();

            SettingsModel defaultSettings = new SettingsModel();
            Assert.AreEqual(defaultSettings, settings);
        }

        [TestMethod]
        public void TrySaveSettingsToLocalDevice_WritesAndReadsTheSame()
        {
            SettingsModel settings = new SettingsModel
            {
                CopyFileShowMenu = true,
                CopyFileFormat = CopyFileFormat.Uri,
                CopyFileOnlyFilename = true,
                CopyFileConvertToUnc = true,
                CopyEmailFormat = CopyEmailFormat.Thunderbird,
                CopyEmailConvertToUnc = false,
            };
            ISettingsService service = new SettingsService(RegistryPath);
            Assert.IsTrue(service.TrySaveSettingsToLocalDevice(settings));
            SettingsModel loadedSettings = service.LoadSettingsOrDefault();

            Assert.AreEqual(settings, loadedSettings);
        }
    }
}
