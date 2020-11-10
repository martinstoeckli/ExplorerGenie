unit UnitMenuModelIcon;

interface
uses
  Windows,
  ComObj,
  WinCodec;

type

  /// <summary>
  /// Part of a TMenuModel which points to a resource icon and can convert it to a bitmap handle,
  /// which can be shown in the context menu.
  /// </summary>
  TMenuIcon = class(TObject)
  private
    FIconResourceId: String;
    FDestSize: TSize;
    FBitmapHandle: HBITMAP;
    FTriedToCreateBitmapHandle: Boolean;
    function GetOrCreateBitmapHandle(): HBITMAP;

    /// <summary>
    /// Tries to load an icon resource with a given name, scales it to a given size and converts
    /// it to a bitmap which can be used as menu icon with alpha transparency.
    /// PBGRA = Multiplexed-Blue-Green-Red-Alpha
    /// </summary>
    /// <param name="iconResourceId">Id of an icon resource of the application.</param>
    /// <param name="destSize">The required size of the bitmap.</param>
    /// <returns>A menu icon bitmap, or 0 if no such resource could be found, or if it could not
    /// be converted to a bitmap.</returns>
    class function LoadIconAsPBGRA32Bitmap(iconResourceId: String; destSize: TSize): HBITMAP;

    /// <summary>
    /// Lazy loads a singleton imaging factory from Windows (Windows Imaging Component).
    /// </summary>
    /// <returns>The imaging factory or nil if the OS does not support it.</returns>
    class function GetOrCreateImagingFactory(): IWICImagingFactory;
  public
    /// <summary>
    /// Initializes a new instance of the TMenuIcon class.
    /// </summary>
    constructor Create(iconResourceId: String; destSize: TSize);

    /// <summary>
    /// Finalizes an instance of the TMenuIcon class.
    /// </summary>
    destructor Destroy(); override;

    /// <summary>
    /// Gets a handle to a bitmap, which can be used as menu icons with alpha transparency.
    /// The bitmap is lazy loaded and will be released automatically as soon as the destructor is
    /// called, so keep this object alive as long as the bitmap is needed.
    /// </summary>
    property BitmapHandle: HBITMAP read GetOrCreateBitmapHandle;
  end;

implementation
var
  _ImagingFactory: IWICImagingFactory;
  _TriedToCreateImagingFactory: Boolean;

{ TMenuIcon }

constructor TMenuIcon.Create(iconResourceId: String; destSize: TSize);
begin
  FIconResourceId := iconResourceId;
  FDestSize := destSize;
  FBitmapHandle := 0;
  FTriedToCreateBitmapHandle := false;
end;

destructor TMenuIcon.Destroy;
begin
  if (FBitmapHandle <> 0) then
    DeleteObject(FBitmapHandle);
  inherited Destroy;
end;

class function TMenuIcon.LoadIconAsPBGRA32Bitmap(iconResourceId: String; destSize: TSize): HBITMAP;
const
  BitsPerPixel = 32;
var
  iconHandle: HICON;
  imagingFactory: IWICImagingFactory;
  sourceBitmap: IWICBitmap;
  bitmapScaler: IWICBitmapScaler;
  bitmapConverter: IWICFormatConverter;
  bitmapInfo: TBitmapInfo;
  bitmapBuffer: Pointer;
  cbStride, cbBufferSize: UINT;
begin
  Result := 0;
  if (iconResourceId = '') then
    Exit;

  iconHandle := LoadImage(HInstance, PChar(iconResourceId), IMAGE_ICON, 0, 0, LR_DEFAULTCOLOR);
  if (iconHandle = 0) then
    exit;

  try
  imagingFactory := GetOrCreateImagingFactory();
  if ((imagingFactory = nil) or
      (Failed(imagingFactory.CreateBitmapFromHICON(iconHandle, sourceBitmap)))) or
      (Failed(imagingFactory.CreateBitmapScaler(bitmapScaler)) or
      (Failed(bitmapScaler.Initialize(sourceBitmap, destSize.Width, destSize.Height, WICBitmapInterpolationModeCubic))) or
      (Failed(imagingFactory.CreateFormatConverter(bitmapConverter))) or
      (Failed(bitmapConverter.Initialize(bitmapScaler, GUID_WICPixelFormat32bppPBGRA, WICBitmapDitherTypeNone, nil, 0.0, WICBitmapPaletteTypeCustom)))) then
    exit;

  ZeroMemory(@bitmapInfo, SizeOf(TBitmapInfo));
  bitmapInfo.bmiHeader.biSize := SizeOf(bitmapInfo.bmiHeader);
  bitmapInfo.bmiHeader.biPlanes := 1;
  bitmapInfo.bmiHeader.biCompression := BI_RGB;
  bitmapInfo.bmiHeader.biWidth := destSize.Width;
  bitmapInfo.bmiHeader.biHeight := -destSize.Height;
  bitmapInfo.bmiHeader.biBitCount := BitsPerPixel;

  Result := CreateDIBSection(0, bitmapInfo, DIB_RGB_COLORS, bitmapBuffer, 0, 0);
  if (Result = 0) then
    exit;

  cbStride := destSize.Width * (BitsPerPixel div 8);
  cbBufferSize := UINT(Abs(destSize.Height)) * cbStride;
  if Failed(bitmapConverter.CopyPixels(nil, cbStride, cbBufferSize, bitmapBuffer)) then
  begin
    DeleteObject(Result);
    Result := 0;
  end;
  finally
    DestroyIcon(iconHandle);
  end;
end;

class function TMenuIcon.GetOrCreateImagingFactory: IWICImagingFactory;
begin
  if (not _TriedToCreateImagingFactory) then
  begin
    _TriedToCreateImagingFactory := true;
    try
     // The imaging factory should be available since Windows Vista
    _ImagingFactory := CreateComObject(CLSID_WICImagingFactory) as IWICImagingFactory;
    except
      _ImagingFactory := nil;
    end;
  end;
  Result := _ImagingFactory;
end;

function TMenuIcon.GetOrCreateBitmapHandle: HBITMAP;
begin
  if (not FTriedToCreateBitmapHandle) then
  begin
    FTriedToCreateBitmapHandle := true;
    FBitmapHandle := LoadIconAsPBGRA32Bitmap(FIconResourceId, FDestSize);
  end;
  Result := FBitmapHandle;
end;

end.
