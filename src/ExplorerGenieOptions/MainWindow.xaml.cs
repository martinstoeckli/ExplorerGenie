// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Windows;
using System.Windows.Controls;
using ExplorerGenieShared;
using ExplorerGenieShared.ViewModels;

namespace ExplorerGenieOptions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CommandLineArgs arguments = CommandLineInterpreter.ParseCommandLine(Environment.CommandLine);

            TabControl tabControl = this.FindName("tab") as TabControl;
            if ("-OpenedFromCopy".Equals(arguments.Action))
                tabControl.SelectedIndex = 0;
            else if ("-OpenedFromGoto".Equals(arguments.Action))
                tabControl.SelectedIndex = 1;
            else if ("-OpenedFromHash".Equals(arguments.Action))
                tabControl.SelectedIndex = 2;
            else
                tabControl.SelectedIndex = 0;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Unfortunately the DataGrid header seems not bindable with sane complexity.
            DataGrid hashDataGrid = FindName("hashDataGrid") as DataGrid;
            hashDataGrid.Columns[0].Header = GetViewModel().Language["guiAlgorithm"];
            hashDataGrid.Columns[1].Header = GetViewModel().Language["guiHashValue"];
            gridCustomGotoTools.Columns[0].Header = GetViewModel().Language["guiGotoMenuTitle"];
            gridCustomGotoTools.Columns[1].Header = GetViewModel().Language["guiGotoMenuCommand"];
            gridCustomGotoTools.Columns[2].Header = GetViewModel().Language["guiGotoMenuAdmin"];
        }

        private SettingsViewModel GetViewModel()
        {
            return (SettingsViewModel)DataContext;
        }

        private void SystemFolderDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid.SelectedItem is SystemFolderViewModel selectedViewModel)
            {
                GetViewModel().SystemFoldersPageViewModel.GotoSystemFolderCommand.Execute(selectedViewModel);
            }
        }

        private void HashDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid.SelectedItem is HashResultViewModel selectedViewModel)
            {
                GetViewModel().CalculateHashPageViewModel.CopyHashToClipboardCommand.Execute(selectedViewModel);
            }
        }
    }
}
