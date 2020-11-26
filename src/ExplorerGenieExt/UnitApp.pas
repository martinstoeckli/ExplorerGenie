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
  UnitMenuModel,
  UnitMenuModelIcon,
  UnitActions,
  UnitLanguageService,
  UnitSettingsModel,
  UnitSettingsService;

type
  /// <summary>
  /// Main class of the shell extension, it implements the required interfaces.
  /// </summary>
  TApp = class(TAutoObject, IApp, IContextMenu, IShellExtInit)
  private
    FMenus: TMenuModelList;
    FFilenames: TStringList;
    function CreateMenuModels(settingsService: TSettingsService; languageService: ILanguageService): TMenuModelList;
    procedure BuildContextMenus(
      menus: TMenuModelList; parent: HMENU; startInsertingAt: UINT; firstFreeCmdId: UINT; var nextFreeCmdId: UINT);

    // IContextMenu
    function QueryContextMenu(
      Menu: HMENU; indexMenu, idCmdFirst, idCmdLast, uFlags: UINT): HResult; stdcall;
    function InvokeCommand(var lpici: TCMInvokeCommandInfo): HResult; stdcall;
    function GetCommandString(
      idcmd: UINT_Ptr; uType: UINT; pwreserved: puint; pszName: LPStr; cchMax: uint): HResult; stdcall;

    // IShellExtInit
    function IShellExtInit.Initialize = SEIInitialize; // avoid compiler warning
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
  settingsService: TSettingsService;
  languageService: ILanguageService;
begin
  inherited Initialize;
  FFilenames := TStringList.Create;

  settingsService := TSettingsService.Create();
  languageService := TLanguageServiceFactory.CreateLanguageService('ExplorerGenie');
  try
    FMenus := CreateMenuModels(settingsService, languageService);
  finally
    languageService := nil;
    settingsService.Free;
  end;
end;

destructor TApp.Destroy;
begin
  try
    FMenus.Free;
    FFilenames.Free;
  except
    on e: Exception do
      MessageBox(0, PChar(e.Message), '', MB_ICONERROR);
  end;
  inherited Destroy;
end;

function TApp.CreateMenuModels(settingsService: TSettingsService; languageService: ILanguageService): TMenuModelList;
var
  settings: TSettingsModel;
  iconSize: TSize;
  menuClipboard: TMenuModel;
  submenuCopyFilename: TMenuModel;
  submenuCopyEmail: TMenuModel;
  submenuCopyOptions: TMenuModel;
  menuGoto: TMenuModel;
  submenuGotoCmd: TMenuModel;
  submenuGotoCmdAdmin: TMenuModel;
  submenuGotoPowershell: TMenuModel;
  submenuGotoPowershellAdmin: TMenuModel;
  submenuGotoExplorer: TMenuModel;
  submenuGotoExplorerAdmin: TMenuModel;
  submenuGotoOptions: TMenuModel;
  menuHash: TMenuModel;
begin
  Result := TMenuModelList.Create(true);
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
    Result.Add(menuClipboard);

    submenuCopyFilename := TMenuModel.Create;
    submenuCopyFilename.Title := languageService.LoadText('submenuCopyFile', 'Copy filename(s)');
    submenuCopyFilename.Icon := TMenuIcon.Create('icoCopy', iconSize);
    submenuCopyFilename.OnClicked :=
      procedure
      begin
        TActions.OnCopyFileClicked(FFilenames);
      end;
    menuClipboard.Children.Add(submenuCopyFilename);

    submenuCopyEmail := TMenuModel.Create;
    submenuCopyEmail.Title := languageService.LoadText('submenuCopyEmail', 'Copy as email link');
    submenuCopyEmail.Icon := TMenuIcon.Create('icoMail', iconSize);
    submenuCopyEmail.OnClicked :=
      procedure
      begin
        TActions.OnCopyEmailClicked(FFilenames);
      end;
    menuClipboard.Children.Add(submenuCopyEmail);

    submenuCopyOptions := TMenuModel.Create;
    submenuCopyOptions.Title := languageService.LoadText('submenuOptions', 'Options');
    submenuCopyOptions.Icon := TMenuIcon.Create('icoOptions', iconSize);
    submenuCopyOptions.OnClicked :=
      procedure
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
    Result.Add(menuGoto);

    if (settings.GotoCommandPrompt) then
    begin
      submenuGotoCmd := TMenuModel.Create;
      submenuGotoCmd.Title := languageService.LoadText('submenuGotoCmd', 'Open in Command Prompt');
      submenuGotoCmd.Icon := TMenuIcon.Create('icoCmd', iconSize);
      submenuGotoCmd.OnClicked :=
        procedure
        begin
          TActions.OnGotoCmdClicked(FFilenames, false);
        end;
      menuGoto.Children.Add(submenuGotoCmd);

      submenuGotoCmdAdmin := TMenuModel.Create;
      submenuGotoCmdAdmin.Title := languageService.LoadText('submenuGotoCmdAdmin', 'Open in Command Prompt as admin');
      submenuGotoCmdAdmin.Icon := TMenuIcon.Create('icoCmd', iconSize);
      submenuGotoCmdAdmin.OnClicked :=
        procedure
        begin
          TActions.OnGotoCmdClicked(FFilenames, true);
        end;
      menuGoto.Children.Add(submenuGotoCmdAdmin);
    end;

    if (settings.GotoPowerShell) then
    begin
      submenuGotoPowershell := TMenuModel.Create;
      submenuGotoPowershell.Title := languageService.LoadText('submenuGotoPowershell', 'Open in Power Shell');
      submenuGotoPowershell.Icon := TMenuIcon.Create('icoPowershell', iconSize);
      submenuGotoPowershell.OnClicked :=
        procedure
        begin
          TActions.OnGotoPowershellClicked(FFilenames, false);
        end;
      menuGoto.Children.Add(submenuGotoPowershell);

      submenuGotoPowershellAdmin := TMenuModel.Create;
      submenuGotoPowershellAdmin.Title := languageService.LoadText('submenuGotoPowershellAdmin', 'Open in Power Shell as admin');
      submenuGotoPowershellAdmin.Icon := TMenuIcon.Create('icoPowershell', iconSize);
      submenuGotoPowershellAdmin.OnClicked :=
        procedure
        begin
          TActions.OnGotoPowershellClicked(FFilenames, true);
        end;
      menuGoto.Children.Add(submenuGotoPowershellAdmin);
    end;

    if (settings.GotoExplorer) then
    begin
      submenuGotoExplorer := TMenuModel.Create;
      submenuGotoExplorer.Title := languageService.LoadText('submenuGotoExplorer', 'Open in Explorer');
      submenuGotoExplorer.Icon := TMenuIcon.Create('icoExplorer', iconSize);
      submenuGotoExplorer.OnClicked :=
        procedure
        begin
          TActions.OnGotoExplorerClicked(FFilenames, false);
        end;
      menuGoto.Children.Add(submenuGotoExplorer);

      submenuGotoExplorerAdmin := TMenuModel.Create;
      submenuGotoExplorerAdmin.Title := languageService.LoadText('submenuGotoExplorerAdmin', 'Open in Explorer as admin');
      submenuGotoExplorerAdmin.Icon := TMenuIcon.Create('icoExplorer', iconSize);
      submenuGotoExplorerAdmin.OnClicked :=
        procedure
        begin
          TActions.OnGotoExplorerClicked(FFilenames, true);
        end;
      menuGoto.Children.Add(submenuGotoExplorerAdmin);
    end;

    submenuGotoOptions := TMenuModel.Create;
    submenuGotoOptions.Title := languageService.LoadText('submenuOptions', 'Options');
    submenuGotoOptions.Icon := TMenuIcon.Create('icoOptions', iconSize);
    submenuGotoOptions.OnClicked :=
      procedure
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
      procedure
      begin
        TActions.OnHashClicked(FFilenames);
      end;
    Result.Add(menuHash);
  end;
  finally
    settings.Free();
  end;
end;

function TApp.QueryContextMenu(
  Menu: HMENU; indexMenu, idCmdFirst, idCmdLast, uFlags: UINT): HResult; stdcall;
var
  nextFreeCmdId: UINT;
  offsetForNextIdCmdFirst: Integer;
begin
  try
  Result := 0;
  if ((uFlags and $0000000F) = CMF_NORMAL) or ((uFlags and CMF_EXPLORE) <> 0) then
  begin
    nextFreeCmdId := idCmdFirst;
    BuildContextMenus(FMenus, Menu, indexMenu, idCmdFirst, nextFreeCmdId);
    offsetForNextIdCmdFirst := nextFreeCmdId - idCmdFirst; // Highest used id + 1 - first used id
    Result := MakeResult(SEVERITY_SUCCESS, 0, offsetForNextIdCmdFirst);
  end;
  except
    Result := 0; // Don't let an exception escape to the explorer process
  end;
end;

procedure TApp.BuildContextMenus(
  menus: TMenuModelList; parent: HMENU; startInsertingAt: UINT; firstFreeCmdId: UINT; var nextFreeCmdId: UINT);
var
  index: Integer;
  menuModel: TMenuModel;
  menuItemInfo: TMenuItemInfo;
  popupMenu: HMENU;
begin
  for index := 0 to menus.Count - 1 do
  begin
    menuModel := menus[index];
    menuModel.RelativeCmdId := nextFreeCmdId - firstFreeCmdId; // Later we need the relative id.
    Inc(nextFreeCmdId);

    ZeroMemory(@menuItemInfo, SizeOf(TMenuItemInfo));
    menuItemInfo.cbSize := SizeOf(TMenuItemInfo);
    if (menuModel.Icon <> nil) then
      menuItemInfo.hbmpItem := menuModel.Icon.BitmapHandle;
    menuItemInfo.dwTypeData := PChar(menuModel.Title);
    menuItemInfo.wID := firstFreeCmdId + menuModel.RelativeCmdId; // This must be the absolute id.

    if (menuModel.HasChildren) then
    begin
      // This is a group menu with sub items
      popupMenu := CreatePopupMenu;
      menuItemInfo.fMask := MIIM_ID or MIIM_STRING or MIIM_BITMAP or MIIM_SUBMENU;
      menuItemInfo.hSubMenu := popupMenu;
      InsertMenuItem(parent, startInsertingAt + UINT(index), True, menuItemInfo);

      // Recursive call for children
      BuildContextMenus(menuModel.Children, popupMenu, 0, firstFreeCmdId, nextFreeCmdId);
    end
    else
    begin
      // This is a normal menu item with an action
      menuItemInfo.fMask := MIIM_ID or MIIM_STRING or MIIM_BITMAP;
      InsertMenuItem(parent, startInsertingAt + UINT(index), True, menuItemInfo);
    end;
  end;
end;

function TApp.InvokeCommand(var lpici: TCMInvokeCommandInfo): HResult; stdcall;
var
  relativeCmdId: WORD;
  menuModel: TMenuModel;
begin
  Result := E_FAIL;

  // Make sure we can handle the command
  if (HiWord(DWORD(lpici.lpVerb)) = 0) then
  begin
    Result := S_OK;

    // Find the clicked menu item
    try
      relativeCmdId := LoWord(DWORD(lpici.lpVerb));
      menuModel := FMenus.FindByRelativeCmdId(relativeCmdId);
      if (menuModel <> nil) and Assigned(menuModel.OnClicked) then
        menuModel.OnClicked();
    except
      Result := E_FAIL; // Don't let an exception escape to the explorer process
    end;
  end;
end;

function TApp.GetCommandString(
  idcmd: UINT_Ptr; uType: UINT; pwreserved: puint; pszName: LPStr; cchMax: uint): HResult; stdcall;
var
  descriptionA: AnsiString;
  descriptionW: WideString;
begin
  try
  Result := S_OK;
  case uType of
  GCS_HELPTEXTA:
    begin
      descriptionA := APP_DESCRIPTION;
      lstrcpynA(PAnsiChar(pszName), PAnsiChar(descriptionA), cchMax);
    end;
  GCS_HELPTEXTW:
    begin
      descriptionW := APP_DESCRIPTION;
      lstrcpynW(PWideChar(pszName), PWideChar(descriptionW), cchMax);
    end
  else
    Result := E_NOTIMPL;
  end;
  except
    Result := E_FAIL; // Don't let an exception escape to the explorer process
  end;
end;

function TApp.SEIInitialize(
  pidlFolder: PItemIDList; lpdobj: IDataObject; hKeyProgID: HKEY): HRESULT; stdcall;
var
  formatEtc: TFormatEtc;
  stgMedium: TStgMedium;
  dropHandle: HDROP;
  buffer: WideString;
  count, index: integer;
  length: integer;
  filename: String;
begin
  try
  Result := E_INVALIDARG;
  FFilenames.Clear;

  // Prepare format structure
  ZeroMemory(@formatEtc, SizeOf(TFormatEtc));
  formatEtc.cfFormat := CF_HDROP;
  formatEtc.ptd := nil;
  formatEtc.dwAspect := DVASPECT_CONTENT;
  formatEtc.lindex := -1;
  formatEtc.tymed := TYMED_HGLOBAL;
  stgMedium.tymed := TYMED_HGLOBAL;

  // get handle
  if (lpdobj <> nil) and Succeeded(lpdobj.GetData(formatEtc, stgMedium)) then
  begin
    dropHandle := HDROP(GlobalLock(stgMedium.hGlobal));
    try
      if (dropHandle <> 0) then
      begin
        // Enumerate filenames.
        // Data can contain WideString or AnsiString(Win95, 98, ME), we catch only WideString
        if (PDropFiles(dropHandle).fWide) then
        begin
          count := DragQueryFileW(dropHandle, $FFFFFFFF, nil, 0);
          for index := 0 to count - 1 do
          begin
            // Get length of filename
            length := DragQueryFileW(dropHandle, index, nil, 0);

            // Allocate the memory, the #0 is not included in "length"
            SetLength(buffer, length + 1);

            // Get filename
            DragQueryFileW(dropHandle, index, PWideChar(buffer), length + 1);
            SetLength(buffer, length);
            filename := buffer;
            FFilenames.Add(filename);
          end;
        end;
        Result := S_OK;
      end;
    finally
      GlobalUnlock(stgMedium.hGlobal);
      ReleaseStgMedium(stgMedium);
    end;
  end;
  except
    Result := E_FAIL; // Don't let an exception escape to the explorer process
  end;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TApp, Class_App, ciMultiInstance, tmApartment);
end.
