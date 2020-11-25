// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitSettingsModel;

interface
type
  /// <summary>
  /// Model of the applications settings.
  /// </summary>
  TSettingsModel = class(TObject)
  private
    FCopyFileShowMenu: Boolean;
    FGotoShowMenu: Boolean;
    FGotoCommandPrompt: Boolean;
    FGotoPowerShell: Boolean;
    FGotoExplorer: Boolean;
  public
    constructor Create();
    procedure SetToDefault();
    property CopyFileShowMenu: Boolean read FCopyFileShowMenu write FCopyFileShowMenu;
    property GotoShowMenu: Boolean read FGotoShowMenu write FGotoShowMenu;
    property GotoCommandPrompt: Boolean read FGotoCommandPrompt write FGotoCommandPrompt;
    property GotoPowerShell: Boolean read FGotoPowerShell write FGotoPowerShell;
    property GotoExplorer: Boolean read FGotoExplorer write FGotoExplorer;
  end;

implementation

constructor TSettingsModel.Create;
begin
  SetToDefault();
end;

procedure TSettingsModel.SetToDefault;
begin
  CopyFileShowMenu := true;
  GotoShowMenu := true;
  GotoCommandPrompt := true;
  GotoPowerShell := true;
  GotoExplorer := true;
end;

end.
