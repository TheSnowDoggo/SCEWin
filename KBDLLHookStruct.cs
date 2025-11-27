using System.Runtime.InteropServices;

namespace SCEWin;

[StructLayout(LayoutKind.Sequential)]
public readonly struct KBDLLHookStruct
{
    public enum Flag : uint
    {
        Extended = 0x01,
        Injected = 0x10,
        Altdown  = 0x20,
        Up       = 0x80,
    }

    public readonly uint  VkCode;
    public readonly uint  ScanCode;
    public readonly Flag  Flags;
    public readonly uint  Time;
    public readonly ulong DwExtraInfo;

    public static KBDLLHookStruct Marshall(IntPtr lParam)
    {
        return Marshal.PtrToStructure<KBDLLHookStruct>(lParam);
    }
}
