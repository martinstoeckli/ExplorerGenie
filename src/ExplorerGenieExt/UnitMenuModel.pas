// Copyright © 2020 Martin Stoeckli.
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

unit UnitMenuModel;

interface
uses
  Classes,
  Generics.Collections,
  System.SysUtils,
  Windows,
  UnitMenuModelIcon;

type
  TMenuModelList = class;

  /// <summary>
  /// The model class which can describe a menu tree structure.
  /// </summary>
  TMenuModel = class(TObject)
  private
    FChildren: TMenuModelList;
    FTitle: String;
    FIconResourceId: Integer;
    FIsSeparator: Boolean;
    FOnClicked: TProc<TMenuModel, TStrings>;
    FContext: TObject;
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
    /// Gets or sets the title of the menu item.
    /// </summary>
    property Title: String read FTitle write FTitle;

    /// <summary>
    /// Gets or sets the id of a icon resource. A value of 0 indicates that no icon is associated.
    /// </summary>
    property IconResourceId: Integer read FIconResourceId write FIconResourceId;

    /// <summary>
    /// Gets or sets an a value indicating whether the menu represents a separator line.
    /// </summary>
    property IsSeparator: Boolean read FIsSeparator write FIsSeparator;

    /// <summary>
    /// Gets or sets a delgate which should be executed when the user clicked the menu item.
    /// </summary>
    property OnClicked: TProc<TMenuModel, TStrings> read FOnClicked write FOnClicked;

    /// <summary>
    /// Gets a lazy created list of sub menu items.
    /// </summary>
    property Children: TMenuModelList read GetOrCreateChildren;

    /// <summary>
    /// Gets a value indicating whether there are sub menu items.
    /// </summary>
    property HasChildren: Boolean read GetHasChildren;

    /// <summary>
    /// Gets or sets an optional reference to a context object.
    /// </summary>
    property Context: TObject read FContext write FContext;
  end;

  /// <summary>
  /// A list of menu items.
  /// </summary>
  TMenuModelList = class(TObjectList<TMenuModel>)
  public
    /// <summary>
    /// Shortcut for checking whether there are at least one item in the list.
    /// </summary>
    /// <returns>Returns true if the list contains at least one item, otherwise false.</returns>
    function Any(): Boolean;
  end;

implementation

{ TMenuModel }

constructor TMenuModel.Create;
begin
  FChildren := nil;
  FContext := nil;
  FIconResourceId := 0;
end;

destructor TMenuModel.Destroy;
begin
  FChildren.Free;
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
  Result := (FChildren <> nil) and (FChildren.Any());
end;

{ TMenuModelList }

function TMenuModelList.Any: Boolean;
begin
  Result := Count > 0;
end;

end.
