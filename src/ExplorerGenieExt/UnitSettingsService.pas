// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitSettingsService;

interface
uses
  Registry,
  System.Classes,
  System.JSON.Readers,
  System.JSON.Types,
  System.SysUtils,
  Winapi.Windows,
  UnitLanguageService,
  UnitSettingsModel,
  UnitSettingsGotoToolModel;

type
  /// <summary>
  /// Service which can load/safe the settings from the locale device.
  /// </summary>
  TSettingsService = class(TObject)
  private
    FLanguage: ILanguageService;
    procedure AddPredefinedGotoTools(settings: TSettingsModel);
  public
    /// <summary>
    /// Initializes a new instance of the TSettingsService class.
    /// </summary>
    /// <param name="language">Language service.</param>
    constructor Create(language: ILanguageService);

    /// <summary>
    /// Loads the settings from the locale device. If no settings exists, new empty
    /// settings are returned.
    /// </summary>
    /// <returns>The loaded or new created and updated settings.</returns>
    procedure LoadSettingsOrDefault(settings: TSettingsModel);

    /// <summary>
    /// Loads the custom goto tools from a given XML and adds them to the settings list.
    /// </summary>
    /// <param name="settings">The settings whose goto tool list receives the new items.</param>
    /// <param name="serializedTools">The JSON stored in the registry.</param>
    class procedure AddCustomGotoTools(settings: TSettingsModel; serializedTools: String);
  end;

implementation

const
  KEY_APPLICATION = 'SOFTWARE\MartinStoeckli\ExplorerGenie';

{ TSettingsService }

constructor TSettingsService.Create(language: ILanguageService);
begin
  FLanguage := language;
end;

procedure TSettingsService.LoadSettingsOrDefault(settings: TSettingsModel);
var
  registry: TRegistry;
  isVisible: Boolean;
  customGotoTools: String;
begin
  settings.SetToDefault();
  AddPredefinedGotoTools(settings);

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
      begin
        isVisible := StrToBoolDef(registry.ReadString('GotoCommandPrompt'), true);
        settings.GotoTools[0].Visible := isVisible;
        settings.GotoTools[1].Visible := isVisible;
      end;
      if (registry.ValueExists('GotoPowerShell')) then
      begin
        isVisible := StrToBoolDef(registry.ReadString('GotoPowerShell'), true);
        settings.GotoTools[2].Visible := isVisible;
        settings.GotoTools[3].Visible := isVisible;
      end;
      if (registry.ValueExists('GotoExplorer')) then
      begin
        isVisible := StrToBoolDef(registry.ReadString('GotoExplorer'), true);
        settings.GotoTools[4].Visible := isVisible;
        settings.GotoTools[5].Visible := isVisible;
      end;
      if (registry.ValueExists('CustomGotoTools')) then
        customGotoTools := registry.ReadString('CustomGotoTools');
      if (registry.ValueExists('HashShowMenu')) then
        settings.HashShowMenu := StrToBoolDef(registry.ReadString('HashShowMenu'), settings.HashShowMenu);
      AddCustomGotoTools(settings, customGotoTools);
    end;
  except
    // keep default values
  end;
  registry.Free;
end;

procedure TSettingsService.AddPredefinedGotoTools(settings: TSettingsModel);
var
  gotoTool: TSettingsGotoToolModel;
begin
  gotoTool := TSettingsGotoToolModel.Create();
  gotoTool.ToolIndex := 0;
  gotoTool.Title := FLanguage.LoadText('submenuGotoCmd', 'Open in Command Prompt');
  gotoTool.IconName := 'icoCmd';
  gotoTool.IsCustomTool := false;
  settings.GotoTools.Add(gotoTool);

  gotoTool := TSettingsGotoToolModel.Create();
  gotoTool.ToolIndex := 1;
  gotoTool.Title := FLanguage.LoadText('submenuGotoCmdAdmin', 'Open in Command Prompt as admin');
  gotoTool.IconName := 'icoCmd';
  gotoTool.IsCustomTool := false;
  settings.GotoTools.Add(gotoTool);

  gotoTool := TSettingsGotoToolModel.Create();
  gotoTool.ToolIndex := 2;
  gotoTool.Title := FLanguage.LoadText('submenuGotoPowershell', 'Open in Power Shell');
  gotoTool.IconName := 'icoPowershell';
  gotoTool.IsCustomTool := false;
  settings.GotoTools.Add(gotoTool);

  gotoTool := TSettingsGotoToolModel.Create();
  gotoTool.ToolIndex := 3;
  gotoTool.Title := FLanguage.LoadText('submenuGotoPowershellAdmin', 'Open in Power Shell as admin');
  gotoTool.IconName := 'icoPowershell';
  gotoTool.IsCustomTool := false;
  settings.GotoTools.Add(gotoTool);

  gotoTool := TSettingsGotoToolModel.Create();
  gotoTool.ToolIndex := 4;
  gotoTool.Title := FLanguage.LoadText('submenuGotoExplorer', 'Open in Explorer');
  gotoTool.IconName := 'icoExplorer';
  gotoTool.IsCustomTool := false;
  settings.GotoTools.Add(gotoTool);

  gotoTool := TSettingsGotoToolModel.Create();
  gotoTool.ToolIndex := 5;
  gotoTool.Title := FLanguage.LoadText('submenuGotoExplorerAdmin', 'Open in Explorer as admin');
  gotoTool.IconName := 'icoExplorer';
  gotoTool.IsCustomTool := false;
  settings.GotoTools.Add(gotoTool);
end;

class procedure TSettingsService.AddCustomGotoTools(settings: TSettingsModel; serializedTools: String);
var
  stringReader: TStringReader;
  jsonReader: TJsonTextReader;
  gotoToolIndex: Integer;
  gotoTool: TSettingsGotoToolModel;
  lastPropertyName: String;
begin
  if (Trim(serializedTools) = '') then
    Exit;

  // The TJsonTextReader has the lowest possible performance impact (better than TJSONObject.ParseJSONValue)
  // and in contrast to IXmlDocument it doesn't has dependencies to a COM object.
  stringReader := TStringReader.Create(serializedTools);
  jsonReader := TJsonTextReader.Create(stringReader);
  try
    gotoToolIndex := 0;
    gotoTool := nil;
    lastPropertyName := '';
    while jsonReader.Read() do
    begin
      case jsonReader.TokenType of
        TJsonToken.PropertyName:
          begin
            lastPropertyName := LowerCase(jsonReader.Value.AsString());
          end;
        TJsonToken.String:
          begin
            if (lastPropertyName = 'menutitle') then
            begin
              gotoTool := TSettingsGotoToolModel.Create();
              gotoTool.ToolIndex := gotoToolIndex;
              gotoTool.Title := jsonReader.Value.AsString();
              gotoTool.IconName := 'icoCustomTool';
              gotoTool.IsCustomTool := true;
              gotoTool.Visible := false; // check later in commandline property
              settings.GotoTools.Add(gotoTool);
              Inc(gotoToolIndex);
            end
            else if (lastPropertyName = 'commandline') then
            begin
              if (gotoTool <> nil) then
                gotoTool.Visible := (Trim(gotoTool.Title) <> '') and (Trim(jsonReader.Value.AsString()) <> '');
              gotoTool := nil;
            end;
          end;
      end;
    end;
  finally
    jsonReader.Free;
    stringReader.Free;
  end;
end;

end.
