using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.UI;
using Microsoft.Xna.Framework.Content;
using System;

namespace game.Scenes;

public class TitleManager
{
    private Font _font;
    private Button _startButton;
    private Button _exitButton;
    private TextureRegion _overlayRegion;
    private TextureRegion _tituloRegion;

    public Action StartCallback;

    public TitleManager(Action startCallback)
    {
        StartCallback = startCallback;
    } 

    public void LoadContent(ContentManager content)
    {
        _font = new Font(content.Load<SpriteFont>("fonts/game-font"));
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "sprites/atlas-definition.xml");
        _overlayRegion = atlas.GetRegion("overlay");
        _tituloRegion = atlas.GetRegion("titulo_menu");

        Point renderResolution = Core.RenderResolution;
        const int buttonWidth = 128;
        const int buttonHeight = 100;
        const int buttonSpacing = 16;
        int buttonsHeight = buttonHeight * 2 + buttonSpacing;
        int buttonsTop = (renderResolution.Y - buttonsHeight) / 2;
        int xPos = 40;
        Rectangle startButtonBounds = new(
            xPos,
            100,
            buttonWidth,
            buttonHeight
        );
        Rectangle exitButtonBounds = new(
            xPos,
            150,
            buttonWidth,
            buttonHeight
        );

 
        _startButton = new Button(
            _font,
            "",
            startButtonBounds,
            atlas,
            "comecar",
            () => StartCallback()
        );

        _exitButton = new Button(
            _font,
            "",
            exitButtonBounds,
            atlas,
            "sair",
            () => Core.ExitGame()
        );
    }

    public void Update()
    {
        _startButton.Update(Core.Input.Mouse);
        _exitButton.Update(Core.Input.Mouse);

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            _startButton.Activate();
        }
    }

    public void Draw()
    {
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _overlayRegion.Draw(Core.SpriteBatch, new Vector2(0, 0), Color.White, 0, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0);
        _startButton.Draw(Core.SpriteBatch);
        _exitButton.Draw(Core.SpriteBatch);
        _tituloRegion.Draw(Core.SpriteBatch, new Vector2(40, 20), Color.White, 0, Vector2.Zero, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
        Core.SpriteBatch.End();
    }
}
