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
  UnitMenuModelIcon;

type
  /// <summary>
  /// Main class of the shell extension, it implements the required interfaces.
  /// </summary>
  TApp = class(TAutoObject, IApp, IContextMenu, IShellExtInit)
  private
    FMenus: TMenuModelList;
    FFilenames: TStringList;
    function CreateMenuModels(): TMenuModelList;
    procedure BuildContextMenus(
      menus: TMenuModelList; parent: HMENU; startInsertingAt: UINT; firstFreeCmdId: UINT; var nextFreeCmdId: UINT);

    class procedure OnCopyFileClicked(filenames: TStrings);
    class procedure OnCopyEmailClicked(filenames: TStrings);
    class procedure OnCopyOptionsClicked(filenames: TStrings);

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
    /// Initializes a new instance of a TApp Com object.
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
begin
  inherited Initialize;
  FFilenames := TStringList.Create;
  FMenus := CreateMenuModels();
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

function TApp.CreateMenuModels(): TMenuModelList;
var
  iconSize: TSize;
  menuClipboard: TMenuModel;
  submenuCopyFilename: TMenuModel;
  submenuCopyEmail: TMenuModel;
  submenuCopyOptions: TMenuModel;
begin
  iconSize.Width := GetSystemMetrics(SM_CXMENUCHECK);
  iconSize.Height := GetSystemMetrics(SM_CYMENUCHECK);

  submenuCopyFilename := TMenuModel.Create;
  submenuCopyFilename.Title := 'Copy filename(s)';
  submenuCopyFilename.Icon := TMenuIcon.Create('icoCopy', iconSize);
  submenuCopyFilename.OnClicked :=
    procedure
    begin
      OnCopyFileClicked(FFilenames);
    end;

  submenuCopyEmail := TMenuModel.Create;
  submenuCopyEmail.Title := 'Copy as email link';
  submenuCopyEmail.Icon := TMenuIcon.Create('icoMail', iconSize);
  submenuCopyEmail.OnClicked :=
    procedure
    begin
      OnCopyEmailClicked(FFilenames);
    end;

  submenuCopyOptions := TMenuModel.Create;
  submenuCopyOptions.Title := 'Options';
  submenuCopyOptions.Icon := TMenuIcon.Create('icoOptions', iconSize);
  submenuCopyOptions.OnClicked :=
    procedure
    begin
      OnCopyOptionsClicked(FFilenames);
    end;

  menuClipboard := TMenuModel.Create;
  menuClipboard.Title := 'Copy path';
  menuClipboard.Icon := TMenuIcon.Create('icoClipboard', iconSize);
  menuClipboard.Children.Add(submenuCopyFilename);
  menuClipboard.Children.Add(submenuCopyEmail);
  menuClipboard.Children.Add(submenuCopyOptions);

  Result := TMenuModelList.Create(true);
  Result.Add(menuClipboard)
end;

class procedure TApp.OnCopyFileClicked(filenames: TStrings);
begin
end;

class procedure TApp.OnCopyEmailClicked(filenames: TStrings);
begin
end;

class procedure TApp.OnCopyOptionsClicked(filenames: TStrings);
begin
end;

function TApp.QueryContextMenu(
  Menu: HMENU; indexMenu, idCmdFirst, idCmdLast, uFlags: UINT): HResult; stdcall;
var
  nextFreeCmdId: UINT;
  offsetForNextIdCmdFirst: Integer;
begin
  Result := 0;
  if ((uFlags and $0000000F) = CMF_NORMAL) or ((uFlags and CMF_EXPLORE) <> 0) then
  begin
    nextFreeCmdId := idCmdFirst;
    BuildContextMenus(FMenus, Menu, indexMenu, idCmdFirst, nextFreeCmdId);
    offsetForNextIdCmdFirst := nextFreeCmdId - idCmdFirst; // Highest used id + 1 - first used id
    Result := MakeResult(SEVERITY_SUCCESS, 0, offsetForNextIdCmdFirst);
  end;
end;

procedure TApp.BuildContextMenus(
  menus: TMenuModelList; parent: HMENU; startInsertingAt: UINT; firstFreeCmdId: UINT; var nextFreeCmdId: UINT);
var
  index: UINT;
  menuModel: TMenuModel;
  menuItemInfo: TMenuItemInfo;
  popupMenu: HMENU;
begin
  for index := 0 to menus.Count -1 do
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
      InsertMenuItem(parent, startInsertingAt + index, True, menuItemInfo);

      // Recursive call for children
      BuildContextMenus(menuModel.Children, popupMenu, 0, firstFreeCmdId, nextFreeCmdId);
    end
    else
    begin
      // This is a normal menu item with an action
      menuItemInfo.fMask := MIIM_ID or MIIM_STRING or MIIM_BITMAP;
      InsertMenuItem(parent, startInsertingAt + index, True, menuItemInfo);
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
    end;
  end;
end;

function TApp.GetCommandString(
  idcmd: UINT_Ptr; uType: UINT; pwreserved: puint; pszName: LPStr; cchMax: uint): HResult; stdcall;
var
  descriptionA: AnsiString;
  descriptionW: WideString;
begin
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
end;

initialization
  TAutoObjectFactory.Create(ComServer, TApp, Class_App, ciMultiInstance, tmApartment);
end.
