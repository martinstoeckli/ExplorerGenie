// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitSettingsService;

interface
uses
  Registry,
  System.SysUtils,
  Winapi.Windows,
  UnitSettingsModel;

type
  /// <summary>
  /// Service which can load/safe the settings from the locale device.
  /// </summary>
  TSettingsService = class(TObject)
  private
  public
    /// <summary>
    /// Loads the settings from the locale device. If no settings exists, new empty
    /// settings are returned.
    /// </summary>
    /// <returns>The loaded or new created and updated settings.</returns>
    procedure LoadSettingsOrDefault(settings: TSettingsModel);
  end;

implementation

const
  KEY_APPLICATION = 'SOFTWARE\MartinStoeckli\ExplorerGenie';

{ TSettingsService }

procedure TSettingsService.LoadSettingsOrDefault(settings: TSettingsModel);
var
  registry: TRegistry;
begin
  settings.SetToDefault();

  registry := TRegistry.Create;
  try
    registry.RootKey := HKEY_CURRENT_USER;
    if (registry.OpenKeyReadOnly(KEY_APPLICATION)) then
    begin
      if (registry.ValueExists('CopyFileShowMenu')) then
        settings.CopyFileShowMenu := StrToBoolDef(registry.ReadString('CopyFileShowMenu'), settings.CopyFileShowMenu);
      if (registry.ValueExists('GotoShowMenu')) then
        settings.GotoShowMenu := StrToBoolDef(registry.ReadString('GotoShowMenu'), settings.GotoShowMenu);
      if (registry.ValueExists('GotoCommandPrompt')) then
        settings.GotoCommandPrompt := StrToBoolDef(registry.ReadString('GotoCommandPrompt'), settings.GotoCommandPrompt);
      if (registry.ValueExists('GotoPowerShell')) then
        settings.GotoPowerShell := StrToBoolDef(registry.ReadString('GotoPowerShell'), settings.GotoPowerShell);
      if (registry.ValueExists('GotoExplorer')) then
        settings.GotoExplorer := StrToBoolDef(registry.ReadString('GotoExplorer'), settings.GotoExplorer);
      if (registry.ValueExists('HashShowMenu')) then
        settings.HashShowMenu := StrToBoolDef(registry.ReadString('HashShowMenu'), settings.HashShowMenu);
    end;
  except
    // keep default values
  end;
  registry.Free;
end;

end.
