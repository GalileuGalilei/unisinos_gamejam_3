using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

/// <summary>
/// Provides a small wrapper around a loaded sprite font.
/// </summary>
public sealed class Font
{
    /// <summary>
    /// Gets the underlying MonoGame sprite font.
    /// </summary>
    public SpriteFont SpriteFont { get; }

    /// <summary>
    /// Creates a font wrapper for a loaded sprite font.
    /// </summary>
    public Font(SpriteFont spriteFont)
    {
        SpriteFont = spriteFont ?? throw new ArgumentNullException(nameof(spriteFont));
    }

    /// <summary>
    /// Measures the specified text using this font.
    /// </summary>
    public Vector2 MeasureString(string text) => SpriteFont.MeasureString(text);

    /// <summary>
    /// Draws text using this font.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
    {
        spriteBatch.DrawString(SpriteFont, text, position, color);
    }
}
