using Microsoft.Win32.SafeHandles;
using SCENeo;
using SCENeo.Utils;

namespace SCEWin;

public sealed class WinOutput : IOutputSource
{
    private const string ConsoleFileName = "CONOUT$";

    private static readonly Lazy<WinOutput> _lazy = new(() => new());

    private readonly SafeFileHandle _handle;

    private WinOutput()
    {
        _handle = WinApi.CreateFileW(ConsoleFileName, WinApi.AccessGeneric.Write, WinApi.ShareMode.Write, IntPtr.Zero, WinApi.CreateMode.OpenAlways, 0, IntPtr.Zero);

        if (_handle.IsInvalid)
        {
            throw new Exception($"Failed to resolve File Handle during initialization: {WinApi.GetLastError()}");
        }
    }

    public static WinOutput Instance { get => _lazy.Value; }

    public void Update(IView<Pixel> grid)
    {
        WinApi.CharInfo[] buf = ToCharInfoBuffer(grid);

        var size = new WinApi.Coord()
        {
            X = (short)grid.Width,
            Y = (short)grid.Height,
        };

        var rect = new WinApi.SmallRect()
        {
            Left   = 0,
            Top    = 0,
            Right  = size.X,
            Bottom = size.Y,
        };

        WinApi.WriteConsoleOutputW(_handle, buf, size, WinApi.Coord.Zero, ref rect);
    }

    private static short ToAttributes(SCEColor fgColor, SCEColor bgColor)
    {
        return (short)(fgColor.ToConsoleColor() + ((int)bgColor.ToConsoleColor() << 4));
    }

    private static WinApi.CharInfo ToCharInfo(Pixel pixel)
    {
        var charInfo = new WinApi.CharInfo()
        {
            Char = new WinApi.CharUnion()
            {
                UnicodeChar = pixel.Element,
            },
            Attributes = ToAttributes(pixel.FgColor, pixel.BgColor),
        };
        return charInfo;
    }

    private static WinApi.CharInfo[] ToCharInfoBuffer(IView<Pixel> grid)
    {
        var buffer = new WinApi.CharInfo[grid.Size()];

        int i = 0;
        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                buffer[i++] = ToCharInfo(grid[x, y]);
            }
        }

        return buffer;
    }
}