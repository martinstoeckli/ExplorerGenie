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

// $Rev: 98336 $
// Datei am 30.10.2022 21:59:35 erzeugt aus der unten beschriebenen Typbibliothek.

// ************************************************************************  //
// Typbib.: D:\Source\ExplorerGenie\src\ExplorerGenieExt\ExplorerGenieExt (1)
// LIBID: {551C9002-4521-471A-9542-F3D4CA752011}
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

  LIBID_ExplorerGenieExt: TGUID = '{551C9002-4521-471A-9542-F3D4CA752011}';

  IID_IApp: TGUID = '{5F10B977-DBAE-4692-8652-0D8A89F85EFE}';
  CLASS_App: TGUID = '{5C6755B8-615C-4212-88F3-4E254CAE46FA}';
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
// GUID:      {5F10B977-DBAE-4692-8652-0D8A89F85EFE}
// *********************************************************************//
  IApp = interface(IDispatch)
    ['{5F10B977-DBAE-4692-8652-0D8A89F85EFE}']
  end;

// *********************************************************************//
// DispIntf:  IAppDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {5F10B977-DBAE-4692-8652-0D8A89F85EFE}
// *********************************************************************//
  IAppDisp = dispinterface
    ['{5F10B977-DBAE-4692-8652-0D8A89F85EFE}']
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

