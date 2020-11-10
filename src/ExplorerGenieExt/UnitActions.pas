unit UnitActions;

interface
uses
  Windows,
  Classes,
  SysUtils,
  StrUtils,
  ShellApi,
  IOUtils;

type
  TActions = class(TObject)
  private
    /// <summary>
    /// Formats the list of filenames, so it can be used as commandline parameters. The common
    /// root directory is stripped away and passed as the first parameter, to reduce the length
    /// of the command line.
    /// </summary>
    /// <param name="filenames">List of absolute paths to files.</param>
    /// <returns>Formatted parameter with files.</returns>
    class function GetFilenamesParamString(filenames: TStrings): String;

    /// <summary>
    /// Starts a new application/process with the given parameters.
    /// Internally it uses CreateProcess() which accepts much longer parameter strings (up to 32767
    /// chars) than we could pass to ShellExecute() or to cmd.exe.
    /// </summary>
    /// <param name="cmd">Path to the executable.</param>
    /// <param name="params">Command line parameters.</param>
    /// <param name="visible">A value indicating whether the executable should be shown (Window),
    /// or if it is started invisible.</param>
    class procedure ExecuteCommand(const cmd, params: String; visible: Boolean);

    class function FindExplorerGenieCmdPath: String;
    class function FindExplorerGenieOptPath: String;
  public
    /// <summary>
    /// Action for menu item "copy file"
    /// </summary>
    /// <param name="filenames">List with paths of selected files.</param>
    class procedure OnCopyFileClicked(filenames: TStrings);

    /// <summary>
    /// Action for menu item "copy as email"
    /// </summary>
    /// <param name="filenames">List with paths of selected files.</param>
    class procedure OnCopyEmailClicked(filenames: TStrings);

    /// <summary>
    /// Action for menu item "copy options"
    /// </summary>
    /// <param name="filenames">List with paths of selected files.</param>
    class procedure OnCopyOptionsClicked();
  end;

implementation

class procedure TActions.OnCopyFileClicked(filenames: TStrings);
var
  exePath: String;
  params: String;
begin
  exePath := FindExplorerGenieCmdPath();
  params := '-CopyFile ' + GetFilenamesParamString(filenames);
  ExecuteCommand(exePath, params, false);
end;

class procedure TActions.OnCopyEmailClicked(filenames: TStrings);
var
  exePath: String;
  params: String;
begin
  exePath := FindExplorerGenieCmdPath();
  params := '-CopyEmail ' + GetFilenamesParamString(filenames);
  ExecuteCommand(exePath, params, false);
end;

class procedure TActions.OnCopyOptionsClicked();
var
  exePath: String;
begin
  exePath := FindExplorerGenieOptPath();
  ExecuteCommand(exePath, '-OpenedFromCopy', true);
end;

class function TActions.FindExplorerGenieCmdPath: String;
var
  dllPath: String;
begin
  dllPath := GetModuleName(HInstance);
  Result := TPath.Combine(ExtractFilePath(dllPath), 'ExplorerGenieCmd.exe');
end;

class function TActions.FindExplorerGenieOptPath: String;
var
  dllPath: String;
begin
  dllPath := GetModuleName(HInstance);
  Result := TPath.Combine(ExtractFilePath(dllPath), 'ExplorerGenieOpt.exe');
end;

class function TActions.GetFilenamesParamString(filenames: TStrings): String;
var
  params: TStringList;
  rootDir: String;
  rootDirLen: Integer;
  filename: String;
  index: Integer;
begin
  Result := '';
  if (filenames.Count = 0) then Exit;

  params := TStringList.Create;
  try
    // extract root directory from filenames (should be the same for every file)
    rootDir := ExtractFilePath(filenames[0]);
    rootDirLen := Length(rootDir);
    params.Add(rootDir);

    // remove root directory from all filenames, to save command line length
    for index := 0 to filenames.Count - 1 do
    begin
      filename := filenames[index];
      if StartsText(rootDir, filename) then
        Delete(filename, 1, rootDirLen);
      params.Add(filename);
    end;

    Result := params.CommaText;
  finally
    params.Free;
  end;
end;

class procedure TActions.ExecuteCommand(const cmd, params: String; visible: Boolean);
var
  applicationName: WideString;
  commandLine: WideString;
  startupInfo: TStartupInfo;
  processInfo: TProcessInformation;
  success: BOOL;
begin
  // prepare startupinfo structure
  ZeroMemory(@startupInfo, SizeOf(TStartupInfo));
  startupInfo.cb := Sizeof(TStartupInfo);

  if (not visible) then
  begin
    startupInfo.dwFlags := STARTF_USESHOWWINDOW;
    startupInfo.wShowWindow := SW_HIDE;
  end;

  // start the process
  applicationName := cmd; // According to the docs, it must be passed as a variable which can be modified.
  commandLine := params;
  success := CreateProcess(PWideChar(applicationName), PWideChar(commandLine), nil, nil, True, NORMAL_PRIORITY_CLASS, nil, nil, startupInfo, processInfo);
  if success then
  begin
    CloseHandle(processInfo.hThread);
    CloseHandle(processInfo.hProcess);
  end;
end;

end.
