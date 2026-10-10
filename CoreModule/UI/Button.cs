using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace MonoGameLibrary.UI;

/// <summary>
/// A clickable rectangular button with atlas-based visual states.
/// </summary>
public sealed class Button
{
    private readonly Font _font;
    private readonly Action _onClick;
    private readonly TextureRegion _defaultRegion;
    private readonly TextureRegion _hoveredRegion;
    private readonly TextureRegion _selectedRegion;

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
    /// Gets whether the mouse button is pressed while over the button.
    /// </summary>
    public bool IsSelected { get; private set; }

    /// <summary>
    /// Creates a button that invokes the specified action when clicked.
    /// </summary>
    public Button(
        Font font,
        string text,
        Rectangle bounds,
        TextureAtlas atlas,
        string spriteName,
        Action onClick)
    {
        _font = font ?? throw new ArgumentNullException(nameof(font));
        Text = text ?? throw new ArgumentNullException(nameof(text));
        ArgumentNullException.ThrowIfNull(atlas);
        ArgumentException.ThrowIfNullOrWhiteSpace(spriteName);
        _onClick = onClick ?? throw new ArgumentNullException(nameof(onClick));

        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bounds), "Button bounds must have a positive width and height.");
        }

        Bounds = bounds;
        _defaultRegion = atlas.GetRegion(spriteName);
        _hoveredRegion = atlas.GetRegion($"{spriteName}_over");
        _selectedRegion = atlas.GetRegion($"{spriteName}_selecionado");
    }

    /// <summary>
    /// Updates the button hover state and handles mouse clicks.
    /// </summary>
    public void Update(MouseInfo mouse)
    {
        ArgumentNullException.ThrowIfNull(mouse);

        IsHovered = Bounds.Contains(mouse.Position);
        IsSelected = IsHovered && mouse.IsButtonDown(MouseButton.Left);
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

        TextureRegion region = IsSelected
            ? _selectedRegion
            : IsHovered
                ? _hoveredRegion
                : _defaultRegion;
        spriteBatch.Draw(region.Texture, Bounds, region.SourceRectangle, Color.White);

        Vector2 textSize = _font.MeasureString(Text);
        Vector2 textPosition = new(
            Bounds.X + (Bounds.Width - textSize.X) * 0.5f,
            Bounds.Y + (Bounds.Height - textSize.Y) * 0.5f
        );
        _font.Draw(spriteBatch, Text, textPosition, Color.White);
    }
}
