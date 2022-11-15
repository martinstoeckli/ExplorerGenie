unit UnitExplorerCommandMenuItem;

{$WARN SYMBOL_PLATFORM OFF}

interface
uses
  ActiveX,
  Classes,
  ComObj,
  ShlObj,
  StdVcl,
  Windows,
  ExplorerGenieExt_TLB,
  UnitMenuModel;

type
  TExplorerCommandMenuItem = class(TAutoObject, IExplorerCommandMenuItem, IExplorerCommand)
  private
    FModel: TMenuModel;
  protected
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
    property Model: TMenuModel read FModel write FModel;
  end;

implementation

uses
  ComServ,
  UnitEnumExplorerCommand;

function TExplorerCommandMenuItem.EnumSubCommands(out ppEnum: IEnumExplorerCommand): HRESULT;
begin
  Result := S_OK;
  ppEnum := TEnumExplorerCommand.Create(Model.Children);
end;

function TExplorerCommandMenuItem.GetCanonicalName(var pguidCommandName: TGUID): HRESULT;
begin
  Result := S_OK;
  pguidCommandName := TGuid.Empty;
end;

function TExplorerCommandMenuItem.GetFlags(var pFlags: TExpCmdFlags): HRESULT;
begin
  Result := S_OK;
  if (Model.Title = '') or (Model.Title = '-') then
    pFlags := ECF_ISSEPARATOR
  else if (Model.Children.Count > 0) then
    pFlags := ECF_HASSUBCOMMANDS
  else
    pFlags := ECF_DEFAULT;
end;

function TExplorerCommandMenuItem.GetIcon(const psiItemArray: IShellItemArray; var ppszIcon: LPWSTR): HRESULT;
begin
  Result := E_NOTIMPL;
  ppszIcon := nil;
end;

function TExplorerCommandMenuItem.GetState(const psiItemArray: IShellItemArray; fOkToBeSlow: BOOL; var pCmdState: TExpCmdState): HRESULT;
begin
  Result := S_OK;
  pCmdState := ECS_ENABLED;
end;

function TExplorerCommandMenuItem.GetTitle(const psiItemArray: IShellItemArray; var ppszName: LPWSTR): HRESULT;
var
  titleW: WideString;
begin
  try
  if (Model.Title = '') then
  begin
    Result := E_NOTIMPL;
    ppszName := nil;
  end
  else
  begin
    Result := S_OK;
    titleW := Model.Title;
    lstrcpyW(PWideChar(ppszName), PWideChar(titleW));
  end;
  except
    Result := E_FAIL; // Don't let an exception escape to the explorer process
  end;
end;

function TExplorerCommandMenuItem.GetToolTip(const psiItemArray: IShellItemArray; var ppszInfotip: LPWSTR): HRESULT;
begin
  Result := E_NOTIMPL;
  ppszInfotip := nil;
end;

function TExplorerCommandMenuItem.ExplorerCommandInvoke(const psiItemArray: IShellItemArray; const pbc: IBindCtx): HRESULT;
begin
  try
  Result := S_OK;
  if (Model <> nil) and Assigned(Model.OnClicked) then
  begin
    Model.OnClicked(Model);
  end;
  except
    Result := E_FAIL; // Don't let an exception escape to the explorer process
  end;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TExplorerCommand, Class_TExplorerCommandMenu,
    ciMultiInstance, tmApartment);
end.
