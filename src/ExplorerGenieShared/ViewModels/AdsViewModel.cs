// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public List<AdsStreamViewModel> Streams
        {
            get 
            { 
                if (_streams == null)
                {
                    _streams = new List<AdsStreamViewModel>();
                    var streamInfos = AlternativeDataStream.EnumerateStreams(FullPath);
                    _streams.AddRange(streamInfos.Select(streamInfo => new AdsStreamViewModel(streamInfo, Language)));
                }
                return _streams; 
            }
        }

        public string StreamCountBadge
        {
            get
            {
                string format = Streams.Count == 1 ? "{0} stream" : "{0} streams";
                return string.Format(format, Streams.Count);
            }
        }

        public bool HasStreams
        {
            get { return Streams.Count > 0; }
        }
    }

    /// <summary>
    /// View model for a single alternative data stream.
    /// </summary>
    public class AdsStreamViewModel : ViewModelBaseWithLanguage
    {
        private FileStreamInfo _model;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdsStreamViewModel"/> class.
        /// </summary>
        public AdsStreamViewModel(FileStreamInfo model, ILanguageService language)
        {
            _model = model;
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
                AlternativeDataStream.DeleteStream(_model.FullPath, _model.StreamName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Language["guiNtfsAdsDelete"], MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
