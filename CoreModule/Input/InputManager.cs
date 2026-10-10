using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Input;

/// <summary>
/// Coordinates keyboard, mouse, and gamepad input state updates.
/// </summary>
public class InputManager
{
    /// <summary>
    /// Gets the state information for keyboard input.
    /// </summary>
    public KeyboardInfo Keyboard { get; private set; }

    /// <summary>
    /// Gets the state information for mouse input.
    /// </summary>
    public MouseInfo Mouse { get; private set; }

    /// <summary>
    /// Gets the state information for the four supported gamepads.
    /// </summary>
    public GamePadInfo[] GamePads { get; private set; }

    /// <summary>
    /// Creates a new <see cref="InputManager"/>.
    /// </summary>
    public InputManager()
    {
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();

        GamePads = new GamePadInfo[4];
        for (int i = 0; i < GamePads.Length; i++)
        {
            GamePads[i] = new GamePadInfo((PlayerIndex)i);
        }
    }

    /// <summary>
    /// Updates state information for all input devices.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();

        foreach (GamePadInfo gamePad in GamePads)
        {
            gamePad.Update(gameTime);
        }
    }
}
