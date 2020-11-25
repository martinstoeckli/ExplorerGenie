unit UnitLanguageServiceTest;

interface
uses
  Classes,
  DUnitX.TestFramework,
  System.Generics.Collections,
  UnitLanguageService;

type

  [TestFixture]
  TLanguageServiceTest = class(TObject)
  private
    FService: ILanguageServiceForUnittest;
  public
    [Setup]
    procedure Setup;

    [Test]
    procedure ReadFromFile_ReadsNormalResources();

    [Test]
    procedure ReadFromFile_SkipsComments();

    [Test]
    procedure ReadFromFile_ReplacesNewLines();

    [Test]
    procedure ReadFromFile_IgnoresInvalidLines();
  end;

implementation


{ TLanguageServiceTest }

procedure TLanguageServiceTest.Setup;
begin
  FService := TLanguageServiceFactory.CreateLanguageService('test') as ILanguageServiceForUnittest;
end;

procedure TLanguageServiceTest.ReadFromFile_ReadsNormalResources;
var
  resFile: TStringList;
  resDictionary: TDictionary<string, string>;
begin
  resDictionary := TDictionary<string, string>.Create();
  resFile := TStringList.Create();
  try
  resFile.Add('guiClose Close');
  resFile.Add('guiInfo Information with space.');

  FService.ReadFromFile(resDictionary, resFile);
  Assert.AreEqual(2, resDictionary.Count);
  Assert.AreEqual('Close', resDictionary['guiClose']);
  Assert.AreEqual('Information with space.', resDictionary['guiInfo']);
  finally
    resFile.Free;
    resDictionary.Free;
  end;
end;

procedure TLanguageServiceTest.ReadFromFile_ReplacesNewLines;
var
  resFile: TStringList;
  resDictionary: TDictionary<string, string>;
begin
  resDictionary := TDictionary<string, string>.Create();
  resFile := TStringList.Create();
  try
  resFile.Add('guiClose Close\nBut on two lines.');

  FService.ReadFromFile(resDictionary, resFile);
  Assert.AreEqual('Close'#13#10'But on two lines.', resDictionary['guiClose']);
  finally
    resFile.Free;
    resDictionary.Free;
  end;
end;

procedure TLanguageServiceTest.ReadFromFile_SkipsComments;
var
  resFile: TStringList;
  resDictionary: TDictionary<string, string>;
begin
  resDictionary := TDictionary<string, string>.Create();
  resFile := TStringList.Create();
  try
  resFile.Add('// Just a comment');
  resFile.Add('guiClose  Close ');

  FService.ReadFromFile(resDictionary, resFile);
  Assert.AreEqual(1, resDictionary.Count);
  Assert.AreEqual('Close', resDictionary['guiClose']);
  finally
    resFile.Free;
    resDictionary.Free;
  end;
end;

procedure TLanguageServiceTest.ReadFromFile_IgnoresInvalidLines;
var
  resFile: TStringList;
  resDictionary: TDictionary<string, string>;
begin
  resDictionary := TDictionary<string, string>.Create();
  resFile := TStringList.Create();
  try
  resFile.Add('');
  resFile.Add('guiClose');
  resFile.Add('guiClose ');
  resFile.Add('guiClose  ');
  resFile.Add('888  Don''t ignore this valid line. ');

  FService.ReadFromFile(resDictionary, resFile);
  Assert.AreEqual(1, resDictionary.Count);
  Assert.AreEqual('Don''t ignore this valid line.', resDictionary['888']);
  finally
    resFile.Free;
    resDictionary.Free;
  end;
end;

initialization
  TDUnitX.RegisterTestFixture(TLanguageServiceTest);
end.
