// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using ExplorerGenieShared.Models;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// ViewModel for a single <see cref="CustomGotoToolModel"/>.
    /// </summary>
    public class CustomGotoToolViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGotoToolViewModel"/> class.
        /// </summary>
        public CustomGotoToolViewModel()
            : this(new CustomGotoToolModel())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGotoToolViewModel"/> class.
        /// </summary>
        /// <param name="model">Sets the <see cref="Model"/>.</param>
        public CustomGotoToolViewModel(CustomGotoToolModel model)
        {
            Model = model;
        }

        /// <summary>
        /// Gets the wrapped model.
        /// </summary>
        public CustomGotoToolModel Model { get; private set; }

        /// <inheritdoc cref="CustomGotoToolModel.MenuTitle"/>
        public string MenuTitle
        {
            get { return Model.MenuTitle; }
            set { SetPropertyIndirect(() => Model.MenuTitle, (v) => Model.MenuTitle = v, value, false); }
        }

        /// <inheritdoc cref="CustomGotoToolModel.CommandLine"/>
        public string CommandLine
        {
            get { return Model.CommandLine; }
            set { SetPropertyIndirect(() => Model.CommandLine, (v) => Model.CommandLine = v, value, false); }
        }

        /// <inheritdoc cref="CustomGotoToolModel.AsAdmin"/>
        public bool AsAdmin
        {
            get { return Model.AsAdmin; }
            set { SetPropertyIndirect(() => Model.AsAdmin, (v) => Model.AsAdmin = v, value, false); }
        }
    }
}
