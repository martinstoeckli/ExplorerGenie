library ExplorerGenieExt;

{$R 'ExplorerGenieIcons.res'}

uses
  Windows,
  ComServ,
  ExplorerGenieExt_TLB in 'ExplorerGenieExt_TLB.pas',
  UnitApp in 'UnitApp.pas' {App: CoClass},
  UnitContextMenuRegistrar in 'UnitContextMenuRegistrar.pas',
  UnitMenuModel in 'UnitMenuModel.pas',
  UnitMenuModelIcon in 'UnitMenuModelIcon.pas',
  UnitActions in 'UnitActions.pas',
  UnitSettingsModel in 'UnitSettingsModel.pas',
  UnitSettingsService in 'UnitSettingsService.pas',
  UnitLanguageService in 'UnitLanguageService.pas',
  UnitSettingsGotoToolModel in 'UnitSettingsGotoToolModel.pas',
  UnitExplorerCommand in 'UnitExplorerCommand.pas',
  UnitEnumExplorerCommand in 'UnitEnumExplorerCommand.pas',
  UnitLogger in 'UnitLogger.pas';

exports
  DllGetClassObject,
  DllCanUnloadNow,
  DllRegisterServer,
  DllUnregisterServer,
  DllInstall;

{$R *.TLB}

{$R *.RES}

begin
end.
