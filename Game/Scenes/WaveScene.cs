using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using game.Senoidal;
using MonoGameLibrary.UI;

namespace game.Scenes;

public class WaveScene : Scene
{
    private const float AMP_THRESHOLD = 4f;
    private const float FRQ_THRESHOLD = 0.0001f;
    private const float PHA_THRESHOLD = 0.09f;

    private GradientManager _gradientManager;
    private WaveManager _waveManager;

    private TextureAtlas _atlas;
    private TextureRegion _overlay;
    private TextureRegion _radialButtonRegion;

    private RadialButton _radialButtonAmplitude;
    private RadialButton _radialButtonPhase;
    private RadialButton _radialButtonFrequency;

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = false;
    }

    public override void LoadContent()
    {
        _gradientManager = new GradientManager();

        Effect waveShader = Content.Load<Effect>("shaders/WaveShader");
        _waveManager = new WaveManager(waveShader);

        _atlas = TextureAtlas.FromFile(Core.Content, "sprites/atlas-definition.xml");
        _overlay = _atlas.GetRegion("overlay");
        _radialButtonRegion = _atlas.GetRegion("botao_radial");
        _radialButtonAmplitude = new RadialButton(_radialButtonRegion, new Rectangle(200, 20,  40, 40));
        _radialButtonPhase = new RadialButton(_radialButtonRegion,     new Rectangle(200, 70,  40, 40));
        _radialButtonFrequency = new RadialButton(_radialButtonRegion, new Rectangle(200, 120, 40, 40));
    }

    public override void Update(GameTime gameTime)
    {
        // VOLTA PRA TELA INICIAL
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Core.ChangeScene(new TitleScene());
        }

        // PULA PRA PROXIMA FASE (SIPA REMOVER DEPOIS)
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            Reset();
        }

        _radialButtonAmplitude.Update(Core.Input.Mouse);
        _radialButtonPhase.Update(Core.Input.Mouse);
        _radialButtonFrequency.Update(Core.Input.Mouse);

        _waveManager.RotateAmplitude(_radialButtonAmplitude.RotationDelta);
        _waveManager.RotatePhaseOffset(_radialButtonPhase.RotationDelta);
        _waveManager.RotateFrequency(_radialButtonFrequency.RotationDelta);

        if (_radialButtonAmplitude.RotationDelta != 0f ||
            _radialButtonPhase.RotationDelta != 0f ||
            _radialButtonFrequency.RotationDelta != 0f)
        {
            CheckPrecision();
        }

        _waveManager.Update(gameTime);
    }

    private void CheckPrecision()
    {
        if (_waveManager.AmplitudeDiff < AMP_THRESHOLD &&
            _waveManager.FrequencyDiff < FRQ_THRESHOLD &&
            _waveManager.PhaseDiff < PHA_THRESHOLD)
        {
            Reset();
        }
    }

    private void Reset()
    {
        _gradientManager.ResetTexture();
        _waveManager.Reset();
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.MonoGameOrange);

        int width = Core.GraphicsDevice.Viewport.Width;
        int height = Core.GraphicsDevice.Viewport.Height;
        Rectangle viewportRect = new Rectangle(0, 0, width, height);

        Core.SpriteBatch.Begin();

        Texture2D backgroundTexture = _gradientManager.GetTexture();
        Core.SpriteBatch.Draw(backgroundTexture, viewportRect, Color.White);

        Core.SpriteBatch.End();

        _waveManager.Draw(Core.SpriteBatch);

        Core.SpriteBatch.Begin();
        _overlay.Draw(Core.SpriteBatch, Vector2.Zero, Color.White);
        _radialButtonAmplitude.Draw();
        _radialButtonPhase.Draw();
        _radialButtonFrequency.Draw();
        Core.SpriteBatch.End();
    }
}
