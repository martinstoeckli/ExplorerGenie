unit UnitExplorerCommand;

interface
uses
  ActiveX,
  Classes,
  ComObj,
  Generics.Collections,
  ShlObj,
  Windows,
  UnitMenuModel;

type
  /// <summary>
  /// Wraps a TMenuModel and implements the IExplorerCommand interface required
  /// by the explorer.
  /// Access instances of this class only via interface to get reference counting.
  /// </summary>
  TExplorerCommand = class(TComObject, IExplorerCommand)
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
    constructor Create(model: TMenuModel);

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
end;

function TExplorerCommand.EnumSubCommands(out ppEnum: IEnumExplorerCommand): HRESULT;
begin
  Result := S_OK;
  ppEnum := TEnumExplorerCommand.Create(Model.Children);
end;

function TExplorerCommand.GetCanonicalName(var pguidCommandName: TGUID): HRESULT;
begin
  Result := S_OK;
  pguidCommandName := TGuid.Empty;
end;

function TExplorerCommand.GetFlags(var pFlags: TExpCmdFlags): HRESULT;
begin
  Result := S_OK;
  if (Model.Title = '') or (Model.Title = '-') then
    pFlags := ECF_ISSEPARATOR
  else if (Model.Children.Count > 0) then
    pFlags := ECF_HASSUBCOMMANDS
  else
    pFlags := ECF_DEFAULT;
end;

function TExplorerCommand.GetIcon(const psiItemArray: IShellItemArray; var ppszIcon: LPWSTR): HRESULT;
begin
  Result := E_NOTIMPL;
  ppszIcon := nil;
end;

function TExplorerCommand.GetState(const psiItemArray: IShellItemArray; fOkToBeSlow: BOOL; var pCmdState: TExpCmdState): HRESULT;
begin
  Result := S_OK;
  pCmdState := ECS_ENABLED;
end;

function TExplorerCommand.GetTitle(const psiItemArray: IShellItemArray; var ppszName: LPWSTR): HRESULT;
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

function TExplorerCommand.GetToolTip(const psiItemArray: IShellItemArray; var ppszInfotip: LPWSTR): HRESULT;
begin
  Result := E_NOTIMPL;
  ppszInfotip := nil;
end;

function TExplorerCommand.ExplorerCommandInvoke(const psiItemArray: IShellItemArray; const pbc: IBindCtx): HRESULT;
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

end.
