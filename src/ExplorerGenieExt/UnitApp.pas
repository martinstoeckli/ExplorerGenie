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
    class procedure AddMenuSeparator(menues: TMenuModelList);

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
  menuGroupClipboard: TMenuModelList;
  menuGroupGoto: TMenuModelList;
  menuGroupHash: TMenuModelList;
//  menuClipboard: TMenuModel;
  submenuCopyFilename: TMenuModel;
  submenuCopyEmail: TMenuModel;
  submenuCopyOptions: TMenuModel;
//  menuGoto: TMenuModel;
  gotoTool: TSettingsGotoToolModel;
  submenuGotoTool: TMenuModel;
//  submenuGotoOptions: TMenuModel;
  menuHash: TMenuModel;
begin
  Result := TMenuModel.Create();
  Result.Title := 'ExplorerGenie';
  Result.IconResourceId := IcoClipboard;

  settings := TSettingsModel.Create();
  menuGroupClipboard := TMenuModelList.Create(false);
  menuGroupGoto := TMenuModelList.Create(false);
  menuGroupHash := TMenuModelList.Create(false);
  try
  settingsService.LoadSettingsOrDefault(settings);

  if (settings.CopyFileShowMenu) then
  begin
//    menuClipboard := TMenuModel.Create;
//    menuClipboard.Title := languageService.LoadText('menuCopyFile', 'Copy as path');
//    menuClipboard.Icon := TMenuIcon.Create('icoClipboard', iconSize);
//    Result.Children.Add(menuClipboard);

    submenuCopyFilename := TMenuModel.Create;
    submenuCopyFilename.Title := languageService.LoadText('submenuCopyFile', 'Copy filename(s)');
    submenuCopyFilename.IconResourceId := IcoCopy;
    submenuCopyFilename.OnClicked :=
      procedure (caller: TMenuModel; filenames: TStrings)
      begin
        TActions.OnCopyFileClicked(filenames);
      end;
    menuGroupClipboard.Add(submenuCopyFilename);

    submenuCopyEmail := TMenuModel.Create;
    submenuCopyEmail.Title := languageService.LoadText('submenuCopyEmail', 'Copy as email link');
    submenuCopyEmail.IconResourceId := IcoMail;
    submenuCopyEmail.OnClicked :=
      procedure (caller: TMenuModel; filenames: TStrings)
      begin
        TActions.OnCopyEmailClicked(filenames);
      end;
    menuGroupClipboard.Add(submenuCopyEmail);
  end;

  if (settings.GotoShowMenu) then
  begin
//    menuGoto := TMenuModel.Create;
//    menuGoto.Title := languageService.LoadText('menuGoto', 'Go to tool');
//    menuGoto.Icon := TMenuIcon.Create('icoCmd', iconSize);
//    Result.Children.Add(menuGoto);
    for gotoTool in settings.GotoTools do
    begin
      if (gotoTool.Visible) then
      begin
        submenuGotoTool := TMenuModel.Create();
        submenuGotoTool.Title := gotoTool.Title;
        submenuGotoTool.IconResourceId := gotoTool.IconResourceId;
        submenuGotoTool.Context := gotoTool;
        submenuGotoTool.OnClicked :=
          procedure (caller: TMenuModel; filenames: TStrings)
          begin
            TActions.OnGotoToolClicked(filenames, caller.Context as TSettingsGotoToolModel);
          end;
        menuGroupGoto.Add(submenuGotoTool);
      end;
    end;

//    submenuGotoOptions := TMenuModel.Create;
//    submenuGotoOptions.Title := languageService.LoadText('submenuOptions', 'Options');
//    submenuGotoOptions.Icon := TMenuIcon.Create('icoOptions', iconSize);
//    submenuGotoOptions.OnClicked :=
//      procedure (caller: TMenuModel; filenames: TStrings)
//      begin
//        TActions.OnGotoOptionsClicked(filenames);
//      end;
//    Result.Children.Add(submenuGotoOptions);
  end;

  if (settings.HashShowMenu) then
  begin
    menuHash := TMenuModel.Create;
    menuHash.Title := languageService.LoadText('menuHash', 'Calculate hash');
    menuHash.IconResourceId := IcoHash;
    menuHash.OnClicked :=
      procedure (caller: TMenuModel; filenames: TStrings)
      begin
        TActions.OnHashClicked(filenames);
      end;
    menuGroupHash.Add(menuHash);
  end;

  // Add options menu
  submenuCopyOptions := TMenuModel.Create;
  submenuCopyOptions.Title := languageService.LoadText('submenuOptions', 'Options');
  submenuCopyOptions.IconResourceId := IcoOptions;
  submenuCopyOptions.OnClicked :=
    procedure (caller: TMenuModel; filenames: TStrings)
    begin
      TActions.OnCopyOptionsClicked(filenames);
    end;

  // Add separators
  if (menuGroupClipboard.Any()) then
    AddMenuSeparator(menuGroupClipboard);
  if (menuGroupGoto.Any()) then
    AddMenuSeparator(menuGroupGoto);
  if (menuGroupHash.Any()) then
    AddMenuSeparator(menuGroupHash);

  Result.Children.AddRange(menuGroupClipboard);
  Result.Children.AddRange(menuGroupGoto);
  Result.Children.AddRange(menuGroupHash);
  Result.Children.Add(submenuCopyOptions);
  finally
    menuGroupHash.Free();
    menuGroupGoto.Free();
    menuGroupClipboard.Free();
    settings.Free();
  end;
end;

class procedure TApp.AddMenuSeparator(menues: TMenuModelList);
var
  menuSeparator: TMenuModel;
begin
  menuSeparator := TMenuModel.Create();
  menuSeparator.IsSeparator := true;
  menues.Add(menuSeparator);
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
