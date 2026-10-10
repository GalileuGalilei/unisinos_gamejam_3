using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace MonoGameLibrary.UI;

/// <summary>
/// A rotationalButton that can be rotated with mouse input
/// </summary>
public class RadialButton
{
    private readonly TextureRegion _textureRegion;
    private readonly Rectangle _bounds;
    private float _rotation;
    private float _previousMouseAngle;
    private bool _isDragging;

    public float Rotation => _rotation;
    public float RotationDelta { get; private set; }

    public RadialButton(TextureRegion textureRegion, Rectangle bounds)
    {
        ArgumentNullException.ThrowIfNull(textureRegion);
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bounds), "Button bounds must have a positive width and height.");
        }

        _textureRegion = textureRegion;
        _bounds = bounds;
    }

    public void Update(MouseInfo mouse)
    {
        ArgumentNullException.ThrowIfNull(mouse);
        RotationDelta = 0f;

        Point viewportSize = Core.GraphicsDevice.Viewport.Bounds.Size;
        Point renderResolution = Core.RenderResolution;
        Point mousePosition = new(
            mouse.Position.X * renderResolution.X / viewportSize.X,
            mouse.Position.Y * renderResolution.Y / viewportSize.Y
        );
        Vector2 centerPosition = new(
            _bounds.X + _bounds.Width / 2f,
            _bounds.Y + _bounds.Height / 2f
        );

        if (!_isDragging)
        {
            if (mouse.WasButtonJustPressed(MouseButton.Left) && _bounds.Contains(mousePosition))
            {
                _isDragging = true;
                Vector2 offset = mousePosition.ToVector2() - centerPosition;
                _previousMouseAngle = (float)Math.Atan2(offset.Y, offset.X);
            }

            return;
        }

        if (!mouse.IsButtonDown(MouseButton.Left))
        {
            _isDragging = false;
            return;
        }

        Vector2 currentOffset = mousePosition.ToVector2() - centerPosition;
        float currentMouseAngle = (float)Math.Atan2(currentOffset.Y, currentOffset.X);
        RotationDelta = MathHelper.WrapAngle(currentMouseAngle - _previousMouseAngle);
        _rotation += RotationDelta;
        _previousMouseAngle = currentMouseAngle;
    }

    public void Draw()
    {
        Vector2 scale = new(
            (float)_bounds.Width / _textureRegion.Width,
            (float)_bounds.Height / _textureRegion.Height
        );
        Vector2 origin = new Vector2(_textureRegion.Width / 2f, _textureRegion.Height / 2f) + new Vector2(8f,-6f);
        Vector2 position = new(
            _bounds.X + _bounds.Width / 2f,
            _bounds.Y + _bounds.Height / 2f
        );

        _textureRegion.Draw(
            Core.SpriteBatch,
            position,
            Color.White,
            _rotation,
            origin,
            scale,
            SpriteEffects.None,
            0f
        );
    }
}