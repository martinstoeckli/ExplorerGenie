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
    FIconName: String;
    FIsCustomTool: Boolean;
    FVisible: Boolean;
  public
    /// <summary>
    /// Initializes a new instance of the TSettingsGotoToolModel class.
    /// </summary>
    constructor Create();
    property ToolIndex: Integer read FToolIndex write FToolIndex;
    property Title: String read FTitle write FTitle;
    property IconName: String read FIconName write FIconName;
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
