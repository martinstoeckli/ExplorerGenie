﻿// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using ExplorerGenieShared.Models;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ExplorerGenieShared.Services
{
    /// <summary>
    /// Implements the <see cref="ISettingsService"/> interface for the Windows platform.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly string _registryPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        public SettingsService()
            : this(@"SOFTWARE\MartinStoeckli\ExplorerGenie")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// Only used by unittests.
        /// </summary>
        /// <param name="registryPath">Overrides the registry path for unittests.</param>
        public SettingsService(string registryPath)
        {
            _registryPath = registryPath;
        }

        /// <inheritdoc/>
        public SettingsModel LoadSettingsOrDefault()
        {
            SettingsModel model = new SettingsModel();
            RegistryKey registry = Registry.CurrentUser.OpenSubKey(_registryPath, RegistryKeyPermissionCheck.ReadSubTree);
            if (registry == null)
                return model;

            model.FreshInstallation = registry.GetValueAsBool(nameof(model.FreshInstallation), model.FreshInstallation);
            model.CopyFileShowMenu = registry.GetValueAsBool(nameof(model.CopyFileShowMenu), model.CopyFileShowMenu);
            model.CopyFileFormat = registry.GetValueAsEnum(nameof(model.CopyFileFormat), model.CopyFileFormat);
            model.CopyFileOnlyFilename = registry.GetValueAsBool(nameof(model.CopyFileOnlyFilename), model.CopyFileOnlyFilename);
            model.CopyFileConvertToUnc = registry.GetValueAsBool(nameof(model.CopyFileConvertToUnc), model.CopyFileConvertToUnc);
            model.CopyEmailFormat = registry.GetValueAsEnum(nameof(model.CopyEmailFormat), model.CopyEmailFormat);
            model.CopyEmailConvertToUnc = registry.GetValueAsBool(nameof(model.CopyEmailConvertToUnc), model.CopyEmailConvertToUnc);
            model.GotoCommandPrompt = registry.GetValueAsBool(nameof(model.GotoCommandPrompt), model.GotoCommandPrompt);
            model.GotoPowerShell = registry.GetValueAsBool(nameof(model.GotoPowerShell), model.GotoPowerShell);
            model.GotoExplorer = registry.GetValueAsBool(nameof(model.GotoExplorer), model.GotoExplorer);
            model.HashShowMenu = registry.GetValueAsBool(nameof(model.HashShowMenu), model.HashShowMenu);
            model.NewFolderShowMenu = registry.GetValueAsBool(nameof(model.NewFolderShowMenu), model.NewFolderShowMenu);
            model.SymbolicLinkShowMenu = registry.GetValueAsBool(nameof(model.SymbolicLinkShowMenu), model.SymbolicLinkShowMenu);

            string jsonCustomTools = registry.GetValueAsString(nameof(model.CustomGotoTools), string.Empty);
            if (!string.IsNullOrEmpty(jsonCustomTools))
            {
                try
                {
                    CustomGotoToolModelList tools = JsonConvert.DeserializeObject<CustomGotoToolModelList>(jsonCustomTools);
                    model.CustomGotoTools = tools;
                }
                catch (Exception)
                {
                    // Don't endanger loading of other settings.
                }
            }
            return model;
        }

        /// <inheritdoc/>
        public bool TrySaveSettingsToLocalDevice(SettingsModel model)
        {
            try
            {
                RegistryKey registry = Registry.CurrentUser.CreateSubKey(_registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
                registry.SetValue(nameof(model.FreshInstallation), model.FreshInstallation);
                registry.SetValue(nameof(model.CopyFileShowMenu), model.CopyFileShowMenu);
                registry.SetValue(nameof(model.CopyFileFormat), model.CopyFileFormat);
                registry.SetValue(nameof(model.CopyFileOnlyFilename), model.CopyFileOnlyFilename);
                registry.SetValue(nameof(model.CopyFileConvertToUnc), model.CopyFileConvertToUnc);
                registry.SetValue(nameof(model.CopyEmailFormat), model.CopyEmailFormat);
                registry.SetValue(nameof(model.CopyEmailConvertToUnc), model.CopyEmailConvertToUnc);
                registry.SetValue(nameof(model.GotoCommandPrompt), model.GotoCommandPrompt);
                registry.SetValue(nameof(model.GotoPowerShell), model.GotoPowerShell);
                registry.SetValue(nameof(model.GotoExplorer), model.GotoExplorer);
                registry.SetValue(nameof(model.HashShowMenu), model.HashShowMenu);
                registry.SetValue(nameof(model.NewFolderShowMenu), model.NewFolderShowMenu);
                registry.SetValue(nameof(model.SymbolicLinkShowMenu), model.SymbolicLinkShowMenu);

                string jsonCustomTools = JsonConvert.SerializeObject(model.CustomGotoTools);
                registry.SetValue(nameof(model.CustomGotoTools), jsonCustomTools);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
