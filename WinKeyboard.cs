using System.Runtime.InteropServices;

namespace SCEWin;

public static class WinKeyboard
{
    private const int WHKeyboardLL = 13;

    private static IntPtr HookId = IntPtr.Zero;

    public static event Action<MessageType, KBDLLHookStruct>? OnInput;

    public static bool CheckFocus { get; set; } = false;

    public static void Start()
    {
        HookId = WinApi.SetWindowsHookExW(WHKeyboardLL, HookCallback, 0, 0);

        if (HookId == IntPtr.Zero)
        {
            throw new Exception("Failed to hook.");
        }

        var msg = new WinApi.TagMsg();

        while (WinApi.GetMessageW(out msg, IntPtr.Zero, 0, 0))
        {
            WinApi.TranslateMessage(ref msg);
            WinApi.DispatchMessage(ref msg);
        }

        WinApi.UnhookWindowsHookEx(HookId);
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode != 0 || (CheckFocus && !InFocus()))
        {
            return WinApi.CallNextHookEx(HookId, nCode, wParam, lParam); 
        }
        
        MessageType messageType = (MessageType)wParam;
        KBDLLHookStruct kbdll   = KBDLLHookStruct.Marshall(lParam);

        OnInput?.Invoke(messageType, kbdll);

        return WinApi.CallNextHookEx(HookId, nCode, wParam, lParam);
    }

    private static bool InFocus()
    {
        IntPtr hwnd = WinApi.GetForegroundWindow();

        uint result = WinApi.GetWindowThreadProcessId(hwnd, out uint lpdwProcessId);

        if (result == 0)
        {
            throw new Exception($"Failed to resolve process id: {Marshal.GetLastWin32Error()}.");
        }

        return Environment.ProcessId == lpdwProcessId;
    }
}