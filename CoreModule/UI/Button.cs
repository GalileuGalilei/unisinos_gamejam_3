using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace MonoGameLibrary.UI;

/// <summary>
/// A clickable, text-based rectangular button.
/// </summary>
public sealed class Button : IDisposable
{
    private readonly Texture2D _pixel;
    private readonly Font _font;
    private readonly Action _onClick;

    /// <summary>
    /// Gets the text displayed by the button.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the button bounds in screen coordinates.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    /// Gets whether the mouse is currently over the button.
    /// </summary>
    public bool IsHovered { get; private set; }

    /// <summary>
    /// Creates a button that invokes the specified action when clicked.
    /// </summary>
    public Button(
        GraphicsDevice graphicsDevice,
        Font font,
        string text,
        Rectangle bounds,
        Action onClick)
    {
        ArgumentNullException.ThrowIfNull(graphicsDevice);
        _font = font ?? throw new ArgumentNullException(nameof(font));
        Text = text ?? throw new ArgumentNullException(nameof(text));
        _onClick = onClick ?? throw new ArgumentNullException(nameof(onClick));

        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bounds), "Button bounds must have a positive width and height.");
        }

        Bounds = bounds;
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    /// <summary>
    /// Updates the button hover state and handles mouse clicks.
    /// </summary>
    public void Update(MouseInfo mouse)
    {
        ArgumentNullException.ThrowIfNull(mouse);

        IsHovered = Bounds.Contains(mouse.Position);
        if (IsHovered && mouse.WasButtonJustPressed(MouseButton.Left))
        {
            Activate();
        }
    }

    /// <summary>
    /// Invokes the button action, for example from a keyboard shortcut.
    /// </summary>
    public void Activate() => _onClick();

    /// <summary>
    /// Draws the button and its centered label.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);

        Color backgroundColor = IsHovered
            ? new Color(104, 156, 224)
            : new Color(61, 91, 153);

        spriteBatch.Draw(_pixel, Bounds, backgroundColor);

        Vector2 textSize = _font.MeasureString(Text);
        Vector2 textPosition = new(
            Bounds.X + (Bounds.Width - textSize.X) * 0.5f,
            Bounds.Y + (Bounds.Height - textSize.Y) * 0.5f
        );
        _font.Draw(spriteBatch, Text, textPosition, Color.White);
    }

    /// <inheritdoc />
    public void Dispose() => _pixel.Dispose();
}
