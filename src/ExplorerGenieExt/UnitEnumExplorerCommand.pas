unit UnitEnumExplorerCommand;

interface
uses
  ComObj,
  ShlObj,
  Windows,
  UnitMenuModel;

type
  /// <summary>
  /// Enumerator class which wraps a TMenuModelList and implements the
  /// IEnumExplorerCommand interface required by the explorer.
  /// Access instances of this class only via interface to get reference counting.
  /// </summary>
  TEnumExplorerCommand = class(TComObject, IEnumExplorerCommand)
  private
    FModel: TMenuModelList;
    FCursor: Integer;

    // IEnumExplorerCommand
    function Next(celt: Cardinal; out pUICommand: IExplorerCommand; var pceltFetched: Cardinal): HRESULT; stdcall;
    function Skip(celt: Cardinal): HRESULT; stdcall;
    function Reset: HRESULT; stdcall;
    function Clone(out ppenum: IEnumExplorerCommand): HRESULT; stdcall;
  public
    constructor Create(model: TMenuModelList);

    property Model: TMenuModelList read FModel;
  end;

implementation
uses
  UnitExplorerCommand;

{ TEnumExplorerCommand }

constructor TEnumExplorerCommand.Create(model: TMenuModelList);
begin
  inherited Create();
  FModel := model;
end;

function TEnumExplorerCommand.Clone(out ppenum: IEnumExplorerCommand): HRESULT;
begin
  try
    Result := S_OK;
    ppenum := TEnumExplorerCommand.Create(TMenuModelList.Create(Model));
  except
    Result := E_FAIL; // Don't let an exception escape to the explorer process
  end;
end;

function TEnumExplorerCommand.Next(celt: Cardinal; out pUICommand: IExplorerCommand; var pceltFetched: Cardinal): HRESULT;
type
  TIExplorerCommandArray = array of IExplorerCommand;
var
  command: IExplorerCommand;
  celtAsInt: Integer;
  fetchedAsInt: Integer;
begin
  try
    celtAsInt := celt;
    fetchedAsInt := 0;
    pceltFetched := fetchedAsInt;
    while (fetchedAsInt < celtAsInt) and (FCursor < Model.Count) do
    begin
      command := TExplorerCommand.Create(Model[FCursor]);
      TIExplorerCommandArray(pUICommand)[fetchedAsInt] := command;

      Inc(FCursor);
      Inc(fetchedAsInt);
      pceltFetched := fetchedAsInt;
    end;

    if (pceltFetched = celt) then
      Result := S_OK
    else
      Result := S_FALSE;
  except
    Result := E_FAIL; // Don't let an exception escape to the explorer process
  end;
end;

function TEnumExplorerCommand.Reset: HRESULT;
begin
	Result := S_OK;
  FCursor := 0;
end;

function TEnumExplorerCommand.Skip(celt: Cardinal): HRESULT;
var
  celtAsInt: Integer;
begin
  celtAsInt := celt;
  if (FCursor + celtAsInt >= Model.Count) then
  begin
    Result := S_FALSE;
  end
  else
  begin
    Result := S_OK;
    Inc(FCursor, celtAsInt);
  end;
end;

end.
