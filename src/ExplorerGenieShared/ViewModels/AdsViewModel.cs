// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ExplorerGenieShared.Services;

namespace ExplorerGenieShared.ViewModels
{
    /// <summary>
    /// Collects information about the alternative datastreams of a single file/directory.
    /// </summary>
    public class AdsViewModel : ViewModelBaseWithLanguage
    {
        private List<AdsStreamViewModel> _streams;

        public AdsViewModel(string fullPath, ILanguageService language)
        {
            FullPath = fullPath;
            Language = language;
            IsDirectory = Directory.Exists(fullPath);
            FileOrDirectoryName = Path.GetFileName(fullPath);
        }

        public bool IsDirectory { get; }

        public string FullPath { get; }

        public string FileOrDirectoryName { get; }

        /// <summary>
        /// Gets a list of available streams for this file.
        /// </summary>
        public List<AdsStreamViewModel> Streams
        {
            get 
            {
                if (_streams == null)
                {
                    RefreshStreams();
                }
                return _streams; 
            }

            private set
            {
                if (SetProperty(ref _streams, value, false))
                {
                    this.OnPropertyChanged(nameof(HasStreams));
                    this.OnPropertyChanged(nameof(StreamCountBadge));
                }
            }
        }

        /// <summary>
        /// Gets a text like "1 stream" which shows how many streams are available.
        /// </summary>
        public string StreamCountBadge
        {
            get
            {
                string format = Streams.Count == 1 ? "{0} stream" : "{0} streams";
                return string.Format(format, Streams.Count);
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are streams in this file or not.
        /// </summary>
        public bool HasStreams
        {
            get { return Streams.Count > 0; }
        }

        public void RefreshStreams()
        {
            var streamInfos = AlternativeDataStream.EnumerateStreams(FullPath);
            var streamViewModels = streamInfos.Select(streamInfo => new AdsStreamViewModel(streamInfo, RefreshStreams, Language)).ToList();
            Streams = streamViewModels.ToList();
        }
    }

    /// <summary>
    /// View model for a single alternative data stream.
    /// </summary>
    public class AdsStreamViewModel : ViewModelBaseWithLanguage
    {
        private FileStreamInfo _model;
        private Action _refreshStreams;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdsStreamViewModel"/> class.
        /// </summary>
        public AdsStreamViewModel(FileStreamInfo model, Action refreshStreams, ILanguageService language)
        {
            _model = model;
            _refreshStreams = refreshStreams;
            Language = language;
            ExportStreamCommand = new RelayCommand<string>(ExportStream);
            DeleteStreamCommand = new RelayCommand<string>(DeleteStream);
        }

        public string StreamDescription
        {
            get { return string.Format(
                "{0} [{1}]",
                _model.StreamName,
                Win32Api.StrFormatByteSize(_model.StreamSize)); }
        }

        /// <summary>
        /// Gets the command which exports a stream to a file.
        /// </summary>
        public ICommand ExportStreamCommand { get; }

        private async void ExportStream(object param)
        {
            const string MainDataStreamName = ":$DATA";

            string streamPart = _model.StreamName;
            if (streamPart.EndsWith(MainDataStreamName, StringComparison.OrdinalIgnoreCase))
                streamPart = streamPart.Remove(streamPart.Length - MainDataStreamName.Length);
            streamPart = streamPart.Replace(":", "➽");

            string targetFileName = _model.FullPath + streamPart + ".txt";
            targetFileName = PathUtils.GetUniqueFilename(targetFileName);
            try
            {
                await AlternativeDataStream.SaveStreamAs(_model.FullPath, _model.StreamName, targetFileName);
                MessageBox.Show(
                    Language.LoadTextFmt("guiNtfsAdsExportSuccess", Path.GetFileName(targetFileName)), 
                    Language["guiNtfsAdsExport"], 
                    MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Language["guiNtfsAdsExport"], MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// Gets the command which exports a stream to a file.
        /// </summary>
        public ICommand DeleteStreamCommand { get; }

        private void DeleteStream(object param)
        {
            try
            {
                if (MessageBox.Show(
                    Language.LoadTextFmt("guiNtfsAdsDeleteConfirm", _model.StreamName, Path.GetFileName(_model.FullPath)),
                    Language["guiNtfsAdsDelete"],
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    AlternativeDataStream.DeleteStream(_model.FullPath, _model.StreamName);
                    _refreshStreams();
                    MessageBox.Show(Language["guiNtfsAdsDeleteSuccess"], Language["guiNtfsAdsDelete"], MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Language["guiNtfsAdsDelete"], MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
