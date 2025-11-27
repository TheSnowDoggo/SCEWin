using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace SCEWin;

// Made with help from from:
// https://stackoverflow.com/questions/2754518/how-can-i-write-fast-colored-output-to-console

internal static partial class WinApi
{
    #region Enums

    internal enum AccessGeneric : uint
    {
        All     = 0x10000000,
        Execute = 0x20000000,
        Write   = 0x40000000,
        Read    = 0x80000000,
    }

    internal enum ShareMode : uint
    {
        None   = 0x00000000,
        Delete = 0x00000004,
        Read   = 0x00000001,
        Write  = 0x00000002,
    }

    internal enum CreateMode : uint
    {
        CreateAlways     = 2,
        CreateNew        = 1,
        OpenAlways       = 4,
        OpenExisting     = 3,
        TruncateExisting = 5,
    }

    internal enum FileFlag : uint
    {
        Archive   = 32,
        Encrypted = 16384,
        Hidden    = 2,
        Normal    = 128,
        Offline   = 4096,
        Readonly  = 1,
        System    = 4,
        Temporary = 256,
    }

    internal enum FileAttribute : uint
    {
        BackupSemantics  = 0x02000000,
        DeleteOnClose    = 0x04000000,
        NoBuffering      = 0x20000000,
        OpenNoRecall     = 0x00100000,
        OpenReparsePoint = 0x00200000,
        Overlapped       = 0x40000000,
        PosixSemantics   = 0x01000000,
        RandomAccess     = 0x10000000,
        SessionAware     = 0x00800000,
        SequentialScan   = 0x08000000,
        WriteThrough     = 0x80000000,
    }

    #endregion

    #region Structs

    [StructLayout(LayoutKind.Explicit)]
    public record struct CharUnion
    {
        [FieldOffset(0)] public ushort UnicodeChar;
        [FieldOffset(0)] public byte   ASCIIChar;
    }

    [StructLayout(LayoutKind.Explicit)]
    public record struct CharInfo
    {
        [FieldOffset(0)] public CharUnion Char;
        [FieldOffset(2)] public short Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public record struct Coord(short X, short Y)
    {
        public static Coord Zero { get; } = new(0, 0);
    }

    [StructLayout(LayoutKind.Sequential)]
    public record struct SmallRect(short Left, short Top, short Right, short Bottom);

    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct TagMsg
    {
        public readonly IntPtr   Hwnd;
        public readonly uint     Message;
        public readonly IntPtr   WParam;
        public readonly IntPtr   LParam;
        public readonly uint     Time;
        public readonly TagPoint Point;
        public readonly uint     LPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct TagPoint
    {
        public readonly int X;
        public readonly int Y;
    }

    #endregion

    internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    private const string Kernel32 = "kernel32";
    private const string User32   = "user32";

    #region Kernel32

    [LibraryImport(Kernel32, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial SafeFileHandle CreateFileW(
        string        lpFileName,
        AccessGeneric dwDesiredAccess,
        ShareMode     dwShareMode,
        IntPtr        securityAttributes,
        CreateMode    dwCreationDisposition,
        uint          dwFlagsAndAttributes,
        IntPtr        hTemplateFile);

    [LibraryImport(Kernel32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WriteConsoleOutputW(
        SafeFileHandle  hConsoleOutput,
        [In] CharInfo[] lpBuffer,
        Coord           dwBufferSize,
        Coord           dwBufferCoord,
        ref SmallRect   lpWriteRegion);

    #endregion

    #region User32

    [LibraryImport(User32, SetLastError = true)]
    internal static partial short GetKeyState(int nVirtKey);

    [LibraryImport(User32, SetLastError = true)]
    internal static partial uint GetLastError();

    [LibraryImport(User32, SetLastError = true)]
    internal static partial IntPtr SetWindowsHookExW(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnhookWindowsHookEx(IntPtr hHook);

    [LibraryImport(User32, SetLastError = true)]
    internal static partial IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

    [LibraryImport(User32, SetLastError = true)]
    internal static partial IntPtr GetForegroundWindow();

    [LibraryImport(User32, SetLastError = true)]
    internal static partial uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TranslateMessage(ref TagMsg lpMsg);

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DispatchMessage(ref TagMsg lpMsg);

    [LibraryImport(User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetMessageW(out TagMsg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    #endregion
}