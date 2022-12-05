// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitMenuModelGoto;

interface
uses
  UnitMenuModel;

type
  /// <summary>
  /// Inherits from IMenuModel and adds information for a Goto menu. Because they can live longer than
  /// the application itself (IExplorerCommand) they should be used reference counted by interface.
  /// </summary>
  IMenuModelGoto = interface(IMenuModel)
    ['{AD0A6D76-A225-4C49-B557-9A6FE2957C74}']
    function GetIsCustomTool(): Boolean;
    procedure SetIsCustomTool(value: Boolean);
    function GetToolIndex(): Integer;
    procedure SetToolIndex(value: Integer);

    property IsCustomTool: Boolean read GetIsCustomTool write SetIsCustomTool;
    property ToolIndex: Integer read GetToolIndex write SetToolIndex;
  end;

  /// <summary>
  /// Implementation of the IMenuModelGoto interface.
  /// Access instances of this class only via interface to get reference counting.
  /// </summary>
  TMenuModelGoto = class(TMenuModel, IMenuModelGoto)
  private
    FIsCustomTool: Boolean;
    FToolIndex: Integer;
  protected
    function GetIsCustomTool(): Boolean;
    procedure SetIsCustomTool(value: Boolean);
    function GetToolIndex(): Integer;
    procedure SetToolIndex(value: Integer);
  end;

implementation

{ TMenuModelGoto }

function TMenuModelGoto.GetIsCustomTool: Boolean;
begin
  Result := FIsCustomTool;
end;

function TMenuModelGoto.GetToolIndex: Integer;
begin
  Result := FToolIndex;
end;

procedure TMenuModelGoto.SetIsCustomTool(value: Boolean);
begin
  FIsCustomTool := value;
end;

procedure TMenuModelGoto.SetToolIndex(value: Integer);
begin
  FToolIndex := value;
end;

end.
