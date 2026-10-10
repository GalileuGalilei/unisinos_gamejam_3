using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.UI;
using game.Senoidal;

namespace game.Scenes;

public class TitleScene : Scene
{
    private Font _font;
    private Button _startButton;
    private Button _exitButton;
    private TextureRegion _overlayRegion;
    private TextureRegion _tituloRegion;

    private InterferenceOverlayManager _overlayManager;


    public override void Initialize()
    {
        base.Initialize();
        Core.ExitOnEscape = true;
    }

    public override void LoadContent()
    {
        _font = new Font(Content.Load<SpriteFont>("fonts/game-font"));
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
            () => Core.ChangeScene(new WaveScene())
        );

        _exitButton = new Button(
            _font,
            "",
            exitButtonBounds,
            atlas,
            "sair",
            () => Core.ExitGame()
        );

        Effect interferenceShader = Content.Load<Effect>("shaders/interferenceShader");
        _overlayManager = new InterferenceOverlayManager(Core.GraphicsDevice, interferenceShader);

        // Exemplo de como você pode setar os parâmetros visualmente!
        _overlayManager.TargetOpacity = 0.05f;
        _overlayManager.TargetBaseColor = Color.LightGray;

        _overlayManager.PlayerOpacity = 0.05f;
        _overlayManager.PlayerBaseColor = Color.Cyan; // Pode testar cores diferentes aqui
    }

    public override void Update(GameTime gameTime)
    {
        _startButton.Update(Core.Input.Mouse);
        _exitButton.Update(Core.Input.Mouse);

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
        _overlayRegion.Draw(Core.SpriteBatch, new Vector2(0, 0), Color.White, 0, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0);
        _startButton.Draw(Core.SpriteBatch);
        _exitButton.Draw(Core.SpriteBatch);
        _tituloRegion.Draw(Core.SpriteBatch, new Vector2(40, 20), Color.White, 0, Vector2.Zero, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
        Core.SpriteBatch.End();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
    }
}
