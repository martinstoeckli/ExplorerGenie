// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using ExplorerGenieShared.Models;
using Microsoft.Win32;

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

            model.CopyFileShowMenu = registry.GetValueAsBool("CopyFileShowMenu", model.CopyFileShowMenu);
            model.CopyFileFormat = registry.GetValueAsEnum("CopyFileFormat", model.CopyFileFormat);
            model.CopyFileOnlyFilename = registry.GetValueAsBool("CopyFileOnlyFilename", model.CopyFileOnlyFilename);
            model.CopyFileConvertToUnc = registry.GetValueAsBool("CopyFileConvertToUnc", model.CopyFileConvertToUnc);
            model.OpenInShowMenu = registry.GetValueAsBool("CopyEmailShowMenu", model.CopyFileShowMenu);
            model.CopyEmailFormat = registry.GetValueAsEnum("CopyEmailFormat", model.CopyEmailFormat);
            model.CopyEmailConvertToUnc = registry.GetValueAsBool("CopyEmailConvertToUnc", model.CopyEmailConvertToUnc);
            return model;
        }

        /// <inheritdoc/>
        public bool TrySaveSettingsToLocalDevice(SettingsModel model)
        {
            try
            {
                RegistryKey registry = Registry.CurrentUser.CreateSubKey(_registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
                registry.SetValue("CopyFileShowMenu", model.CopyFileShowMenu);
                registry.SetValue("CopyFileFormat", model.CopyFileFormat);
                registry.SetValue("CopyFileOnlyFilename", model.CopyFileOnlyFilename);
                registry.SetValue("CopyFileConvertToUnc", model.CopyFileConvertToUnc);
                registry.SetValue("CopyEmailShowMenu", model.OpenInShowMenu);
                registry.SetValue("CopyEmailFormat", model.CopyEmailFormat);
                registry.SetValue("CopyEmailConvertToUnc", model.CopyEmailConvertToUnc);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
