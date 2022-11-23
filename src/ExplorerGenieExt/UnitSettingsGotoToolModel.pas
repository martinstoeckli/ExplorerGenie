// Copyright © 2021 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitSettingsGotoToolModel;

interface
type
  /// <summary>
  /// Model of a single goto tool, either a predefined or a user defined tool.
  /// </summary>
  TSettingsGotoToolModel = class(TObject)
  private
    FToolIndex: Integer;
    FTitle: String;
    FIconResourceId: Integer;
    FIsCustomTool: Boolean;
    FVisible: Boolean;
  public
    /// <summary>
    /// Initializes a new instance of the TSettingsGotoToolModel class.
    /// </summary>
    constructor Create();
    property ToolIndex: Integer read FToolIndex write FToolIndex;
    property Title: String read FTitle write FTitle;
    property IconResourceId: Integer read FIconResourceId write FIconResourceId;
    property IsCustomTool: Boolean read FIsCustomTool write FIsCustomTool;
    property Visible: Boolean read FVisible write FVisible;
  end;

implementation

{ TSettingsGotoToolModel }

constructor TSettingsGotoToolModel.Create;
begin
  Visible := true;
end;

end.
