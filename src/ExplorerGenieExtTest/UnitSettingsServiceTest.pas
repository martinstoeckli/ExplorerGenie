unit UnitSettingsServiceTest;

interface
uses
  Classes,
  DUnitX.TestFramework,
  System.Generics.Collections,
  UnitSettingsModel,
  UnitSettingsService;

type

  [TestFixture]
  TSettingsServiceTest = class(TObject)
  public
    [Test]
    procedure AddCustomGotoTools_CorrectlyLoadsXml();
  end;

implementation

{ TSettingsServiceTest }

procedure TSettingsServiceTest.AddCustomGotoTools_CorrectlyLoadsXml;
const
  JSON =
   '[{"MenuTitle":"Visual Studio Code","CommandLine":"\"C:\\Programs\\Microsoft VS Code\\Code.exe\" \"{D}\"","AsAdmin":false},' +
   '{"MenuTitle":"robocopy🖐7","CommandLine":"C:\\program files\\robocopy.exe {D}","AsAdmin":true},' +
   '{"MenuTitle":"Invalid","CommandLine":"  ","AsAdmin":true}]';
var
  settings: TSettingsModel;
begin
  settings := TSettingsModel.Create();
  TSettingsService.AddCustomGotoTools(settings, JSON);

  Assert.AreEqual(3, settings.GotoTools.Count);

  Assert.AreEqual('Visual Studio Code', settings.GotoTools[0].Title);
  Assert.AreEqual(0, settings.GotoTools[0].ToolIndex);
  Assert.IsTrue(settings.GotoTools[0].IsCustomTool);
  Assert.IsTrue(settings.GotoTools[0].Visible);

  Assert.AreEqual('robocopy🖐7', settings.GotoTools[1].Title);
  Assert.AreEqual(1, settings.GotoTools[1].ToolIndex);
  Assert.IsTrue(settings.GotoTools[1].IsCustomTool);
  Assert.IsTrue(settings.GotoTools[1].Visible);

  Assert.AreEqual('Invalid', settings.GotoTools[2].Title);
  Assert.AreEqual(2, settings.GotoTools[2].ToolIndex);
  Assert.IsTrue(settings.GotoTools[2].IsCustomTool);
  Assert.IsFalse(settings.GotoTools[2].Visible);
end;

initialization
  TDUnitX.RegisterTestFixture(TSettingsServiceTest);
end.
