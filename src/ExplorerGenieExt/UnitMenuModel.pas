// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitMenuModel;

interface
uses
  Classes,
  Generics.Collections,
  SysUtils,
  UnitMenuModelIcon;

type
  TExplorerCommandFilter = (ecfAll, ecfDiretoryOnly);

  /// <summary>
  /// The model class which can describe a menu tree structure. Because they can live longer than
  /// the application itself (IExplorerCommand) they should be used reference counted by interface.
  /// </summary>
  /// <remarks>
  /// This model can outlive the application object and therefore should self contain all
  /// information required by the Windows IExplorerCommand API. Store and keep properties in
  /// WideString instead of string.
  /// </remarks>
  IMenuModel = interface
    ['{923968D3-DFEF-47FE-A688-2E3D3DE1B728}']
    function GetTitle(): WideString;
    procedure SetTitle(value: WideString);
    function GetIconResourcePath(): WideString;
    procedure SetIconResourcePath(value: WideString);
    function GetIsSeparator(): Boolean;
    procedure SetIsSeparator(value: Boolean);
    function GetFilter(): TExplorerCommandFilter;
    procedure SetFilter(value: TExplorerCommandFilter);
    function GetOnClicked(): TProc<IMenuModel, TStrings>;
    procedure SetOnClicked(value: TProc<IMenuModel, TStrings>);
    function GetChildrenCount: Integer;
    function GetChild(index: Integer): IMenuModel;

    /// <summary>
    /// Gets or sets the title of the menu item.
    /// </summary>
    property Title: WideString read GetTitle write SetTitle;

    /// <summary>
    /// Gets or sets the path of an icon resource. An empty string indicates that no icon is
    /// associated. The path looks like...
    ///   <ResourceDllPath>,-<IconId>
    ///   C:\Programs\MyApp.dll,-101
    /// ...where only numeric ids seem to be accepted.
    /// </summary>
    property IconResourcePath: WideString read GetIconResourcePath write SetIconResourcePath;

    /// <summary>
    /// Gets or sets an a value indicating whether the menu represents a separator line.
    /// </summary>
    property IsSeparator: Boolean read GetIsSeparator write SetIsSeparator;

    /// <summary>
    /// Gets or sets an a filter which decides whether the menu is visible depending on the user selection.
    /// </summary>
    property Filter: TExplorerCommandFilter read GetFilter write SetFilter;

    /// <summary>
    /// Gets or sets a delgate which should be executed when the user clicked the menu item.
    /// </summary>
    property OnClicked: TProc<IMenuModel, TStrings> read GetOnClicked write SetOnClicked;

    /// <summary>
    /// Gets the number of children.
    /// </summary>
    property ChildrenCount: Integer read GetChildrenCount;

    /// <summary>
    /// Gets a child menu at the given position.
    /// </summary>
    property Children[index: Integer]: IMenuModel read GetChild;

    /// <summary>
    /// Adds a child menu to the Children list.
    /// </summary>
    procedure AddChild(const item: IMenuModel);

    /// <summary>
    /// Adds a collection of child menus to the Children list.
    /// </summary>
    procedure AddChildren(const collection: TEnumerable<IMenuModel>);
  end;

  /// <summary>
  /// Implementation of the IMenuModel interface.
  /// Access instances of this class only via interface to get reference counting.
  /// </summary>
  TMenuModel = class(TInterfacedObject, IMenuModel)
  private
    FChildren: TList<IMenuModel>;
    FTitle: WideString;
    FIconResourcePath: WideString;
    FIsSeparator: Boolean;
    FFilter: TExplorerCommandFilter;
    FOnClicked: TProc<IMenuModel, TStrings>;
    function GetOrCreateChildren(): TList<IMenuModel>;
  protected
    // IMenuModel
    function GetTitle(): WideString;
    procedure SetTitle(value: WideString);
    function GetIconResourcePath(): WideString;
    procedure SetIconResourcePath(value: WideString);
    function GetIsSeparator(): Boolean;
    procedure SetIsSeparator(value: Boolean);
    function GetFilter(): TExplorerCommandFilter;
    procedure SetFilter(value: TExplorerCommandFilter);
    function GetOnClicked(): TProc<IMenuModel, TStrings>;
    procedure SetOnClicked(value: TProc<IMenuModel, TStrings>);
    function GetChildrenCount: Integer;
    function GetChild(index: Integer): IMenuModel;
    procedure AddChild(const item: IMenuModel);
    procedure AddChildren(const collection: TEnumerable<IMenuModel>);
  public
    /// <summary>
    /// Initializes a new instance of the TMenuModel class.
    /// </summary>
    constructor Create;

    /// <summary>
    /// Finalizes an instance of the TMenuModel class.
    /// </summary>
    destructor Destroy; override;
  end;

implementation

{ TMenuModel }

constructor TMenuModel.Create;
begin
  FChildren := nil;
  FFilter := ecfAll;
end;

destructor TMenuModel.Destroy;
begin
  FChildren.Free;
  inherited Destroy;
end;

procedure TMenuModel.AddChild(const item: IMenuModel);
begin
  GetOrCreateChildren().Add(item);
end;

procedure TMenuModel.AddChildren(const collection: TEnumerable<IMenuModel>);
begin
  GetOrCreateChildren().AddRange(collection);
end;

function TMenuModel.GetChild(index: Integer): IMenuModel;
begin
  Result := GetOrCreateChildren()[index];
end;

function TMenuModel.GetChildrenCount: Integer;
begin
  Result := 0;
  if (FChildren <> nil) then
    Result := FChildren.Count;
end;

function TMenuModel.GetFilter: TExplorerCommandFilter;
begin
  Result := FFilter;
end;

function TMenuModel.GetIconResourcePath: WideString;
begin
  Result := FIconResourcePath;
end;

function TMenuModel.GetIsSeparator: Boolean;
begin
  Result := FIsSeparator;
end;

function TMenuModel.GetOnClicked: TProc<IMenuModel, TStrings>;
begin
  Result := FOnClicked;
end;

function TMenuModel.GetOrCreateChildren: TList<IMenuModel>;
begin
  if (FChildren = nil) then
    FChildren := TList<IMenuModel>.Create();
  Result := FChildren;
end;

function TMenuModel.GetTitle: WideString;
begin
  Result := FTitle;
end;

procedure TMenuModel.SetFilter(value: TExplorerCommandFilter);
begin
  FFilter := value;
end;

procedure TMenuModel.SetIconResourcePath(value: WideString);
begin
  FIconResourcePath := value;
end;

procedure TMenuModel.SetIsSeparator(value: Boolean);
begin
  FIsSeparator := value;
end;

procedure TMenuModel.SetOnClicked(value: TProc<IMenuModel, TStrings>);
begin
  FOnClicked := value;
end;

procedure TMenuModel.SetTitle(value: WideString);
begin
  FTitle := value;
end;

end.
