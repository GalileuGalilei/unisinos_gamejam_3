using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.UI;

namespace game.Scenes;

public class TitleScene : Scene
{
    private Font _font;
    private Button _gameButton;

    public override void Initialize()
    {
        base.Initialize();
        Core.ExitOnEscape = true;
    }

    public override void LoadContent()
    {
        _font = new Font(Content.Load<SpriteFont>("fonts/game-font"));

        Rectangle viewport = Core.GraphicsDevice.Viewport.Bounds;
        const int buttonWidth = 360;
        const int buttonHeight = 100;
        Rectangle buttonBounds = new(
            (viewport.Width - buttonWidth) / 2,
            (viewport.Height - buttonHeight) / 2,
            buttonWidth,
            buttonHeight
        );

        _gameButton = new Button(
            Core.GraphicsDevice,
            _font,
            "game title",
            buttonBounds,
            () => Core.ChangeScene(new GameScene())
        );
    }

    public override void Update(GameTime gameTime)
    {
        _gameButton.Update(Core.Input.Mouse);

        // Keep Enter as a keyboard shortcut for the start button.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            _gameButton.Activate();
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _gameButton.Draw(Core.SpriteBatch);
        Core.SpriteBatch.End();
    }

    public override void UnloadContent()
    {
        _gameButton?.Dispose();
        base.UnloadContent();
    }
}
