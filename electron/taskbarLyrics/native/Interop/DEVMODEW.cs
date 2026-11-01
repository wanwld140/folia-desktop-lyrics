// DEVMODEW supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct DEVMODEW
{
	public unsafe fixed char dmDeviceName[32];

	public ushort dmSpecVersion;

	public ushort dmDriverVersion;

	public ushort dmSize;

	public ushort dmDriverExtra;

	public uint dmFields;

	public int dmPositionX;

	public int dmPositionY;

	public uint dmDisplayOrientation;

	public uint dmDisplayFixedOutput;

	public short dmColor;

	public short dmDuplex;

	public short dmYResolution;

	public short dmTTOption;

	public short dmCollate;

	public unsafe fixed char dmFormName[32];

	public ushort dmLogPixels;

	public uint dmBitsPerPel;

	public uint dmPelsWidth;

	public uint dmPelsHeight;

	public uint dmDisplayFlags;

	public uint dmDisplayFrequency;

	public uint dmICMMethod;

	public uint dmICMIntent;

	public uint dmMediaType;

	public uint dmDitherType;

	public uint dmReserved1;

	public uint dmReserved2;

	public uint dmPanningWidth;

	public uint dmPanningHeight;
}
