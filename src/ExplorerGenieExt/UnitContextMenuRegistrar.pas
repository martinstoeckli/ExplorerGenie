// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitContextMenuRegistrar;

interface
uses
  Windows,
  Registry,
  SysUtils;

type
  /// <summary>
  /// Creates the registry entries for a COM object which acts as a context menu of the Windows
  /// explorer (shell extension). The COM object itself should be registered separately.
  /// </summary>
  TContextMenuRegistrar = class(TObject)
  private
    class procedure CreateRegistryValue(const rootKey: HKEY; const key, valueName, value: String);
    class procedure DeleteRegistryKey(const rootKey: HKEY; const key: String);
    class procedure DeleteRegistryValue(const rootKey: HKEY; const key, valueName: String);
  public
    /// <summary>
    /// Creates all necessary registry keys for the context menu.
    /// </summary>
    /// <param name="classId">The guid of the application.</param>
    /// <param name="name">The name of the application.</param>
    class procedure RegisterServer(classId: TGUID; const name: String); virtual;

    /// <summary>
    /// Removes all registry keys of the context menu.
    /// </summary>
    /// <param name="classId">The guid of the application.</param>
    /// <param name="name">The name of the application.</param>
    class procedure UnregisterServer(classId: TGUID; const name: String); virtual;
  end;

implementation

class procedure TContextMenuRegistrar.RegisterServer(classId: TGUID; const name: String);
var
  classIdString: String;
  key: String;
begin
  classIdString := GUIDToString(classId);

  // register extension for all file types.
  key := '*\shellex\ContextMenuHandlers\' + name;
  CreateRegistryValue(HKEY_CLASSES_ROOT, key, '', classIdString);

  // register extension for folders
  key := 'Directory\shellex\ContextMenuHandlers\' + name;
  CreateRegistryValue(HKEY_CLASSES_ROOT, key, '', classIdString);

  // enter shell extension as approved
  key := 'SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved';
  CreateRegistryValue(HKEY_LOCAL_MACHINE, key, classIdString, name);
end;

class procedure TContextMenuRegistrar.UnregisterServer(classId: TGUID; const name: String);
var
  classIdString: String;
  key: String;
begin
  classIdString := GUIDToString(classId);

  // unregister extension for all file types.
  key := '*\shellex\ContextMenuHandlers\' + name;
  DeleteRegistryKey(HKEY_CLASSES_ROOT, key);

  // unregister extension for folders
  key := 'Directory\shellex\ContextMenuHandlers\' + name;
  DeleteRegistryKey(HKEY_CLASSES_ROOT, key);

  // remove approved shell extension
  key := 'SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved';
  DeleteRegistryValue(HKEY_LOCAL_MACHINE, key, classIdString);
end;

class procedure TContextMenuRegistrar.CreateRegistryValue(const rootKey: HKEY; const key, valueName, value: String);
var
  registry: TRegistry;
begin
  registry := TRegistry.Create;
  try
    registry.RootKey := rootKey;
    if registry.OpenKey(key, True) then
    begin
      registry.WriteString(valueName, value);
      registry.CloseKey;
    end;
  finally
    registry.Free;
  end;
end;

class procedure TContextMenuRegistrar.DeleteRegistryKey(const rootKey: HKEY; const key: String);
var
  registry: TRegistry;
begin
  registry := TRegistry.Create;
  try
    registry.RootKey := rootKey;
    registry.DeleteKey(key);
  finally
    registry.Free;
  end;
end;

class procedure TContextMenuRegistrar.DeleteRegistryValue(const rootKey: HKEY; const key, valueName: String);
var
  registry: TRegistry;
begin
  registry := TRegistry.Create;
  try
    registry.RootKey := rootKey;
    if registry.OpenKey(key, False) then
    begin
      registry.DeleteValue(valueName);
      registry.CloseKey;
    end;
  finally
    registry.Free;
  end;
end;

end.


