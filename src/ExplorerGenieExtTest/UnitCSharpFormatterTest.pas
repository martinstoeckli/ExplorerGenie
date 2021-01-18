unit UnitCSharpFormatterTest;

interface
uses
  Classes,
  DUnitX.TestFramework,
  System.Generics.Collections,
  System.SysUtils,
  UnitCSharpFormatter;

type

  [TestFixture]
  TStringFormatterTest = class(TObject)
  private
    FFormatter: TCSharpFormatter;
  public
    [Setup]
    procedure Setup;

    [TearDown]
    procedure TearDown;

    [Test]
    procedure Format_WorksWithNoPlaceholder();

    [Test]
    procedure Format_EmptyFormatReturnsEmpty();

    [Test]
    procedure Format_ReplacesCurlyBracketsWithArgs();
  end;

implementation


{ TLanguageServiceTest }

procedure TStringFormatterTest.Setup;
begin
  FFormatter := TCSharpFormatter.Create(TFormatSettings.Invariant);
end;

procedure TStringFormatterTest.TearDown;
begin
  FFormatter.Free;
end;

procedure TStringFormatterTest.Format_WorksWithNoPlaceholder;
var
  res: String;
begin
  res := FFormatter.Format('The brown fox jumps over the 8 lazy dogs.', []);
  Assert.AreEqual('The brown fox jumps over the 8 lazy dogs.', res);

  res := FFormatter.Format('The brown fox jumps over the 8 lazy dogs.', [888]);
  Assert.AreEqual('The brown fox jumps over the 8 lazy dogs.', res);
end;

procedure TStringFormatterTest.Format_EmptyFormatReturnsEmpty;
var
  res: String;
begin
  res := FFormatter.Format('', [888]);
  Assert.AreEqual('', res);
end;

procedure TStringFormatterTest.Format_ReplacesCurlyBracketsWithArgs;
var
  res: String;
begin
  res := FFormatter.Format('{0} brown {1} jumps over the lazy {2}', ['The', 'fox', 'dog']);
  Assert.AreEqual('? brown ? jumps over the lazy ?', res);
end;

initialization
  TDUnitX.RegisterTestFixture(TStringFormatterTest);
end.
