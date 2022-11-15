library ExplorerGenieExt;

{$R 'Images.res' 'Images.rc'}

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
  UnitEnumExplorerCommand in 'UnitEnumExplorerCommand.pas';

// Windows apps are difficult to debug, because they are registered in a sandbox,
// by registering it traditionally we can debug the extentsion.
{$IFDEF DEBUG}

  /// <summary>
  /// "Overrides" the base function DllRegisterServer.
  /// </summary>
  function CustomDllRegisterServer: HResult; stdcall;
  begin
    // Call base function
    Result := ComServ.DllRegisterServer;

    // Register context menu
    try
      TContextMenuRegistrar.RegisterServer(CLASS_App, 'ExplorerGenie');
    except
      Result := E_FAIL;
    end;
  end;

  /// <summary>
  /// "Overrides" the base function DllUnregisterServer.
  /// </summary>
  function CustomDllUnregisterServer: HResult;
  begin
    // Call base function
    Result := ComServ.DllUnregisterServer;

    // Register context menu
    try
      TContextMenuRegistrar.UnregisterServer(CLASS_App, 'ExplorerGenie');
    except
      Result := E_FAIL;
    end;
  end;

exports
  DllGetClassObject,
  DllCanUnloadNow,
  CustomDllRegisterServer name 'DllRegisterServer', // Redirect the function
  CustomDllUnregisterServer name 'DllUnregisterServer', // Redirect the function
  DllInstall;

{$ELSE}

exports
  DllGetClassObject,
  DllCanUnloadNow,
  DllRegisterServer,
  DllUnregisterServer,
  DllInstall;

{$ENDIF}

{$R *.TLB}

{$R *.RES}

begin
end.
