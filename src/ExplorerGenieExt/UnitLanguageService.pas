// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitLanguageService;

interface
uses
  Classes,
  Generics.Collections,
  Generics.Defaults,
  IOUtils,
  SysUtils,
  Winapi.Windows;

type
  /// <summary>
  /// The language service can load localized text resources. It searches for resource files of the
  /// format:
  /// <example>
  /// {ResourceDirectoryPath}\Lng.{Domain}.{LanguageCode}
  /// D:\AppDirectory\Lng.AppName.en
  /// </example>
  /// If no file for the requested language can be found, the reader checks whether there exists a
  /// fallback resource file with the extension "en".
  /// </summary>
  ILanguageService = interface
    ['{DBE56DC0-5B97-4F21-8CC4-F0309B000DAA}']
    /// <summary>
    /// Loads a given localized text resource.
    /// </summary>
    /// <param name="id">Id of the text resource.</param>
    /// <param name="defaultText">A default text in case the resource cannot be found.</param>
    /// <returns>Localized text resource.</returns>
    function LoadText(id: String; const defaultText: String): string;
  end;

  TLanguageServiceFactory = class(TObject)
  public
    /// <summary>
    /// Creates a new language service instance.
    /// </summary>
    /// <returns>New language service.</returns>
    class function CreateLanguageService(): ILanguageService; overload;

    /// <summary>
    /// Creates a new language service instance.
    /// </summary>
    /// <param name="domain">The domain of the language resource. For this reader the domain is
    /// preset to the name of the calling DLL/EXE without the file extension..</param>
    /// <returns>New language service.</returns>
    class function CreateLanguageService(domain: String): ILanguageService; overload;
  end;

  /// <summary>
  /// Unittests can queryinterface the ILanguageService interface to this interface to get access
  /// to more methods of the service.
  /// </summary>
  ILanguageServiceForUnittest = interface
    ['{B88C0F97-DE9E-438F-80B4-4A787B19CDBE}']
    procedure ReadFromFile(textResources: TDictionary<string, string>; resourceFile: TStrings);
  end;

implementation
const
  FALLBACK_LANGUAGE_CODE = 'en';

type
  TLanguageService = class(TInterfacedObject, ILanguageService, ILanguageServiceForUnittest)
  private
    FTextResources: TDictionary<string, string>;
    FDomain: String;
    FLanguageCode: String;
    function LazyLoadTextResources(): Boolean;

    /// <summary>
    /// Checks whether a given line contains a comment starting with //, and therefore can be
    /// skipped.
    /// </summary>
    /// <param name="line">Line to test.</param>
    /// <returns>Returns true if the line contains a comment, otherwise false.</returns>
    function IsComment(line: String): Boolean;

    /// <summary>
    /// Splits a line from a file in two parts, id and text. The id is expected at the begin of
    /// the line, separated by a space. It's allowed to have none or multiple whitespace
    /// characters instead of the recommended single space.
    /// </summary>
    /// <param name="line">Line form resource file.</param>
    /// <param name="key">Retreives the found id.</param>
    /// <param name="text">Retreives the found text.</param>
    /// <returns>Returns true if line was valid, otherwise false.</returns>
    function TrySplitLine(line: String; out key: String; out text: String): Boolean;

    /// <summary>
    /// Replaces tags with a special meaning (e.g. "\n" or "\r\n" becomes crlf).
    /// </summary>
    /// <param name="resText">Text to search.</param>
    /// <returns>Text with replaces tags.</returns>
    function ReplaceSpecialTags(resText: String): String;
  public
    constructor Create(domain: String = ''; languageCode: string = '');
    destructor Destroy; override;

    // ILanguageService
    function LoadText(id: string; const defaultText: String): string;

    procedure ReadFromFile(textResources: TDictionary<string, string>; resourceFile: TStrings);

    /// <summary>
    /// Returns the two letter language code (ISO 639-1), used by the current process.
    /// </summary>
    /// <returns>Windows language code, two letters, lowercase.</returns>
    class function FindWindowsLanguageCode(): String;

    /// <summary>
    /// Builds a filename for specific resourcefile, but does not check whether that file
    /// really exists.
    /// </summary>
    /// <param name="directory">Search directory of resource file.</param>
    /// <param name="domain">Domain to which the resource belongs to.</param>
    /// <param name="languageCode">Two letter language code of the resource.</param>
    /// <returns>Expected file path of the resource file.</returns>
    class function BuildResourceFilePath(directory: string; domain: string; languageCode: string): string;
  end;

{ TLanguageServiceFactory }

class function TLanguageServiceFactory.CreateLanguageService: ILanguageService;
begin
  Result := TLanguageService.Create('', '');
end;

class function TLanguageServiceFactory.CreateLanguageService(domain: String): ILanguageService;
begin
  Result := TLanguageService.Create(domain, '');
end;

{ TLanguageService }

constructor TLanguageService.Create(domain: String = ''; languageCode: String = '');
begin
  if (domain <> '') then
    FDomain := domain
  else
    FDomain := TPath.GetFileNameWithoutExtension(GetModuleName(HInstance));

  if (languageCode <> '') then
    FLanguageCode := languageCode
  else
    FLanguageCode := FindWindowsLanguageCode();

  FTextResources := TDictionary<string, string>.Create(TIStringComparer.Ordinal);
end;

destructor TLanguageService.Destroy;
begin
  FTextResources.Free;
  inherited Destroy();
end;

function TLanguageService.LoadText(id: string; const defaultText: String): string;
begin
  if ((not LazyLoadTextResources()) or
      (not FTextResources.TryGetValue(id, Result))) then
  begin
    Result := defaultText;
  end;
end;

function TLanguageService.LazyLoadTextResources: Boolean;
var
  applicationDirectory: String;
  resourceFilePath: String;
  fallbackFilePath: String;
  resourceFile: TStringList;
begin
  if (FTextResources.Count = 0) then
  begin
    applicationDirectory := TPath.GetDirectoryName(GetModuleName(HInstance));

    resourceFilePath := BuildResourceFilePath(applicationDirectory, FDomain, FLanguageCode);
    if (not FileExists(resourceFilePath)) then
      fallbackFilePath := BuildResourceFilePath(applicationDirectory, FDomain, FALLBACK_LANGUAGE_CODE);

    if (FileExists(resourceFilePath)) then
    begin
      resourceFile := TStringList.Create();
      try
        resourceFile.LoadFromFile(resourceFilePath, TEncoding.UTF8);
        ReadFromFile(FTextResources, resourceFile);
      except
        FTextResources.Clear();
      end;
      resourceFile.Free;
    end;
  end;
  Result := FTextResources.Count > 0;
end;

procedure TLanguageService.ReadFromFile(textResources: TDictionary<string, string>; resourceFile: TStrings);
var
  line: String;
  key: String;
  text: String;
begin
  for line in resourceFile do
  begin
    if ((not IsComment(line)) and
        (TrySplitLine(line, key, text))) then
    begin
      text := ReplaceSpecialTags(text);
      textResources.AddOrSetValue(key, text);
    end;
  end;
end;

function TLanguageService.IsComment(line: String): Boolean;
begin
  Result := line.TrimLeft().StartsWith('//');
end;

function TLanguageService.TrySplitLine(line: String; out key: String; out text: String): Boolean;
var
  delimiterPos: Integer;
begin
  key := '';
  text := '';
  Result :=  not line.IsNullOrWhiteSpace(line);

  if (Result) then
  begin
    line := Trim(line);
    delimiterPos := line.IndexOf(' ');
    Result := (delimiterPos >= 2);

    if (Result) then
    begin
      key := line.Substring(0, delimiterPos).TrimRight();
      text := line.Substring(delimiterPos + 1).TrimLeft();
    end;
  end;
end;

function TLanguageService.ReplaceSpecialTags(resText: String): String;
begin
  Result := resText;
  Result := Result.Replace('\r\n', #13#10);
  Result := Result.Replace('\n', #13#10);
end;

class function TLanguageService.FindWindowsLanguageCode(): String;
var
  languageId: LANGID;
  charCount: Integer;
  buffer: String;
begin
  languageId := GetUserDefaultUILanguage();

  // Ask for buffer size (inclusive 0 character)
  charCount := GetLocaleInfo(languageId, LOCALE_SABBREVLANGNAME, nil, 0);
  SetLength(buffer, charCount);

  // Get language code
  GetLocaleInfo(languageId, LOCALE_SABBREVLANGNAME, PChar(buffer), charCount);
  SetLength(buffer, 2);
  Result := LowerCase(buffer);
end;

class function TLanguageService.BuildResourceFilePath(directory, domain, languageCode: string): string;
var
  resourceFileName: String;
begin
  resourceFileName := Format('Lng.%s.%s', [domain, languageCode]);
  Result := TPath.Combine(directory, resourceFileName);
end;

end.
