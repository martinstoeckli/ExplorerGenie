// Copyright � 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitEnumExplorerCommand;

interface
uses
  ComObj,
  ShlObj,
  Windows,
  UnitImports,
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

    // UnitImport.IEnumExplorerCommand
    function Next(celt: ULONG; out pUICommand: PIExplorerCommand; out pceltFetched: ULONG): HResult; stdcall;
    function Skip(celt: ULONG): HResult; stdcall;
    function Reset(): HResult; stdcall;
    function Clone(out ppenum: IEnumExplorerCommand): HResult; stdcall;
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
  // According to the documentation (2022), this method is not currently implemented.
  Result := E_NOTIMPL;
  ppenum := nil;
end;

function TEnumExplorerCommand.Next(celt: ULONG; out pUICommand: PIExplorerCommand; out pceltFetched: ULONG): HResult;
type
  TIExplorerCommandArray = array of IExplorerCommand;
var
  commands: TIExplorerCommandArray;
  command: IExplorerCommand;
begin
  try
    commands := TIExplorerCommandArray(pUICommand);
    pceltFetched := 0;
    while (pceltFetched < celt) and (FCursor < ULONG(Model.Count)) do
    begin
      command := TExplorerCommand.Create(Model[FCursor]);
      commands[pceltFetched] := command;

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
end;

//function TEnumExplorerCommand.Next(celt: ULONG; out pUICommand: PIExplorerCommand; out pceltFetched: ULONG): HResult;
//type
//  TIExplorerCommandArray = array of IExplorerCommand;
//var
//  command: IExplorerCommand;
//  celtAsInt: Integer;
//  fetchedAsInt: Integer;
//begin
//  try
//    celtAsInt := celt;
//    fetchedAsInt := 0;
//    pceltFetched := fetchedAsInt;
//    while (fetchedAsInt < celtAsInt) and (FCursor < Model.Count) do
//    begin
//      command := TExplorerCommand.Create(Model[FCursor]);
//      TIExplorerCommandArray(pUICommand)[fetchedAsInt] := command;
//
//      Inc(FCursor);
//      Inc(fetchedAsInt);
//      pceltFetched := fetchedAsInt;
//    end;
//
//    if (pceltFetched = celt) then
//      Result := S_OK
//    else
//      Result := S_FALSE;
//  except
//    Result := E_FAIL; // Don't let an exception escape to the explorer process
//  end;
//end;

function TEnumExplorerCommand.Reset: HRESULT;
begin
	Result := S_OK;
  FCursor := 0;
end;

function TEnumExplorerCommand.Skip(celt: Cardinal): HRESULT;
begin
  // According to the documentation (2022), this method is not currently implemented.
  Result := E_NOTIMPL;
end;

end.
