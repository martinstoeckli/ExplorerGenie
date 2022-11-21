// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitApp;

{$WARN SYMBOL_PLATFORM OFF}

interface

uses
  Windows,
  Classes,
  SysUtils,
  ComObj,
  ActiveX,
  ShlObj,
  ComServ,
  ExplorerGenieExt_TLB,
  UnitExplorerCommand,
  UnitLogger,
  UnitMenuModel,
  UnitMenuModelIcon,
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
    FMenus: TMenuModel;
    FExplorerCommand: IExplorerCommand;
    function CreateMenuModels(settingsService: TSettingsService; languageService: ILanguageService): TMenuModel;

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
    FMenus.Free;
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

function TApp.CreateMenuModels(settingsService: TSettingsService; languageService: ILanguageService): TMenuModel;
var
  settings: TSettingsModel;
  iconSize: TSize;
  menuClipboard: TMenuModel;
  submenuCopyFilename: TMenuModel;
  submenuCopyEmail: TMenuModel;
  submenuCopyOptions: TMenuModel;
  menuGoto: TMenuModel;
  gotoTool: TSettingsGotoToolModel;
  submenuGotoTool: TMenuModel;
  submenuGotoOptions: TMenuModel;
  menuHash: TMenuModel;
begin
  Result := TMenuModel.Create();
  Result.Title := 'ExplorerGenie';

  settings := TSettingsModel.Create();
  try
  iconSize.Width := GetSystemMetrics(SM_CXMENUCHECK);
  iconSize.Height := GetSystemMetrics(SM_CYMENUCHECK);
  settingsService.LoadSettingsOrDefault(settings);

  if (settings.CopyFileShowMenu) then
  begin
    menuClipboard := TMenuModel.Create;
    menuClipboard.Title := languageService.LoadText('menuCopyFile', 'Copy as path');
    menuClipboard.Icon := TMenuIcon.Create('icoClipboard', iconSize);
    Result.Children.Add(menuClipboard);

    submenuCopyFilename := TMenuModel.Create;
    submenuCopyFilename.Title := languageService.LoadText('submenuCopyFile', 'Copy filename(s)');
    submenuCopyFilename.Icon := TMenuIcon.Create('icoCopy', iconSize);
    submenuCopyFilename.OnClicked :=
      procedure (caller: TMenuModel; filenames: TStrings)
      begin
        TActions.OnCopyFileClicked(filenames);
      end;
    menuClipboard.Children.Add(submenuCopyFilename);

    submenuCopyEmail := TMenuModel.Create;
    submenuCopyEmail.Title := languageService.LoadText('submenuCopyEmail', 'Copy as email link');
    submenuCopyEmail.Icon := TMenuIcon.Create('icoMail', iconSize);
    submenuCopyEmail.OnClicked :=
      procedure (caller: TMenuModel; filenames: TStrings)
      begin
        TActions.OnCopyEmailClicked(filenames);
      end;
    menuClipboard.Children.Add(submenuCopyEmail);

    submenuCopyOptions := TMenuModel.Create;
    submenuCopyOptions.Title := languageService.LoadText('submenuOptions', 'Options');
    submenuCopyOptions.Icon := TMenuIcon.Create('icoOptions', iconSize);
    submenuCopyOptions.OnClicked :=
      procedure (caller: TMenuModel; filenames: TStrings)
      begin
        TActions.OnCopyOptionsClicked(filenames);
      end;
    menuClipboard.Children.Add(submenuCopyOptions);
  end;

  if (settings.GotoShowMenu) then
  begin
    menuGoto := TMenuModel.Create;
    menuGoto.Title := languageService.LoadText('menuGoto', 'Go to tool');
    menuGoto.Icon := TMenuIcon.Create('icoCmd', iconSize);
    Result.Children.Add(menuGoto);

    for gotoTool in settings.GotoTools do
    begin
      if (gotoTool.Visible) then
      begin
        submenuGotoTool := TMenuModel.Create();
        submenuGotoTool.Title := gotoTool.Title;
        submenuGotoTool.Icon := TMenuIcon.Create(gotoTool.IconName, iconSize);
        submenuGotoTool.Context := gotoTool;
        submenuGotoTool.OnClicked :=
          procedure (caller: TMenuModel; filenames: TStrings)
          begin
            TActions.OnGotoToolClicked(filenames, caller.Context as TSettingsGotoToolModel);
          end;
        menuGoto.Children.Add(submenuGotoTool);
      end;
    end;

    submenuGotoOptions := TMenuModel.Create;
    submenuGotoOptions.Title := languageService.LoadText('submenuOptions', 'Options');
    submenuGotoOptions.Icon := TMenuIcon.Create('icoOptions', iconSize);
    submenuGotoOptions.OnClicked :=
      procedure (caller: TMenuModel; filenames: TStrings)
      begin
        TActions.OnGotoOptionsClicked(filenames);
      end;
    menuGoto.Children.Add(submenuGotoOptions);
  end;

  if (settings.HashShowMenu) then
  begin
    menuHash := TMenuModel.Create;
    menuHash.Title := languageService.LoadText('menuHash', 'Calculate hash');
    menuHash.Icon := TMenuIcon.Create('icoHash', iconSize);
    menuHash.OnClicked :=
      procedure (caller: TMenuModel; filenames: TStrings)
      begin
        TActions.OnHashClicked(filenames);
      end;
    Result.Children.Add(menuHash);
  end;
  finally
    settings.Free();
  end;
end;

initialization
  TAutoObjectFactory.Create(ComServer, TApp, Class_App, ciMultiInstance, tmApartment);

  Logger := CreateLoggerDummy();
{$IFDEF DEBUG}
  // Uncomment line to get a logger for debugging.
  Logger := CreateLogger('ExplorerGenie', 'D:\Temp\ExplorerGenie.log');
{$ENDIF}

finalization
  Logger := nil;
end.
