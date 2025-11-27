using SCENeo;

namespace SCEWin;

public static class Input
{
    public static bool KeyPressed(Key key)
    {
        return Convert.ToBoolean(WinApi.GetKeyState((int)key) & 0x8000);
    }
}