// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

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
    /// <param name="option">An option beginning with "-".</param>
    /// <param name="filenames">List of absolute paths to files.</param>
    /// <returns>Formatted parameter with files.</returns>
    class function BuildCommandLine(option: String; filenames: TStrings): String;

    /// <summary>
    /// Gets the first file/directory if the list contains only one item, otherwise it determines
    /// the common directory of the items.
    /// </summary>
    /// <param name="filenames">List of absolute paths to files.</param>
    /// <returns>Common directory.</returns>
    class function GetCommonDirectory(filenames: TStrings): String;

    /// <summary>
    /// Gets the operation verb for calling ShellExecute(), this is either 'open' or 'runas'
    /// depending on whether the file/folder should be opened as normal or admin user.
    /// </summary>
    /// <param name="asAdmin">SEt to true if it should run as admin.</param>
    /// <returns>Operation verb.</returns>
    class function GetShellExecuteOperation(asAdmin: Boolean): String;

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
    class function FindExplorerGenieOptionsPath: String;
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
    class procedure OnCopyOptionsClicked();

    /// <summary>
    /// Action for menu item "open cmd"
    /// </summary>
    /// <param name="filenames">List with paths of selected files.</param>
    class procedure OnGotoCmdClicked(filenames: TStrings; asAdmin: Boolean);

    /// <summary>
    /// Action for menu item "open powershell"
    /// </summary>
    /// <param name="filenames">List with paths of selected files.</param>
    class procedure OnGotoPowershellClicked(filenames: TStrings; asAdmin: Boolean);

    /// <summary>
    /// Action for menu item "open in explorer"
    /// </summary>
    /// <param name="filenames">List with paths of selected files.</param>
    class procedure OnGotoExplorerClicked(filenames: TStrings; asAdmin: Boolean);

    /// <summary>
    /// Action for menu item "Go to options"
    /// </summary>
    class procedure OnGotoOptionsClicked();
  end;

implementation

class procedure TActions.OnCopyFileClicked(filenames: TStrings);
var
  exePath: String;
  params: String;
begin
  exePath := FindExplorerGenieCmdPath();
  params := BuildCommandLine('-CopyFile', filenames);
  ExecuteCommand(exePath, params, false);
end;

class procedure TActions.OnCopyEmailClicked(filenames: TStrings);
var
  exePath: String;
  params: String;
begin
  exePath := FindExplorerGenieCmdPath();
  params := BuildCommandLine('-CopyEmail', filenames);
  ExecuteCommand(exePath, params, false);
end;

class procedure TActions.OnCopyOptionsClicked();
var
  exePath: String;
  params: String;
begin
  exePath := FindExplorerGenieOptionsPath();
  params := BuildCommandLine('-OpenedFromCopy', nil);
  ExecuteCommand(exePath, params, true);
end;

class procedure TActions.OnGotoCmdClicked(filenames: TStrings; asAdmin: Boolean);
var
  commonDirectory: String;
  operation: String;
  params: String;
begin
  commonDirectory := GetCommonDirectory(filenames);
  if (commonDirectory <> '') then
  begin
    operation := GetShellExecuteOperation(asAdmin);
    params := Format('/k "cd /d %s"', [commonDirectory]);
    ShellExecute(0, PChar(operation), PChar('cmd.exe'), PChar(params), PChar(params), SW_SHOWNORMAL);
  end;
end;

class procedure TActions.OnGotoPowershellClicked(filenames: TStrings; asAdmin: Boolean);
var
  commonDirectory: String;
  operation: String;
  params: String;
begin
  commonDirectory := GetCommonDirectory(filenames);
  if (commonDirectory <> '') then
  begin
    operation := GetShellExecuteOperation(asAdmin);
    params := Format('-noexit -command "& {Set-Location -Path ''%s''}"', [commonDirectory]);
    ShellExecute(0, PChar(operation), PChar('powershell.exe'), PChar(params), PChar(params), SW_SHOWNORMAL);
  end;
end;

class procedure TActions.OnGotoExplorerClicked(filenames: TStrings; asAdmin: Boolean);
var
  commonDirectory: String;
  operation: String;
  params: String;
begin
  if (filenames.Count = 0) then
    Exit;
  commonDirectory := filenames[0]; // Can be a file or a directory

  if (commonDirectory <> '') then
  begin
    operation := GetShellExecuteOperation(asAdmin);
    params := Format('/select,"%s"', [commonDirectory]);
    ShellExecute(0, PChar(operation), PChar('explorer.exe'), PChar(params), PChar(params), SW_SHOWNORMAL);
  end;
end;

class procedure TActions.OnGotoOptionsClicked;
var
  exePath: String;
  params: String;
begin
  exePath := FindExplorerGenieOptionsPath();
  params := BuildCommandLine('-OpenedFromGoto', nil);
  ExecuteCommand(exePath, params, true);
end;

class function TActions.FindExplorerGenieCmdPath: String;
var
  dllPath: String;
begin
  dllPath := GetModuleName(HInstance);
  Result := TPath.Combine(ExtractFilePath(dllPath), 'ExplorerGenieCmd.exe');
end;

class function TActions.FindExplorerGenieOptionsPath: String;
var
  dllPath: String;
begin
  dllPath := GetModuleName(HInstance);
  Result := TPath.Combine(ExtractFilePath(dllPath), 'ExplorerGenieOptions.exe');
end;

class function TActions.GetCommonDirectory(filenames: TStrings): String;
var
  firstFilePath: String;
begin
  if (filenames.Count = 0) then
    Exit;

  firstFilePath := filenames[0];
  if (filenames.Count = 1) and (TDirectory.Exists(firstFilePath)) then
  begin
    // User selected a directory
    Result := firstFilePath;
  end
  else
  begin
    // User selected one or more files/directories
    Result := ExtractFileDir(firstFilePath);
  end;
end;

class function TActions.BuildCommandLine(option: String; filenames: TStrings): String;
var
  sb: TSTringBuilder;
  rootDir: String;
  rootDirLen: Integer;
  filename: String;
  index: Integer;
begin
  Result := '';

  sb := TStringBuilder.Create();
  try
    option := StringReplace(option, '"', '', [rfReplaceAll]);
    sb.Append(option);

    if (filenames <> nil) and (filenames.Count > 0) then
    begin
      sb.Append(' ');

      // extract root directory from filenames (should be the same for every file)
      rootDir := ExtractFilePath(filenames[0]);
      rootDirLen := Length(rootDir);
      sb.Append('"' + rootDir + '"'); // file paths can never contain double quotes

      // remove root directory from all filenames, to save command line length
      for index := 0 to filenames.Count - 1 do
      begin
        filename := filenames[index];
        if StartsText(rootDir, filename) then
          Delete(filename, 1, rootDirLen);

        sb.Append(' "' + filename + '"'); // file paths can never contain double quotes
      end;
    end;
    Result := sb.ToString();
  finally
    sb.Free;
  end;
end;

class function TActions.GetShellExecuteOperation(asAdmin: Boolean): String;
begin
  if (asAdmin) then
    Result := 'runas'
  else
    Result := 'open';
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
