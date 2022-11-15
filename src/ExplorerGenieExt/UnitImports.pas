// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitImports;

interface
uses
  ActiveX,
  ShlObj,
  Windows;

type
  IEnumExplorerCommand = interface;

  /// <summary>
  /// Overwrites the ShlObj.IExplorerCommand interface, to get a more correct definition.
  /// </summary>
  IExplorerCommand = interface(IUnknown)
    ['{a08ce4d0-fa25-44ab-b57c-c7b1c323e0b9}']
    function GetTitle(psiItemArray: IShellItemArray; out ppszName: LPWSTR): HResult; stdcall;
    function GetIcon(psiItemArray: IShellItemArray; out ppszIcon: LPWSTR): HResult; stdcall;
    function GetToolTip(psiItemArray: IShellItemArray; out ppszInfotip: LPWSTR): HResult; stdcall;
    function GetCanonicalName(out pguidCommandName: TGUID): HResult; stdcall;
    function GetState(psiItemArray: IShellItemArray; fOkToBeSlow: boolean; out pCmdState: TEXPCMDSTATE): HResult; stdcall;
    function Invoke(psiItemArray: IShellItemArray; pbc: IBindCtx): HResult; stdcall;
    function GetFlags(out pFlags: TEXPCMDFLAGS): HResult; stdcall;
    function EnumSubCommands(out ppEnum: IEnumExplorerCommand): HResult; stdcall;
  end;
  PIExplorerCommand = ^IExplorerCommand;

  /// <summary>
  /// Overwrites the ShlObj.IEnumExplorerCommand interface, to get a more correct definition.
  /// </summary>
  IEnumExplorerCommand = interface(IUnknown)
    ['{a88826f8-186f-4987-aade-ea0cef8fbfe8}']
    function Next(celt: ULONG; out pUICommand: PIExplorerCommand; out pceltFetched: ULONG): HResult; stdcall;
    function Skip(celt: ULONG): HResult; stdcall;
    function Reset(): HResult; stdcall;
    function Clone(out ppenum: IEnumExplorerCommand): HResult; stdcall;
  end;

implementation

end.
