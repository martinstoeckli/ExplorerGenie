// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitSettingsModel;

interface
uses
  Generics.Collections,
  UnitSettingsGotoToolModel;

type
  /// <summary>
  /// Model of the applications settings.
  /// </summary>
  TSettingsModel = class(TObject)
  private
    FCopyFileShowMenu: Boolean;
    FGotoShowMenu: Boolean;
    FHashShowMenu: Boolean;
    FGotoTools: TObjectList<TSettingsGotoToolModel>;
  public
    constructor Create();
    destructor Destroy(); override;
    procedure SetToDefault();
    property CopyFileShowMenu: Boolean read FCopyFileShowMenu write FCopyFileShowMenu;
    property GotoShowMenu: Boolean read FGotoShowMenu write FGotoShowMenu;
    property HashShowMenu: Boolean read FHashShowMenu write FHashShowMenu;
    property GotoTools: TObjectList<TSettingsGotoToolModel> read FGotoTools;
  end;

implementation

constructor TSettingsModel.Create;
begin
  FGotoTools := TObjectList<TSettingsGotoToolModel>.Create(true);
  SetToDefault();
end;

destructor TSettingsModel.Destroy;
begin
  FGotoTools.Free;
  inherited Destroy;
end;

procedure TSettingsModel.SetToDefault;
begin
  CopyFileShowMenu := true;
  GotoShowMenu := true;
  HashShowMenu := true;
  FGotoTools.Clear();
end;

end.
