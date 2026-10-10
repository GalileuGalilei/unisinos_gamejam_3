using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

/// <summary>
/// Tracks input and vibration for a gamepad.
/// </summary>
public class GamePadInfo
{
    private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;

    /// <summary>
    /// Gets the player index this gamepad represents.
    /// </summary>
    public PlayerIndex PlayerIndex { get; }

    /// <summary>
    /// Gets the input state during the previous update cycle.
    /// </summary>
    public GamePadState PreviousState { get; private set; }

    /// <summary>
    /// Gets the input state during the current update cycle.
    /// </summary>
    public GamePadState CurrentState { get; private set; }

    /// <summary>
    /// Gets whether this gamepad is currently connected.
    /// </summary>
    public bool IsConnected => CurrentState.IsConnected;

    /// <summary>
    /// Gets the left thumbstick value.
    /// </summary>
    public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;

    /// <summary>
    /// Gets the right thumbstick value.
    /// </summary>
    public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;

    /// <summary>
    /// Gets the left trigger value.
    /// </summary>
    public float LeftTrigger => CurrentState.Triggers.Left;

    /// <summary>
    /// Gets the right trigger value.
    /// </summary>
    public float RightTrigger => CurrentState.Triggers.Right;

    /// <summary>
    /// Creates a new <see cref="GamePadInfo"/> for the specified player.
    /// </summary>
    public GamePadInfo(PlayerIndex playerIndex)
    {
        PlayerIndex = playerIndex;
        PreviousState = new GamePadState();
        CurrentState = GamePad.GetState(playerIndex);
    }

    /// <summary>
    /// Updates the input state and timed vibration for this gamepad.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex);

        if (_vibrationTimeRemaining > TimeSpan.Zero)
        {
            _vibrationTimeRemaining -= gameTime.ElapsedGameTime;

            if (_vibrationTimeRemaining <= TimeSpan.Zero)
            {
                StopVibration();
            }
        }
    }

    /// <summary>
    /// Returns whether the specified button is currently down.
    /// </summary>
    public bool IsButtonDown(Buttons button) => CurrentState.IsButtonDown(button);

    /// <summary>
    /// Returns whether the specified button is currently up.
    /// </summary>
    public bool IsButtonUp(Buttons button) => CurrentState.IsButtonUp(button);

    /// <summary>
    /// Returns whether the specified button was just pressed on the current frame.
    /// </summary>
    public bool WasButtonJustPressed(Buttons button)
    {
        return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
    }

    /// <summary>
    /// Returns whether the specified button was just released on the current frame.
    /// </summary>
    public bool WasButtonJustReleased(Buttons button)
    {
        return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
    }

    /// <summary>
    /// Sets the vibration strength for both motors for the specified duration.
    /// </summary>
    public void SetVibration(float strength, TimeSpan time)
    {
        _vibrationTimeRemaining = time;
        GamePad.SetVibration(PlayerIndex, strength, strength);
    }

    /// <summary>
    /// Stops vibration for both motors.
    /// </summary>
    public void StopVibration()
    {
        GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
    }
}
