using SCENeo;

namespace SCEWin;

public static class Input
{
    /// <summary>
    /// Determines whether the given key is pressed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns><see langword="true"/> if the key is pressed down; otherwise, <see langword="false"/></returns>
    public static bool KeyPressed(Key key)
    {
        return Convert.ToBoolean(WinApi.GetKeyState((int)key) & 0x8000);
    }

    /// <summary>
    /// Returns the raw WASD move vector.
    /// </summary>
    /// <returns>The raw WASD move vector.</returns>
    public static Vec2I RawWASDMoveVector()
    {
        Vec2I move = Vec2I.Zero;

        if (KeyPressed(Key.W))
        {
            move += Vec2I.Up;
        }

        if (KeyPressed(Key.S))
        {
            move += Vec2I.Down;
        }

        if (KeyPressed(Key.A))
        {
            move += Vec2I.Left;
        }

        if (KeyPressed(Key.D))
        {
            move += Vec2I.Right;
        }

        return move;
    }

    /// <summary>
    /// Returns whether letters should be capitalized.
    /// </summary>
    /// <returns><see langword="true"/> if shift is pressed or caps lock is enabled; otherwise, <see langword="false"/></returns>
    public static bool Capitalize()
    {
        return KeyPressed(Key.Shift) || Console.CapsLock;
    }
}