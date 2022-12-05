// Copyright © 2022 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitLogger;

interface
type
  /// <summary>
  /// A logger which can be used for debugging.
  /// </summary>
  /// <remarks>
  /// Since this shell extension is part of a Windows app, it is very hard to debug otherwise.
  /// </remarks>
  ILogger = interface
    ['{336971F1-2D62-4748-A67A-352AC69EB37F}']
    procedure Debug(const text: string);
  end;

  /// <summary>
  /// Creates an instance of a logger.
  /// The logger interface is reference counted and releases itself when loosing the scope.
  /// </summary>
  /// <param name="name">A name for the logger, which can be be recognized in the log file.</param>
  /// <param name="filePath">The full path (directory & filename) of the logfile to generate.</param>
  /// <returns>Returns a new logger instance.</returns>
  function CreateLogger(const name: string; const filePath: string): ILogger;

  /// <summary>
  /// Creates an instance of a dummy logger which does nothing.
  /// </summary>
  /// <returns>Returns a new logger instance which does nothing.</returns>
  function CreateLoggerDummy(): ILogger;

var
  /// <summary>
  /// Global logger instance, its creation/destruction is left to the application.
  /// </summary>
  Logger: ILogger;

implementation
uses
  Classes,
  IOUtils,
  SysUtils,
  Windows;

type
  TLogger = class(TInterfacedObject, ILogger)
  protected
    FName: String;
    FFilePath: String;
    function FormatOutputLine(const text: String): String;
    procedure WriteLine(const line: String);

    // ILogger
    procedure Debug(const text: string);
  public
    constructor Create(const name: string; const filePath: string);
  end;

  TLoggerDummy = class(TInterfacedObject, ILogger)
    procedure Debug(const text: string);
  end;

function CreateLogger(const name: string; const filePath: string): ILogger;
begin
  Result := TLogger.Create(name, filePath);
end;

function CreateLoggerDummy(): ILogger;
begin
  Result := TLoggerDummy.Create();
end;

{ TLogger }

constructor TLogger.Create(const name: string; const filePath: string);
begin
  FName := name;
  FFilePath := filePath;
end;

procedure TLogger.Debug(const text: string);
var
  outputLine: String;
begin
  outputLine := FormatOutputLine(text);
  WriteLine(outputLine);
end;

function TLogger.FormatOutputLine(const text: String): String;
const
  // logtime -> loggername -> text
  FMT_OUTPUTLINE = '%s'#9'%s'#9'%s';
var
  logTime: String;
begin
  logTime := FormatDateTime('mm.dd hh:nn:ss.zzz', Now);
  Result := Format(FMT_OUTPUTLINE, [logTime, FName, text]);
end;

procedure TLogger.WriteLine(const line: String);
const
  MAX_ATTEMPTS = 5;
var
  outputHandle: THandle;
  attempts: Integer;
  outputLine: AnsiString;
  dummy: DWORD;
begin
  try
    // try to open output file.
    attempts := 0;
    outputHandle := INVALID_HANDLE_VALUE;
    try
      TDirectory.CreateDirectory(TPath.GetDirectoryName(FFilePath));

      while (outputHandle = INVALID_HANDLE_VALUE) and (attempts < MAX_ATTEMPTS) do
      begin
        outputHandle := CreateFile(PChar(FFilePath), GENERIC_WRITE, 0, nil, OPEN_ALWAYS, 0, 0);
        if (outputHandle = INVALID_HANDLE_VALUE) then
        begin // file may be locked, try later
          Sleep(5);
          Inc(attempts);
        end;
      end;

      // write to output file
      if (outputHandle <> INVALID_HANDLE_VALUE) then
      begin
        SetFilePointer(outputHandle, 0, nil, FILE_END);
        outputLine := AnsiString(Line + #13#10);
        WriteFile(outputHandle, outputLine[1], Length(outputLine), dummy, nil);
      end
    finally
      if (outputHandle <> INVALID_HANDLE_VALUE) then
        CloseHandle(outputHandle);
    end;
  except // a logging error musn't interrupt the program flow
  end;
end;

{ TLoggerDummy }

procedure TLoggerDummy.Debug(const text: string);
begin
  // Do nothing
end;

end.
