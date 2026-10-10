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
    float _newRotation;
    float _previousRotation;
    TextureRegion _textureRegion;
    Vector2 _center;
    Rectangle _bounds;

    Vector2 _previusMouseClickedPosition;


    public RadialButton(TextureRegion textureRegion, Rectangle bounds)
    {
        _textureRegion = textureRegion;
        _bounds = bounds;
        _previusMouseClickedPosition = Vector2.Zero;
        _center = new Vector2(_bounds.Width / 2, _bounds.Height / 2) ;//+ new Vector2(_bounds.X, _bounds.Y);
    }

    public void Update(MouseInfo mouse)
    {
        float distanceFromCenter = Vector2.Distance(mouse.Position.ToVector2(), new Vector2(_bounds.X + _center.X, _bounds.Y + _center.Y));
        if (!_bounds.Contains(mouse.Position) || distanceFromCenter < _bounds.Width / 4)
        {
            _previusMouseClickedPosition = Vector2.Zero;
            _previousRotation += _newRotation;
            _newRotation = 0;
            return;
        }

        if(mouse.IsButtonDown(MouseButton.Left) && _previusMouseClickedPosition == Vector2.Zero)
        {
            _previusMouseClickedPosition = mouse.Position.ToVector2();
        }
        
        if(_previusMouseClickedPosition != Vector2.Zero && !mouse.IsButtonDown(MouseButton.Left))
        {
            _previusMouseClickedPosition = Vector2.Zero;
            _previousRotation += _newRotation;
            _newRotation = 0;
            return;
        }

        if (!mouse.IsButtonDown(MouseButton.Left))
        {
            return;
        }

        Vector2 currentMousePosition = mouse.Position.ToVector2();
        Vector2 centerPosition = new Vector2(_bounds.X + _center.X, _bounds.Y + _center.Y);
        _newRotation = (float)Math.Atan2(currentMousePosition.Y - centerPosition.Y, currentMousePosition.X - centerPosition.X) -
                       (float)Math.Atan2(_previusMouseClickedPosition.Y - centerPosition.Y, _previusMouseClickedPosition.X - centerPosition.X);

        if (Math.Abs(_newRotation) > MathHelper.Pi - 0.5)
        {
            _previousRotation += _newRotation;
            _newRotation = 0;
            _previusMouseClickedPosition = currentMousePosition;
        }
    }

    public void Draw()
    {
        Core.SpriteBatch.Draw(
            _textureRegion.Texture,
            new Vector2(Core.GraphicsDevice.Viewport.Width / 2, Core.GraphicsDevice.Viewport.Height / 2),
            _bounds,
            Color.White,
            _newRotation + _previousRotation,
            _center,
            1f,
            SpriteEffects.None,
            0f
        );
    }
}