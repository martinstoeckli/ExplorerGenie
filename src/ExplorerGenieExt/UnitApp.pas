// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitApp;

{$WARN SYMBOL_PLATFORM OFF}

interface

uses
  ActiveX,
  Classes,
  ComObj,
  ComServ,
  ShlObj,
  SysUtils,
  Windows,
  ExplorerGenieExt_TLB,
  Generics.Collections,
  UnitExplorerCommand,
  UnitLogger,
  UnitMenuModel,
  UnitMenuModelIcon,
  UnitMenuModelGoto,
  UnitActions,
  UnitLanguageService,
  UnitSettingsModel,
  UnitSettingsService,
  UnitSettingsGotoToolModel;

type
  /// <summary>
  /// Main class of the shell extension, it implements the required interfaces.
  /// </summary>
  TApp = class(TAutoObject, IApp, IExplorerCommand)
  private
    FMenus: IMenuModel;
    FExplorerCommand: IExplorerCommand;
    function CreateMenuModels(settingsService: TSettingsService; languageService: ILanguageService): IMenuModel;

    // IExplorerCommand
    function GetTitle(const psiItemArray: IShellItemArray; var ppszName: LPWSTR): HRESULT; stdcall;
    function GetIcon(const psiItemArray: IShellItemArray; var ppszIcon: LPWSTR): HRESULT; stdcall;
    function GetToolTip(const psiItemArray: IShellItemArray; var ppszInfotip: LPWSTR): HRESULT; stdcall;
    function GetCanonicalName(var pguidCommandName: TGUID): HRESULT; stdcall;
    function GetState(const psiItemArray: IShellItemArray; fOkToBeSlow: BOOL; var pCmdState: TExpCmdState): HRESULT; stdcall;
    function IExplorerCommand.Invoke = ExplorerCommandInvoke;
    function ExplorerCommandInvoke(const psiItemArray: IShellItemArray; const pbc: IBindCtx): HRESULT; stdcall;
    function GetFlags(var pFlags: TExpCmdFlags): HRESULT; stdcall;
    function EnumSubCommands(out ppEnum: IEnumExplorerCommand): HRESULT; stdcall;
  public
    /// <summary>
    /// Initializes a new instance of a TApp object.
    /// For COM objects this acts as a constructor.
    /// </summary>
    procedure Initialize; override;

    /// <summary>
    /// Finalizes an instance of the TApp class.
    /// </summary>
    destructor Destroy; override;
  end;

implementation
const
  APP_DESCRIPTION = 'ExplorerGenie adds tools to the explorers context menu';

procedure TApp.Initialize;
var
  languageService: ILanguageService;
  settingsService: TSettingsService;
begin
  Logger.Debug('---');
  Logger.Debug('TApp.Initialize');
  inherited Initialize;

  languageService := TLanguageServiceFactory.CreateLanguageService('ExplorerGenie');
{$IFDEF DEBUG}
  // development: Here we can force loading of a specific language.
  languageService := TLanguageServiceFactory.CreateLanguageService('ExplorerGenie', 'en');
{$ENDIF}

  settingsService := TSettingsService.Create(languageService);
  try
    FMenus := CreateMenuModels(settingsService, languageService);
    FExplorerCommand := TExplorerCommand.Create(FMenus) as IExplorerCommand;
  finally
    settingsService.Free;
  end;
end;

destructor TApp.Destroy;
begin
  Logger.Debug('TApp.Destroy');
  try
    FExplorerCommand := nil;
    FMenus := nil;
  except
    on e: Exception do
      MessageBox(0, PChar(e.Message), '', MB_ICONERROR);
  end;
  inherited Destroy;
end;

function TApp.EnumSubCommands(out ppEnum: IEnumExplorerCommand): HRESULT;
begin
  Result := FExplorerCommand.EnumSubCommands(ppEnum);
end;

function TApp.ExplorerCommandInvoke(const psiItemArray: IShellItemArray; const pbc: IBindCtx): HRESULT;
begin
  Result := FExplorerCommand.Invoke(psiItemArray, pbc);
end;

function TApp.GetCanonicalName(var pguidCommandName: TGUID): HRESULT;
begin
  Result := FExplorerCommand.GetCanonicalName(pguidCommandName);
end;

function TApp.GetFlags(var pFlags: TExpCmdFlags): HRESULT;
begin
  Result := FExplorerCommand.GetFlags(pFlags);
end;

function TApp.GetIcon(const psiItemArray: IShellItemArray; var ppszIcon: LPWSTR): HRESULT;
begin
  Result := FExplorerCommand.GetIcon(psiItemArray, ppszIcon);
end;

function TApp.GetState(const psiItemArray: IShellItemArray; fOkToBeSlow: BOOL; var pCmdState: TExpCmdState): HRESULT;
begin
  Result := FExplorerCommand.GetState(psiItemArray, fOkToBeSlow, pCmdState);
end;

function TApp.GetTitle(const psiItemArray: IShellItemArray; var ppszName: LPWSTR): HRESULT;
begin
  Result := FExplorerCommand.GetTitle(psiItemArray, ppszName);
end;

function TApp.GetToolTip(const psiItemArray: IShellItemArray; var ppszInfotip: LPWSTR): HRESULT;
begin
  Result := FExplorerCommand.GetToolTip(psiItemArray, ppszInfotip);
end;

function TApp.CreateMenuModels(settingsService: TSettingsService; languageService: ILanguageService): IMenuModel;

  function _CreateSeparator(): IMenuModel;
  begin
    Result := TMenuModel.Create();
    Result.IsSeparator := true;
  end;

var
  settings: TSettingsModel;
  menuGroupClipboard: TList<IMenuModel>;
  menuGroupGoto: TList<IMenuModel>;
  menuGroupHash: TList<IMenuModel>;
  submenuCopyFilename: IMenuModel;
  submenuCopyEmail: IMenuModel;
  submenuCopyOptions: IMenuModel;
  gotoTool: TSettingsGotoToolModel;
  submenuGotoTool: IMenuModelGoto;
  menuHash: IMenuModel;
begin
  Result := TMenuModel.Create();
  Result.Title := 'ExplorerGenie';
  Result.IconResourceId := IcoGenieLamp;

  settings := TSettingsModel.Create();
  menuGroupClipboard := TList<IMenuModel>.Create();
  menuGroupGoto := TList<IMenuModel>.Create();
  menuGroupHash := TList<IMenuModel>.Create();
  try
  settingsService.LoadSettingsOrDefault(settings);

  if (settings.CopyFileShowMenu) then
  begin
    submenuCopyFilename := TMenuModel.Create();
    submenuCopyFilename.Title := languageService.LoadText('submenuCopyFile', 'Copy filename(s)');
    submenuCopyFilename.IconResourceId := IcoCopy;
    submenuCopyFilename.OnClicked :=
      procedure (caller: IMenuModel; filenames: TStrings)
      begin
        TActions.OnCopyFileClicked(filenames);
      end;
    menuGroupClipboard.Add(submenuCopyFilename);

    submenuCopyEmail := TMenuModel.Create();
    submenuCopyEmail.Title := languageService.LoadText('submenuCopyEmail', 'Copy as email link');
    submenuCopyEmail.IconResourceId := IcoMail;
    submenuCopyEmail.OnClicked :=
      procedure (caller: IMenuModel; filenames: TStrings)
      begin
        TActions.OnCopyEmailClicked(filenames);
      end;
    menuGroupClipboard.Add(submenuCopyEmail);
  end;

  if (settings.GotoShowMenu) then
  begin
    for gotoTool in settings.GotoTools do
    begin
      if (gotoTool.Visible) then
      begin
        submenuGotoTool := TMenuModelGoto.Create();
        submenuGotoTool.Title := gotoTool.Title;
        submenuGotoTool.IconResourceId := gotoTool.IconResourceId;
        submenuGotoTool.IsCustomTool := gotoTool.IsCustomTool;
        submenuGotoTool.ToolIndex := gotoTool.ToolIndex;
        submenuGotoTool.OnClicked :=
          procedure (caller: IMenuModel; filenames: TStrings)
          begin
            TActions.OnGotoToolClicked(caller as IMenuModelGoto, filenames);
          end;
        menuGroupGoto.Add(submenuGotoTool);
      end;
    end;
  end;

  if (settings.HashShowMenu) then
  begin
    menuHash := TMenuModel.Create();
    menuHash.Title := languageService.LoadText('menuHash', 'Calculate hash');
    menuHash.IconResourceId := IcoHash;
    menuHash.OnClicked :=
      procedure (caller: IMenuModel; filenames: TStrings)
      begin
        TActions.OnHashClicked(filenames);
      end;
    menuGroupHash.Add(menuHash);
  end;

  // Add options menu
  submenuCopyOptions := TMenuModel.Create();
  submenuCopyOptions.Title := languageService.LoadText('submenuOptions', 'Options');
  submenuCopyOptions.IconResourceId := IcoOptions;
  submenuCopyOptions.OnClicked :=
    procedure (caller: IMenuModel; filenames: TStrings)
    begin
      TActions.OnCopyOptionsClicked(filenames);
    end;

  // Add separators
  if (menuGroupClipboard.Count > 0) then
    menuGroupClipboard.Add(_CreateSeparator());
  if (menuGroupGoto.Count > 0) then
    menuGroupGoto.Add(_CreateSeparator());
  if (menuGroupHash.Count > 0) then
    menuGroupHash.Add(_CreateSeparator());

  Result.AddChildren(menuGroupClipboard);
  Result.AddChildren(menuGroupGoto);
  Result.AddChildren(menuGroupHash);
  Result.AddChild(submenuCopyOptions);
  finally
    menuGroupHash.Free();
    menuGroupGoto.Free();
    menuGroupClipboard.Free();
    settings.Free();
  end;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TApp, Class_App, ciMultiInstance, tmApartment);

  Logger := CreateLoggerDummy();
{$IFDEF DEBUG}
  // Uncomment line to get a logger for debugging.
  // Logger := CreateLogger('ExplorerGenie', 'D:\Temp\ExplorerGenie.log');
{$ENDIF}

finalization
  Logger := nil;
end.
