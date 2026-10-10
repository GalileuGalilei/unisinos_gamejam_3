using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

/// <summary>
/// Tracks keyboard input between update cycles.
/// </summary>
public class KeyboardInfo
{
    /// <summary>
    /// Gets the state of keyboard input during the previous update cycle.
    /// </summary>
    public KeyboardState PreviousState { get; private set; }

    /// <summary>
    /// Gets the state of keyboard input during the current update cycle.
    /// </summary>
    public KeyboardState CurrentState { get; private set; }

    /// <summary>
    /// Creates a new <see cref="KeyboardInfo"/>.
    /// </summary>
    public KeyboardInfo()
    {
        PreviousState = new KeyboardState();
        CurrentState = Keyboard.GetState();
    }

    /// <summary>
    /// Updates the state information about keyboard input.
    /// </summary>
    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    /// <summary>
    /// Returns whether the specified key is currently down.
    /// </summary>
    public bool IsKeyDown(Keys key) => CurrentState.IsKeyDown(key);

    /// <summary>
    /// Returns whether the specified key is currently up.
    /// </summary>
    public bool IsKeyUp(Keys key) => CurrentState.IsKeyUp(key);

    /// <summary>
    /// Returns whether the specified key was just pressed on the current frame.
    /// </summary>
    public bool WasKeyJustPressed(Keys key)
    {
        return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
    }

    /// <summary>
    /// Returns whether the specified key was just released on the current frame.
    /// </summary>
    public bool WasKeyJustReleased(Keys key)
    {
        return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
    }
}
