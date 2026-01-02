using SCENeo;
using System.Runtime.InteropServices;
using System.Text;

namespace SCEWin;

/// <summary>
/// A struct containing information about a low-level keyboard event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct KBDLLHookStruct
{
    private const int KeyStateSize   = 256;
    private const int PwszBufferSize = 16;

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

    public Key Key { get { return (Key)VkCode; } }

    public static KBDLLHookStruct Marshall(IntPtr lParam)
    {
        return Marshal.PtrToStructure<KBDLLHookStruct>(lParam);
    }

    public string ToUnicode()
    {
        byte[] lpKeyState = new byte[KeyStateSize];

        if (!WinApi.GetKeyboardState(lpKeyState))
        {
            throw new Exception($"Failed to get keyboard state: {Marshal.GetLastWin32Error()}");
        }

        var pwszBuff = new StringBuilder(PwszBufferSize);

        int result = WinApi.ToUnicode(VkCode, ScanCode, lpKeyState, pwszBuff, PwszBufferSize, 0);

        if (result < 0)
        {
            return string.Empty;
        }

        return pwszBuff.ToString(0, result);
    }
}
