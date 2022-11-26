// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitEnumExplorerCommand;

interface
uses
  ComObj,
  ShlObj,
  SysUtils,
  Windows,
  UnitLogger,
  UnitMenuModel;

type
  /// <summary>
  /// Enumerator class which wraps a TMenuModelList and implements the
  /// IEnumExplorerCommand interface required by the explorer.
  /// Access instances of this class only via interface to get reference counting.
  /// </summary>
  TEnumExplorerCommand = class(TInterfacedObject, IEnumExplorerCommand)
  private
    FModel: TMenuModelList;
    FCursor: ULONG;

    // IEnumExplorerCommand
    function Next(celt: Cardinal; out pUICommand: IExplorerCommand; var pceltFetched: Cardinal): HRESULT; stdcall;
    function Skip(celt: Cardinal): HRESULT; stdcall;
    function Reset: HRESULT; stdcall;
    function Clone(out ppenum: IEnumExplorerCommand): HRESULT; stdcall;
  public
    constructor Create(model: TMenuModelList);
    destructor Destroy(); override;

    property Model: TMenuModelList read FModel;
  end;

implementation
uses
  UnitExplorerCommand;

{ TEnumExplorerCommand }

constructor TEnumExplorerCommand.Create(model: TMenuModelList);
begin
  Logger.Debug('TEnumExplorerCommand.Create');
  inherited Create();
  FModel := model;
end;

destructor TEnumExplorerCommand.Destroy;
begin
  Logger.Debug('TEnumExplorerCommand.Destroy');
  inherited;
end;

function TEnumExplorerCommand.Clone(out ppenum: IEnumExplorerCommand): HRESULT;
begin
  Logger.Debug('TEnumExplorerCommand.Clone');

  // According to the documentation (2022), this method is not currently implemented.
  Result := E_NOTIMPL;
  ppenum := nil;
end;

function TEnumExplorerCommand.Next(celt: Cardinal; out pUICommand: IExplorerCommand; var pceltFetched: Cardinal): HRESULT;
const
  MAX_COMMANDS = 1024;
type
  TIExplorerCommandArray = array [0..MAX_COMMANDS-1] of IExplorerCommand;
  PIExplorerCommandArray = ^TIExplorerCommandArray;
var
  commandsArrayPtr: PIExplorerCommandArray;
  command: IExplorerCommand;
begin
  try
    // pUICommand is actually an array which has already memory allocated to hold all references
    // of the requested commands. MAX_COMMAND has no meaning but to define a type of a static array,
    // the memory allocation of the array is done by the Explorer.
    commandsArrayPtr := PIExplorerCommandArray(@pUICommand);
    pceltFetched := 0;
    while (pceltFetched < celt) and (FCursor < ULONG(Model.Count)) do
    begin
      command := TExplorerCommand.Create(Model[FCursor]);
      commandsArrayPtr^[pceltFetched] := command;

      Inc(FCursor);
      Inc(pceltFetched);
    end;

    if (pceltFetched = celt) then
      Result := S_OK
    else
      Result := S_FALSE;
  except
    Result := E_FAIL; // Don't let an exception escape to the explorer process
  end;
  Logger.Debug('TEnumExplorerCommand.Next'#9'fetched/requested=' + IntToStr(pceltFetched) + '/' + IntToStr(celt));
end;

function TEnumExplorerCommand.Reset: HRESULT;
begin
  Logger.Debug('TEnumExplorerCommand.Reset');

	Result := S_OK;
  FCursor := 0;
end;

function TEnumExplorerCommand.Skip(celt: Cardinal): HRESULT;
begin
  Logger.Debug('TEnumExplorerCommand.Skip');

  // According to the documentation (2022), this method is not currently implemented.
  Result := E_NOTIMPL;
end;

end.
