// Copyright � 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitExplorerCommand;

interface
uses
  ActiveX,
  Classes,
  ComObj,
  Generics.Collections,
  Shlwapi,
  ShlObj,
  SysUtils,
  Windows,
  UnitLogger,
  UnitMenuModel;

type
  /// <summary>
  /// Wraps a TMenuModel and implements the IExplorerCommand interface required
  /// by the explorer.
  /// Access instances of this class only via interface to get reference counting.
  /// </summary>
  TExplorerCommand = class(TInterfacedObject, IExplorerCommand)
  private
    FModel: TMenuModel;
    FTitle: WideString;
    class function ReturnWideStringProperty(const value: WideString; out ppszValue: LPWSTR): HRESULT;
    class procedure ReadSelectedFilenames(const psiItemArray: IShellItemArray; filenames: TStringList);

    // IExplorerCommand
    function GetTitle(const psiItemArray: IShellItemArray; var ppszName: LPWSTR): HRESULT; stdcall;
    function GetIcon(const psiItemArray: IShellItemArray; var ppszIcon: LPWSTR): HRESULT; stdcall;
    function GetToolTip(const psiItemArray: IShellItemArray; var ppszInfotip: LPWSTR): HRESULT; stdcall;
    function GetCanonicalName(var pguidCommandName: TGUID): HRESULT; stdcall;
    function GetState(const psiItemArray: IShellItemArray; fOkToBeSlow: BOOL; var pCmdState: TExpCmdState): HRESULT; stdcall;
    function IExplorerCommand.Invoke = ExplorerCommandInvoke;
    function ExplorerCommandInvoke(const psiItemArray: IShellItemArray; const pbc: IBindCtx): HRESULT; stdcall;
    function GetFlags(var pFlags: TExpCmdFlags): HRESULT; stdcall;
    function EnumSubCommands(out ppEnum: IEnumExplorerCommand): HRESULT; stdcall;
  public
    constructor Create(model: TMenuModel);
    destructor Destroy(); override;

    property Model: TMenuModel read FModel;
  end;

implementation
uses
  UnitEnumExplorerCommand;

{ TExplorerCommandBase }

constructor TExplorerCommand.Create(model: TMenuModel);
begin
  inherited Create();
  FModel := model;
  FTitle := Model.Title;
  Logger.Debug('TExplorerCommand.Create'#9 + Model.Title);
end;

destructor TExplorerCommand.Destroy;
begin
  Logger.Debug('TExplorerCommand.Destroy'#9 + Model.Title);
  inherited;
end;

function TExplorerCommand.EnumSubCommands(out ppEnum: IEnumExplorerCommand): HRESULT;
begin
  Result := S_OK;
  ppEnum := TEnumExplorerCommand.Create(Model.Children);
end;

function TExplorerCommand.ExplorerCommandInvoke(const psiItemArray: IShellItemArray; const pbc: IBindCtx): HRESULT;
var
  filenames: TStringList;
begin
  Logger.Debug('TExplorerCommand.ExplorerCommandInvoke'#9 + Model.Title);
  Result := S_OK;
  if (not Assigned(psiItemArray) or (not Assigned(Model)) or (Model.IsSeparator) or (not Assigned(Model.OnClicked))) then
    Exit;

  filenames := TStringList.Create();
  try
    ReadSelectedFilenames(psiItemArray, filenames);
    Model.OnClicked(Model, filenames);
    Logger.Debug('TExplorerCommand.ExplorerCommandInvoke'#9 + filenames.Text);
  except
    Result := E_FAIL; // Don't let an exception escape to the explorer process
  end;
  filenames.Free();
end;

function TExplorerCommand.GetCanonicalName(var pguidCommandName: TGUID): HRESULT;
begin
  Logger.Debug('TExplorerCommand.GetCanonicalName'#9 + Model.Title);
  Result := E_NOTIMPL;
  pguidCommandName := TGuid.Empty;
end;

function TExplorerCommand.GetFlags(var pFlags: TExpCmdFlags): HRESULT;
begin
  Result := S_OK;
  if (Model.IsSeparator) then
    pFlags := ECF_ISSEPARATOR
  else if (Model.HasChildren) then
    pFlags := ECF_HASSUBCOMMANDS
  else
    pFlags := ECF_DEFAULT;
  Logger.Debug('TExplorerCommand.GetFlags'#9 + Model.Title + #9 + IntToStr(pFlags));
end;

function TExplorerCommand.GetIcon(const psiItemArray: IShellItemArray; var ppszIcon: LPWSTR): HRESULT;
begin
  Logger.Debug('TExplorerCommand.GetIcon'#9 + Model.Title);
  Result := ReturnWideStringProperty('', ppszIcon);
end;

function TExplorerCommand.GetState(const psiItemArray: IShellItemArray; fOkToBeSlow: BOOL; var pCmdState: TExpCmdState): HRESULT;
begin
  Logger.Debug('TExplorerCommand.GetState'#9 + Model.Title);
  Result := S_OK;
  pCmdState := ECS_ENABLED;
end;

function TExplorerCommand.GetTitle(const psiItemArray: IShellItemArray; var ppszName: LPWSTR): HRESULT;
begin
  Logger.Debug('TExplorerCommand.GetTitle'#9 + Model.Title);
  if (Model.IsSeparator) then
  begin
    // For separators, the return values must be null and a positive result like S_OK or S_FALSE.
    Result := S_OK;
    ppszName := nil;
  end
  else
  begin
    Result := ReturnWideStringProperty(FTitle, ppszName);
  end;
end;

function TExplorerCommand.GetToolTip(const psiItemArray: IShellItemArray; var ppszInfotip: LPWSTR): HRESULT;
begin
  Logger.Debug('TExplorerCommand.GetToolTip'#9 + Model.Title);
  Result := ReturnWideStringProperty('', ppszInfotip);
end;

class procedure TExplorerCommand.ReadSelectedFilenames(const psiItemArray: IShellItemArray; filenames: TStringList);
var
  selectionCount: Cardinal;
  selectionIndex: Cardinal;
  shellItem: IShellItem;
  filePath: PWideChar;
begin
  filenames.Clear();
  if Succeeded(psiItemArray.GetCount(selectionCount)) then
  begin
    for selectionIndex := 0 to selectionCount - 1 do
    begin
      if Succeeded(psiItemArray.GetItemAt(selectionIndex, shellItem)) and
         Succeeded(shellItem.GetDisplayName(SIGDN_FILESYSPATH, filePath)) then
      begin
        filenames.Add(filePath);
      end;
    end;
  end;
end;

class function TExplorerCommand.ReturnWideStringProperty(const value: WideString; out ppszValue: LPWSTR): HRESULT;
begin
  if (value = '') then
  begin
    Result := E_NOTIMPL;
    ppszValue := nil;
  end
  else
  begin
    Result := S_OK;
    ppszValue := PWideChar(value);
  end;
end;

end.
