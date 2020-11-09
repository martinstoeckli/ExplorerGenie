unit ExplorerGenieExt_TLB;

// ************************************************************************ //
// WARNUNG
// -------
// Die in dieser Datei deklarierten Typen wurden aus Daten einer Typbibliothek
// generiert. Wenn diese Typbibliothek explizit oder indirekt (über eine
// andere Typbibliothek) reimportiert wird oder wenn der Befehl
// 'Aktualisieren' im Typbibliotheks-Editor während des Bearbeitens der
// Typbibliothek aktiviert ist, wird der Inhalt dieser Datei neu generiert und
// alle manuell vorgenommenen Änderungen gehen verloren.
// ************************************************************************ //

// $Rev: 52393 $
// Datei am 09.11.2020 09:31:29 erzeugt aus der unten beschriebenen Typbibliothek.

// ************************************************************************  //
// Typbib.: D:\Source\git\ExplorerGenie\GitHub\src\ExplorerGenieExt\ExplorerGenieExt (1)
// LIBID: {A16065CA-D03A-4F74-B1BB-EC91C4C4E9D4}
// LCID: 0
// Hilfedatei:
// Hilfe-String:
// Liste der Abhäng.:
//   (1) v2.0 stdole, (C:\Windows\SysWOW64\stdole2.tlb)
// SYS_KIND: SYS_WIN32
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit muss ohne Typüberprüfung für Zeiger compiliert werden.
{$WARN SYMBOL_PLATFORM OFF}
{$WRITEABLECONST ON}
{$VARPROPSETTER ON}
{$ALIGN 4}

interface

uses Winapi.Windows, System.Classes, System.Variants, System.Win.StdVCL, Vcl.Graphics, Vcl.OleServer, Winapi.ActiveX;

// *********************************************************************//
// In der Typbibliothek deklarierte GUIDS. Die folgenden Präfixe werden verwendet:
//   Typbibliotheken      : LIBID_xxxx
//   CoClasses            : CLASS_xxxx
//   DISPInterfaces       : DIID_xxxx
//   Nicht-DISP-Interfaces: IID_xxxx
// *********************************************************************//
const
  // Haupt- und Nebenversionen der Typbibliothek
  ExplorerGenieExtMajorVersion = 1;
  ExplorerGenieExtMinorVersion = 0;

  LIBID_ExplorerGenieExt: TGUID = '{A16065CA-D03A-4F74-B1BB-EC91C4C4E9D4}';

  IID_IApp: TGUID = '{264D3051-7949-4D69-AD88-C8B04D4B4254}';
  CLASS_App: TGUID = '{D7C1499F-25C0-4C6E-87F5-1C69EEBA6398}';
type

// *********************************************************************//
// Forward-Deklaration von in der Typbibliothek definierten Typen
// *********************************************************************//
  IApp = interface;
  IAppDisp = dispinterface;

// *********************************************************************//
// Deklaration von in der Typbibliothek definierten CoClasses
// (HINWEIS: Hier wird jede CoClass ihrem Standard-Interface zugewiesen)
// *********************************************************************//
  App = IApp;


// *********************************************************************//
// Interface: IApp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {264D3051-7949-4D69-AD88-C8B04D4B4254}
// *********************************************************************//
  IApp = interface(IDispatch)
    ['{264D3051-7949-4D69-AD88-C8B04D4B4254}']
  end;

// *********************************************************************//
// DispIntf:  IAppDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {264D3051-7949-4D69-AD88-C8B04D4B4254}
// *********************************************************************//
  IAppDisp = dispinterface
    ['{264D3051-7949-4D69-AD88-C8B04D4B4254}']
  end;

// *********************************************************************//
// Die Klasse CoApp stellt die Methoden Create und CreateRemote zur
// Verfügung, um Instanzen des Standard-Interface IApp, dargestellt
// von CoClass App, zu erzeugen. Diese Funktionen können
// von einem Client verwendet werden, der die CoClasses automatisieren
// will, die von dieser Typbibliothek dargestellt werden.
// *********************************************************************//
  CoApp = class
    class function Create: IApp;
    class function CreateRemote(const MachineName: string): IApp;
  end;

implementation

uses System.Win.ComObj;

class function CoApp.Create: IApp;
begin
  Result := CreateComObject(CLASS_App) as IApp;
end;

class function CoApp.CreateRemote(const MachineName: string): IApp;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_App) as IApp;
end;

end.

