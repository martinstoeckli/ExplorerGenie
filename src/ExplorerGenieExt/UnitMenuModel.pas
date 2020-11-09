unit UnitMenuModel;

interface
uses
  Classes,
  Generics.Collections,
  Windows,
  UnitMenuModelIcon;

type
  TMenuModelList = class;
  TMenuClickedDelegate = reference to procedure();

  /// <summary>
  /// The model class which can describe a menu tree structure.
  /// </summary>
  TMenuModel = class(TObject)
  private
    FRelativeCmdId: UINT;
    FChildren: TMenuModelList;
    FTitle: String;
    FIcon: TMenuIcon;
    FOnClicked: TMenuClickedDelegate;
    function GetOrCreateChildren(): TMenuModelList;
    function GetHasChildren(): Boolean;
  public
    /// <summary>
    /// Initializes a new instance of the TMenuModel class.
    /// </summary>
    constructor Create;

    /// <summary>
    /// Finalizes an instance of the TMenuModel class.
    /// </summary>
    destructor Destroy; override;

    /// <summary>
    /// Gets or sets the relative command id of the menu item. This id must be unique over all menu
    /// items. The ids should start with a first id 0 and should be incremented for each visible
    /// menu item, even for groups with sub menu items.
    /// </summary>
    property RelativeCmdId: UINT read FRelativeCmdId write FRelativeCmdId;

    /// <summary>
    /// Gets or sets the title of the menu item.
    /// </summary>
    property Title: String read FTitle write FTitle;

    /// <summary>
    /// Gets or sets an object handling the menu icon. This icon is owned by the menu model and
    /// will be automatically released.
    /// </summary>
    property Icon: TMenuIcon read FIcon write FIcon;

    /// <summary>
    /// Gets or sets a delgate which should be executed when the user clicked the menu item.
    /// </summary>
    property OnClicked: TMenuClickedDelegate read FOnClicked write FOnClicked;

    /// <summary>
    /// Gets a lazy created list of sub menu items.
    /// </summary>
    property Children: TMenuModelList read GetOrCreateChildren;

    /// <summary>
    /// Gets a value indicating whether there are sub menu items.
    /// </summary>
    property HasChildren: Boolean read GetHasChildren;
  end;

  /// <summary>
  /// A list of menu items.
  /// </summary>
  TMenuModelList = class(TObjectList<TMenuModel>)
  public
    /// <summary>
    /// Tries to recursively find the menu item with the given id in the menu tree.
    /// </summary>
    /// <param name="relativeCmdId">The command id we are looking for.</param>
    /// <returns>Returns the found menu item, or nil if no such menu item could be found.</returns>
    function FindByRelativeCmdId(relativeCmdId: UINT): TMenuModel;
  end;

implementation

{ TMenuModel }

constructor TMenuModel.Create;
begin
  FChildren := nil;
  FIcon := nil;
end;

destructor TMenuModel.Destroy;
begin
  FChildren.Free;
  FIcon.Free;
  inherited Destroy;
end;

function TMenuModel.GetOrCreateChildren: TMenuModelList;
begin
  if (FChildren = nil) then
    FChildren := TMenuModelList.Create(True);
  Result := FChildren;
end;

function TMenuModel.GetHasChildren: Boolean;
begin
  Result := (FChildren <> nil) and (FChildren.Count > 0);
end;

{ TMenuModelList }

function TMenuModelList.FindByRelativeCmdId(relativeCmdId: UINT): TMenuModel;
var
  index: Integer;
  menuModel: TMenuModel;
begin
  Result := nil;
  index := 0;
  while (Result = nil) and (index < Count) do
  begin
    menuModel := Items[index];
    Inc(index);

    if (menuModel.RelativeCmdId = relativeCmdId) then
      Result := menuModel;

    if (Result = nil) and (menuModel.HasChildren) then
      Result := menuModel.Children.FindByRelativeCmdId(relativeCmdId);
  end;
end;

end.
