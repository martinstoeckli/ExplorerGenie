// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                    var streamInfos = FileStreamSearcher.GetStreams(FullPath);
                    _streams.AddRange(streamInfos.Select(streamInfo => new AdsStreamViewModel(streamInfo, Language)));

                    if (_streams.Count > 0)
                        _streams.Add(_streams[0]);
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

        public AdsStreamViewModel(FileStreamInfo model, ILanguageService language)
        {
            _model = model;
            Language = language;
        }

        public string StreamDescription
        {
            get { return string.Format(
                "{0} [{1}]",
                _model.StreamName,
                Win32Api.StrFormatByteSize(_model.StreamSize)); }
        }
    }
}
