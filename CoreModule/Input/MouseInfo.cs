using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

/// <summary>
/// Tracks mouse input between update cycles.
/// </summary>
public class MouseInfo
{
    /// <summary>
    /// Gets the state of mouse input during the previous update cycle.
    /// </summary>
    public MouseState PreviousState { get; private set; }

    /// <summary>
    /// Gets the state of mouse input during the current update cycle.
    /// </summary>
    public MouseState CurrentState { get; private set; }

    /// <summary>
    /// Gets or sets the current mouse cursor position in screen space.
    /// </summary>
    public Point Position
    {
        get => CurrentState.Position;
        set => SetPosition(value.X, value.Y);
    }

    /// <summary>
    /// Gets or sets the current mouse cursor x-coordinate in screen space.
    /// </summary>
    public int X
    {
        get => CurrentState.X;
        set => SetPosition(value, CurrentState.Y);
    }

    /// <summary>
    /// Gets or sets the current mouse cursor y-coordinate in screen space.
    /// </summary>
    public int Y
    {
        get => CurrentState.Y;
        set => SetPosition(CurrentState.X, value);
    }

    /// <summary>
    /// Gets the difference in mouse cursor position between the previous and current frame.
    /// </summary>
    public Point PositionDelta => CurrentState.Position - PreviousState.Position;

    /// <summary>
    /// Gets the difference in mouse cursor x-position between the previous and current frame.
    /// </summary>
    public int XDelta => CurrentState.X - PreviousState.X;

    /// <summary>
    /// Gets the difference in mouse cursor y-position between the previous and current frame.
    /// </summary>
    public int YDelta => CurrentState.Y - PreviousState.Y;

    /// <summary>
    /// Gets whether the mouse cursor moved between the previous and current frames.
    /// </summary>
    public bool WasMoved => PositionDelta != Point.Zero;

    /// <summary>
    /// Gets the cumulative scroll wheel value since the start of the game.
    /// </summary>
    public int ScrollWheel => CurrentState.ScrollWheelValue;

    /// <summary>
    /// Gets the change in scroll wheel value between the previous and current frame.
    /// </summary>
    public int ScrollWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

    /// <summary>
    /// Creates a new <see cref="MouseInfo"/>.
    /// </summary>
    public MouseInfo()
    {
        PreviousState = new MouseState();
        CurrentState = Mouse.GetState();
    }

    /// <summary>
    /// Updates the state information about mouse input.
    /// </summary>
    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    /// <summary>
    /// Returns whether the specified mouse button is currently down.
    /// </summary>
    public bool IsButtonDown(MouseButton button)
    {
        return GetButtonState(CurrentState, button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Returns whether the specified mouse button is currently up.
    /// </summary>
    public bool IsButtonUp(MouseButton button)
    {
        return GetButtonState(CurrentState, button) == ButtonState.Released;
    }

    /// <summary>
    /// Returns whether the specified mouse button was just pressed on the current frame.
    /// </summary>
    public bool WasButtonJustPressed(MouseButton button)
    {
        return GetButtonState(CurrentState, button) == ButtonState.Pressed
            && GetButtonState(PreviousState, button) == ButtonState.Released;
    }

    /// <summary>
    /// Returns whether the specified mouse button was just released on the current frame.
    /// </summary>
    public bool WasButtonJustReleased(MouseButton button)
    {
        return GetButtonState(CurrentState, button) == ButtonState.Released
            && GetButtonState(PreviousState, button) == ButtonState.Pressed;
    }

    /// <summary>
    /// Sets the cursor position in screen space and updates the current mouse state.
    /// </summary>
    public void SetPosition(int x, int y)
    {
        Mouse.SetPosition(x, y);
        CurrentState = new MouseState(
            x,
            y,
            CurrentState.ScrollWheelValue,
            CurrentState.LeftButton,
            CurrentState.MiddleButton,
            CurrentState.RightButton,
            CurrentState.XButton1,
            CurrentState.XButton2
        );
    }

    private static ButtonState? GetButtonState(MouseState state, MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => state.LeftButton,
            MouseButton.Middle => state.MiddleButton,
            MouseButton.Right => state.RightButton,
            MouseButton.XButton1 => state.XButton1,
            MouseButton.XButton2 => state.XButton2,
            _ => null
        };
    }
}
