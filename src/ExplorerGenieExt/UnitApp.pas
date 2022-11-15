// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitApp;

{$WARN SYMBOL_PLATFORM OFF}

interface

uses
  Windows,
  Classes,
  SysUtils,
  ComObj,
  ActiveX,
  ShlObj,
  ShellApi,
  ComServ,
  ExplorerGenieExt_TLB,
  UnitExplorerCommand,
  UnitImports,
  UnitMenuModel,
  UnitMenuModelIcon,
  UnitActions,
  UnitLanguageService,
  UnitSettingsModel,
  UnitSettingsService,
  UnitSettingsGotoToolModel;

type
  /// <summary>
  /// Main class of the shell extension, it implements the required interfaces.
  /// </summary>
  TApp = class(TAutoObject, IApp, IExplorerCommand, IShellExtInit)
  private
    FMenus: TMenuModel;
    FExplorerCommand: IExplorerCommand;
    FFilenames: TStringList;
    function CreateMenuModels(settingsService: TSettingsService; languageService: ILanguageService): TMenuModel;

    // UnitImports.IExplorerCommand
    function GetTitle(psiItemArray: IShellItemArray; out ppszName: LPWSTR): HResult; stdcall;
    function GetIcon(psiItemArray: IShellItemArray; out ppszIcon: LPWSTR): HResult; stdcall;
    function GetToolTip(psiItemArray: IShellItemArray; out ppszInfotip: LPWSTR): HResult; stdcall;
    function GetCanonicalName(out pguidCommandName: TGUID): HResult; stdcall;
    function GetState(psiItemArray: IShellItemArray; fOkToBeSlow: boolean; out pCmdState: TEXPCMDSTATE): HResult; stdcall;
    function IExplorerCommand.Invoke = ExplorerCommandInvoke;
    function ExplorerCommandInvoke(psiItemArray: IShellItemArray; pbc: IBindCtx): HResult; stdcall;
    function GetFlags(out pFlags: TEXPCMDFLAGS): HResult; stdcall;
    function EnumSubCommands(out ppEnum: IEnumExplorerCommand): HResult; stdcall;

    // IShellExtInit
    function IShellExtInit.Initialize = SEIInitialize;
    function SEIInitialize(
      pidlFolder: PItemIDList; lpdobj: IDataObject; hKeyProgID: HKEY): HRESULT; stdcall;
  public
    /// <summary>
    /// Initializes a new instance of a TApp object.
    /// For COM objects this acts as a constructor.
    /// </summary>
    procedure Initialize; override;

    /// <summary>
    /// Finalizes an instance of the TApp class.
    /// </summary>
    destructor Destroy; override;
  end;

implementation
const
  APP_DESCRIPTION = 'ExplorerGenie adds tools to the explorers context menu';

procedure TApp.Initialize;
var
  languageService: ILanguageService;
  settingsService: TSettingsService;
begin
  inherited Initialize;
  FFilenames := TStringList.Create;

  languageService := TLanguageServiceFactory.CreateLanguageService('ExplorerGenie');
{$IFDEF DEBUG}
  // development: Here we can force loading of a specific language.
  languageService := TLanguageServiceFactory.CreateLanguageService('ExplorerGenie', 'en');
{$ENDIF}

  settingsService := TSettingsService.Create(languageService);
  try
    FMenus := CreateMenuModels(settingsService, languageService);
    FExplorerCommand := TExplorerCommand.Create(FMenus) as IExplorerCommand;
  finally
    settingsService.Free;
  end;
end;

destructor TApp.Destroy;
begin
  try
    FExplorerCommand := nil;
    FMenus.Free;
    FFilenames.Free;
  except
    on e: Exception do
      MessageBox(0, PChar(e.Message), '', MB_ICONERROR);
  end;
  inherited Destroy;
end;

function TApp.CreateMenuModels(settingsService: TSettingsService; languageService: ILanguageService): TMenuModel;
var
  settings: TSettingsModel;
  iconSize: TSize;
  menuClipboard: TMenuModel;
  submenuCopyFilename: TMenuModel;
  submenuCopyEmail: TMenuModel;
  submenuCopyOptions: TMenuModel;
  menuGoto: TMenuModel;
  gotoTool: TSettingsGotoToolModel;
  submenuGotoTool: TMenuModel;
  submenuGotoOptions: TMenuModel;
  menuHash: TMenuModel;
begin
  Result := TMenuModel.Create();
  Result.Title := 'ExplorerGenie';

  settings := TSettingsModel.Create();
  try
  iconSize.Width := GetSystemMetrics(SM_CXMENUCHECK);
  iconSize.Height := GetSystemMetrics(SM_CYMENUCHECK);
  settingsService.LoadSettingsOrDefault(settings);

  if (settings.CopyFileShowMenu) then
  begin
    menuClipboard := TMenuModel.Create;
    menuClipboard.Title := languageService.LoadText('menuCopyFile', 'Copy as path');
    menuClipboard.Icon := TMenuIcon.Create('icoClipboard', iconSize);
    Result.Children.Add(menuClipboard);

    submenuCopyFilename := TMenuModel.Create;
    submenuCopyFilename.Title := languageService.LoadText('submenuCopyFile', 'Copy filename(s)');
    submenuCopyFilename.Icon := TMenuIcon.Create('icoCopy', iconSize);
    submenuCopyFilename.OnClicked :=
      procedure (caller: TMenuModel)
      begin
        TActions.OnCopyFileClicked(FFilenames);
      end;
    menuClipboard.Children.Add(submenuCopyFilename);

    submenuCopyEmail := TMenuModel.Create;
    submenuCopyEmail.Title := languageService.LoadText('submenuCopyEmail', 'Copy as email link');
    submenuCopyEmail.Icon := TMenuIcon.Create('icoMail', iconSize);
    submenuCopyEmail.OnClicked :=
      procedure (caller: TMenuModel)
      begin
        TActions.OnCopyEmailClicked(FFilenames);
      end;
    menuClipboard.Children.Add(submenuCopyEmail);

    submenuCopyOptions := TMenuModel.Create;
    submenuCopyOptions.Title := languageService.LoadText('submenuOptions', 'Options');
    submenuCopyOptions.Icon := TMenuIcon.Create('icoOptions', iconSize);
    submenuCopyOptions.OnClicked :=
      procedure (caller: TMenuModel)
      begin
        TActions.OnCopyOptionsClicked(FFilenames);
      end;
    menuClipboard.Children.Add(submenuCopyOptions);
  end;

  if (settings.GotoShowMenu) then
  begin
    menuGoto := TMenuModel.Create;
    menuGoto.Title := languageService.LoadText('menuGoto', 'Go to tool');
    menuGoto.Icon := TMenuIcon.Create('icoCmd', iconSize);
    Result.Children.Add(menuGoto);

    for gotoTool in settings.GotoTools do
    begin
      if (gotoTool.Visible) then
      begin
        submenuGotoTool := TMenuModel.Create();
        submenuGotoTool.Title := gotoTool.Title;
        submenuGotoTool.Icon := TMenuIcon.Create(gotoTool.IconName, iconSize);
        submenuGotoTool.Context := gotoTool;
        submenuGotoTool.OnClicked :=
          procedure (caller: TMenuModel)
          begin
            TActions.OnGotoToolClicked(FFilenames, caller.Context as TSettingsGotoToolModel);
          end;
        menuGoto.Children.Add(submenuGotoTool);
      end;
    end;

    submenuGotoOptions := TMenuModel.Create;
    submenuGotoOptions.Title := languageService.LoadText('submenuOptions', 'Options');
    submenuGotoOptions.Icon := TMenuIcon.Create('icoOptions', iconSize);
    submenuGotoOptions.OnClicked :=
      procedure (caller: TMenuModel)
      begin
        TActions.OnGotoOptionsClicked(FFilenames);
      end;
    menuGoto.Children.Add(submenuGotoOptions);
  end;

  if (settings.HashShowMenu) then
  begin
    menuHash := TMenuModel.Create;
    menuHash.Title := languageService.LoadText('menuHash', 'Calculate hash');
    menuHash.Icon := TMenuIcon.Create('icoHash', iconSize);
    menuHash.OnClicked :=
      procedure (caller: TMenuModel)
      begin
        TActions.OnHashClicked(FFilenames);
      end;
    Result.Children.Add(menuHash);
  end;
  finally
    settings.Free();
  end;
end;

function TApp.GetTitle(psiItemArray: IShellItemArray; out ppszName: LPWSTR): HResult;
begin
  Result := FExplorerCommand.GetTitle(psiItemArray, ppszName);
end;

function TApp.GetIcon(psiItemArray: IShellItemArray; out ppszIcon: LPWSTR): HResult; stdcall;
begin
  Result := FExplorerCommand.GetIcon(psiItemArray, ppszIcon);
end;

function TApp.GetToolTip(psiItemArray: IShellItemArray; out ppszInfotip: LPWSTR): HResult; stdcall;
begin
  Result := FExplorerCommand.GetToolTip(psiItemArray, ppszInfotip);
end;

function TApp.GetCanonicalName(out pguidCommandName: TGUID): HResult; stdcall;
begin
  Result := FExplorerCommand.GetCanonicalName(pguidCommandName);
end;

function TApp.GetState(psiItemArray: IShellItemArray; fOkToBeSlow: boolean; out pCmdState: TEXPCMDSTATE): HResult; stdcall;
begin
  Result := FExplorerCommand.GetState(psiItemArray, fOkToBeSlow, pCmdState);
end;

function TApp.ExplorerCommandInvoke(psiItemArray: IShellItemArray; pbc: IBindCtx): HResult; stdcall;
begin
  Result := FExplorerCommand.Invoke(psiItemArray, pbc);
end;

function TApp.GetFlags(out pFlags: TEXPCMDFLAGS): HResult; stdcall;
begin
  Result := FExplorerCommand.GetFlags(pFlags);
end;

function TApp.EnumSubCommands(out ppEnum: IEnumExplorerCommand): HResult; stdcall;
begin
  Result := FExplorerCommand.EnumSubCommands(ppEnum);
end;

function TApp.SEIInitialize(
  pidlFolder: PItemIDList; lpdobj: IDataObject; hKeyProgID: HKEY): HRESULT; stdcall;
//var
//  formatEtc: TFormatEtc;
//  stgMedium: TStgMedium;
//  dropHandle: HDROP;
//  buffer: WideString;
//  count, index: integer;
//  length: integer;
//  filename: String;
begin
  Result := S_OK;

//  try
//  Result := E_INVALIDARG;
//  FFilenames.Clear;
//
//  // Prepare format structure
//  ZeroMemory(@formatEtc, SizeOf(TFormatEtc));
//  formatEtc.cfFormat := CF_HDROP;
//  formatEtc.ptd := nil;
//  formatEtc.dwAspect := DVASPECT_CONTENT;
//  formatEtc.lindex := -1;
//  formatEtc.tymed := TYMED_HGLOBAL;
//  stgMedium.tymed := TYMED_HGLOBAL;
//
//  // get handle
//  if (lpdobj <> nil) and Succeeded(lpdobj.GetData(formatEtc, stgMedium)) then
//  begin
//    dropHandle := HDROP(GlobalLock(stgMedium.hGlobal));
//    try
//      if (dropHandle <> 0) then
//      begin
//        // Enumerate filenames.
//        // Data can contain WideString or AnsiString(Win95, 98, ME), we catch only WideString
//        if (PDropFiles(dropHandle).fWide) then
//        begin
//          count := DragQueryFileW(dropHandle, $FFFFFFFF, nil, 0);
//          for index := 0 to count - 1 do
//          begin
//            // Get length of filename
//            length := DragQueryFileW(dropHandle, index, nil, 0);
//
//            // Allocate the memory, the #0 is not included in "length"
//            SetLength(buffer, length + 1);
//
//            // Get filename
//            DragQueryFileW(dropHandle, index, PWideChar(buffer), length + 1);
//            SetLength(buffer, length);
//            filename := buffer;
//            FFilenames.Add(filename);
//          end;
//        end;
//        Result := S_OK;
//      end;
//    finally
//      GlobalUnlock(stgMedium.hGlobal);
//      ReleaseStgMedium(stgMedium);
//    end;
//  end;
//  except
//    Result := E_FAIL; // Don't let an exception escape to the explorer process
//  end;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TApp, Class_App, ciMultiInstance, tmApartment);
end.
