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
    private Button _startButton;
    private Button _exitButton;

    private RadialButton _radialButton;

    public override void Initialize()
    {
        base.Initialize();
        Core.ExitOnEscape = true;
    }

    public override void LoadContent()
    {
        _font = new Font(Content.Load<SpriteFont>("fonts/game-font"));
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "sprites/menu_sprites_definition.xml");

        Rectangle viewport = Core.GraphicsDevice.Viewport.Bounds;
        const int buttonWidth = 256;
        const int buttonHeight = 256;
        const int buttonSpacing = 16;
        int buttonsHeight = buttonHeight * 2 + buttonSpacing;
        int buttonsTop = (viewport.Height - buttonsHeight) / 2;
        Rectangle startButtonBounds = new(
            (viewport.Width - buttonWidth) / 2,
            buttonsTop,
            buttonWidth,
            buttonHeight
        );
        Rectangle exitButtonBounds = new(
            startButtonBounds.X,
            buttonsTop + buttonHeight + buttonSpacing,
            buttonWidth,
            buttonHeight
        );

        Rectangle radialButtonBounds = new(
            startButtonBounds.X,
            buttonsTop,
            buttonWidth,
            buttonHeight
        );

        _startButton = new Button(
            _font,
            "",
            startButtonBounds,
            atlas,
            "comecar",
            () => Core.ChangeScene(new GameScene())
        );

        _exitButton = new Button(
            _font,
            "",
            exitButtonBounds,
            atlas,
            "sair",
            () => Core.ExitGame()
        );

        _radialButton = new RadialButton(atlas.GetRegion("comecar"), radialButtonBounds);
    }

    public override void Update(GameTime gameTime)
    {
        //_startButton.Update(Core.Input.Mouse);
       // _exitButton.Update(Core.Input.Mouse);
        _radialButton.Update(Core.Input.Mouse);

        // Keep Enter as a keyboard shortcut for the start button.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            _startButton.Activate();
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        //_startButton.Draw(Core.SpriteBatch);
        //_exitButton.Draw(Core.SpriteBatch);
        _radialButton.Draw();
        Core.SpriteBatch.End();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
    }
}
