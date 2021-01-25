// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using ExplorerGenieShared.Models;
using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    internal class GotoToolPageViewModel : ViewModelBaseWithLanguage, IDisposable
    {
        private readonly SettingsModel _model;
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GotoToolPageViewModel"/> class.
        /// </summary>
        /// <param name="model">The shared model from <see cref="SettingsViewModel"/>.</param>
        /// <param name="language">The shared language service from <see cref="SettingsViewModel"/>.</param>
        /// <param name="settingsService">The shared settings service from <see cref="SettingsViewModel"/>.</param>
        public GotoToolPageViewModel(SettingsModel model, ILanguageService language, ISettingsService settingsService)
        {
            _model = model;
            Language = language;
            _settingsService = settingsService;

            // todo:
            _model.CustomGotoTools.Add(new CustomGotoToolModel { MenuTitle = "Sugus", CommandLine = "C:/" });
            _model.CustomGotoTools.Add(new CustomGotoToolModel { MenuTitle = "Caramel", CommandLine = "D:/" });

            // Create custom tools list
            CustomGotoTools = new ObservableCollection<CustomGotoToolViewModel>();
            foreach (CustomGotoToolModel toolModel in _model.CustomGotoTools)
            {
                var toolViewModel = new CustomGotoToolViewModel(toolModel);
                toolViewModel.PropertyChanged += CustomGotoToolChangedEventHandler;
                CustomGotoTools.Add(toolViewModel);
            }
            CustomGotoTools.CollectionChanged += CustomGotoToolsChangedEventHandler;

            // Create commands
            DeleteCustomGotoToolCommand = new RelayCommand<CustomGotoToolViewModel>(DeleteCustomGotoTool);
            MoveUpCommand = new RelayCommand((commandParam) => MoveCustomGotoTool(commandParam, true));
            MoveDownCommand = new RelayCommand((commandParam) => MoveCustomGotoTool(commandParam, false));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GotoToolPageViewModel"/> class.
        /// </summary>
        ~GotoToolPageViewModel()
        {
            Dispose();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GotoToolPageViewModel"/> class.
        /// </summary>
        public void Dispose()
        {
            CustomGotoTools.CollectionChanged -= CustomGotoToolsChangedEventHandler;
            foreach (var toolViewModel in CustomGotoTools)
                toolViewModel.PropertyChanged -= CustomGotoToolChangedEventHandler;
        }

        /// <inheritdoc cref="SettingsModel.GotoShowMenu"/>
        public bool GotoShowMenu
        {
            get { return _model.GotoShowMenu; }

            set
            {
                if (SetPropertyIndirect(() => _model.GotoShowMenu, (v) => _model.GotoShowMenu = v, value, false))
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
            }
        }

        /// <inheritdoc cref="SettingsModel.GotoCommandPrompt"/>
        public bool GotoCommandPrompt
        {
            get { return _model.GotoCommandPrompt; }

            set
            {
                if (SetPropertyIndirect(() => _model.GotoCommandPrompt, (v) => _model.GotoCommandPrompt = v, value, false))
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
            }
        }

        /// <inheritdoc cref="SettingsModel.GotoPowerShell"/>
        public bool GotoPowerShell
        {
            get { return _model.GotoPowerShell; }

            set
            {
                if (SetPropertyIndirect(() => _model.GotoPowerShell, (v) => _model.GotoPowerShell = v, value, false))
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
            }
        }

        /// <inheritdoc cref="SettingsModel.GotoExplorer"/>
        public bool GotoExplorer
        {
            get { return _model.GotoExplorer; }

            set
            {
                if (SetPropertyIndirect(() => _model.GotoExplorer, (v) => _model.GotoExplorer = v, value, false))
                    _settingsService?.TrySaveSettingsToLocalDevice(_model);
            }
        }

        /// <inheritdoc cref="SettingsModel.CustomGotoTools"/>
        public ObservableCollection<CustomGotoToolViewModel> CustomGotoTools { get; private set; }

        private void CustomGotoToolsChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    (e.NewItems[0] as CustomGotoToolViewModel).PropertyChanged += CustomGotoToolChangedEventHandler;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    (e.OldItems[0] as CustomGotoToolViewModel).PropertyChanged -= CustomGotoToolChangedEventHandler;
                    break;
            }
            UpdateCustomGotoToolModels();
        }

        private void CustomGotoToolChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            UpdateCustomGotoToolModels();
        }

        /// <summary>
        /// Should be called whenever the list of custom tools or one of its items changed.
        /// </summary>
        public void UpdateCustomGotoToolModels()
        {
        }

        /// <summary>
        /// Gets the command which deletes an item of the <see cref="CustomGotoTools"/>.
        /// </summary>
        public ICommand DeleteCustomGotoToolCommand { get; private set; }

        private void DeleteCustomGotoTool(CustomGotoToolViewModel customGotoTool)
        {
            CustomGotoTools.Remove(customGotoTool);
        }

        /// <summary>
        /// Gets the command which moves a custom tool upwards.
        /// </summary>
        public ICommand MoveUpCommand { get; private set; }

        /// <summary>
        /// Gets the command which moves a custom tool downwards.
        /// </summary>
        public ICommand MoveDownCommand { get; private set; }

        private void MoveCustomGotoTool(object commandParam, bool upwards)
        {
            if (TryGetModelFromSelectedCells(commandParam, out CustomGotoToolViewModel customGotoTool))
            {
                int oldIndex = CustomGotoTools.IndexOf(customGotoTool);
                int newIndex = upwards ? oldIndex - 1 : oldIndex + 1;
                bool newIndexInValidRange = (newIndex >= 0) && (newIndex < CustomGotoTools.Count);
                if (newIndexInValidRange)
                {
                    CustomGotoTools.Move(oldIndex, newIndex);
                }
            }
        }

        /// <summary>
        /// Gets the bound object of a DataGrid row. The binding can be done with a CommandParameter,
        /// by passing the SelectedCells of a DataGrid.
        /// <example>
        /// CommandParameter="{Binding ElementName=gridCustomGotoTools, Path=SelectedCells}"
        /// </example>
        /// </summary>
        /// <typeparam name="T">Type of the data bound to a row.</typeparam>
        /// <param name="commandParameter">The command parameter passed into the ICommand.</param>
        /// <param name="model">Receives the model, or null in case there is no selection.</param>
        /// <returns>Returns true if a model could be extracted, otherwise false.</returns>
        private bool TryGetModelFromSelectedCells<T>(object commandParameter, out T model)
        {
            if ((commandParameter is IList<DataGridCellInfo> cellInfos) &&
                (cellInfos.Count > 0) &&
                (cellInfos[0].Item is T result))
            {
                model = result;
                return true;
            }
            model = default(T);
            return false;
        }
    }
}
