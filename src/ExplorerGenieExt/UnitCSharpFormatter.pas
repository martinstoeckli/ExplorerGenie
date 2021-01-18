unit UnitCSharpFormatter;

interface
uses
  System.RegularExpressions,
  System.SysUtils;

type
  /// <summary>
  /// Format strings with {n} placeholders like in CSharp.
  /// </summary>
  TCSharpFormatter = class(TObject)
  private
    FSettings: TFormatSettings;
  public
    /// <summary>
    /// Initializes a new instance of the TStringFormatter class.
    /// The localization settings of the current thread will be used.
    /// </summary>
    constructor Create(); overload;

    /// <summary>
    /// Initializes a new instance of the TStringFormatter class.
    /// </summary>
    /// <param name="settings">Localization settings to use.</param>
    constructor Create(const settings: TFormatSettings); overload;

    /// <summary>
    /// Formats a list of arguments into a string.
    /// </summary>
    /// <param name="format">A format string.</param>
    /// <param name="args">The object(s) to format.</param>
    /// <returns>A copy of format in which any tags are replaced by the formatted argument(s).</returns>
    function Format(format: String; const args: array of const): String; overload;

    /// <summary>
    /// Formats a list of arguments into a string.
    /// </summary>
    /// <param name="format">A format string.</param>
    /// <param name="args">The object(s) to format.</param>
    /// <param name="settings">Localization settings to use.</param>
    /// <returns>A copy of format in which any tags are replaced by the formatted argument(s).</returns>
    class function Format(format: String; const args: array of const; settings: TFormatSettings): String; overload;
  end;

implementation

{ TStringFormatter }

constructor TCSharpFormatter.Create;
begin
  FSettings := TFormatSettings.Create();
end;

constructor TCSharpFormatter.Create(const settings: TFormatSettings);
begin
  FSettings := settings;
end;

function TCSharpFormatter.Format(format: String; const args: array of const): String;
begin
  Result := TCSharpFormatter.Format(format, args, FSettings);
end;

class function TCSharpFormatter.Format(format: String; const args: array of const; settings: TFormatSettings): String;
var
  delphiFormat: TStringBuilder;
  regex: TRegEx;
  matches: TMatchCollection;
  match: TMatch;
  index: Integer;
  zeroBasedPos: Integer;
  tag: string;
begin
  delphiFormat := TStringBuilder.Create(format);
  try
    regex := TRegEx.Create('\{[0-9]*\}');
    matches := regex.Matches(format);
    for index := matches.Count - 1 downto 0 do
    begin
      match := matches[index];
      if (match.Success) then
      begin
        tag := match.Value;
        zeroBasedPos := match.Index - 1;
        delphiFormat.Remove(zeroBasedPos, match.Length);
        delphiFormat.Insert(zeroBasedPos, '?');
      end;
    end;

    Result := System.SysUtils.Format(delphiFormat.ToString(), args, settings);
  finally
    delphiFormat.Free;
  end;
end;

end.
