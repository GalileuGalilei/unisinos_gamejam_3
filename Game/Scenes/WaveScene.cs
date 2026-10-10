using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.UI;
using game.Audio;
using game.Input;
using game.Senoidal;

namespace game.Scenes;

public class WaveScene : Scene
{
    private const float AMP_THRESHOLD = 4f;
    private const float FRQ_THRESHOLD = 0.0009f;
    private const float PHA_THRESHOLD = 0.09f;

    private GradientManager _gradientManager;
    private WaveManager _waveManager;
    private AudioManager _audioManager;
    private InterferenceOverlayManager _overlayManager;
    private WaveInput _input;

    private SpriteFont _font;
    private TextureRegion _overlay;

    private RadialButton _radialButtonAmplitude;
    private RadialButton _radialButtonPhase;
    private RadialButton _radialButtonFrequency;

    private bool _showDebug = false;

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = false;
    }

    public override void LoadContent()
    {
        _font = Content.Load<SpriteFont>("fonts/game-font");

        _gradientManager = new GradientManager();

        Effect waveShader = Content.Load<Effect>("shaders/WaveShader");
        _waveManager = new WaveManager(waveShader);

        // UI: moldura e botões radiais
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "sprites/atlas-definition.xml");
        _overlay = atlas.GetRegion("overlay");
        TextureRegion radialButtonRegion = atlas.GetRegion("botao_radial");
        _radialButtonAmplitude = new RadialButton(radialButtonRegion, new Rectangle(200, 20,  40, 40));
        _radialButtonPhase     = new RadialButton(radialButtonRegion, new Rectangle(200, 70,  40, 40));
        _radialButtonFrequency = new RadialButton(radialButtonRegion, new Rectangle(200, 120, 40, 40));

        _input = new WaveInput(_radialButtonAmplitude, _radialButtonFrequency, _radialButtonPhase);

        // Interferência visual: as ondas são criadas uma vez só pelo WaveManager,
        // então basta associá-las aqui.
        Effect interferenceShader = Content.Load<Effect>("shaders/interferenceShader");
        _overlayManager = new InterferenceOverlayManager(Core.GraphicsDevice, interferenceShader);
        _overlayManager.UpdateWaves(_waveManager.TargetWave, _waveManager.PlayerWave);
        _overlayManager.TargetOpacity = 0.05f;
        _overlayManager.TargetBaseColor = Color.LightGray;
        _overlayManager.PlayerOpacity = 0.05f;
        _overlayManager.PlayerBaseColor = Color.Cyan;

        _audioManager = new AudioManager();
        _audioManager.LoadContent(Content);
    }

    public override void Update(GameTime gameTime)
    {
        _input.Update();

        // VOLTA PRA TELA INICIAL
        if (_input.BackRequested)
        {
            _audioManager.Stop();
            Core.ChangeScene(new TitleScene());
        }

        // PULA PRA PROXIMA FASE (SIPA REMOVER DEPOIS)
        if (_input.SkipRequested)
        {
            CompleteLevel();
        }

        // MOSTRA O DEBUG
        if (_input.ToggleDebugRequested)
        {
            _showDebug = !_showDebug;
        }

        // Teclado/scroll: parâmetro selecionado
        if (_input.ParameterChange != 0)
        {
            _waveManager.ChangeParameter(_input.ParameterChange);
        }
        _waveManager.Increment(_input.SelectedParameterDelta);

        // Botões radiais: cada um controla um parâmetro
        _waveManager.RotateAmplitude(_input.AmplitudeRotation);
        _waveManager.RotateFrequency(_input.FrequencyRotation);
        _waveManager.RotatePhaseOffset(_input.PhaseRotation);

        if (_input.HasWaveAdjustment)
        {
            CheckPrecision();
        }

        _waveManager.Update(gameTime);

        _overlayManager.PlayerErrorLevel = _waveManager.CalculateWeightedDiff();
        _audioManager.Update(gameTime, _waveManager.GetMatchProgress(), _input.HasWaveAdjustment);
    }

    private void CheckPrecision()
    {
        if (_waveManager.AmplitudeDiff < AMP_THRESHOLD &&
            _waveManager.FrequencyDiff < FRQ_THRESHOLD &&
            _waveManager.PhaseDiff < PHA_THRESHOLD)
        {
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        _audioManager.PlayLevelCompleteJuice();
        _gradientManager.ResetTexture();
        _waveManager.Reset();
    }

    public override void UnloadContent()
    {
        _audioManager.UnloadContent();
        base.UnloadContent();
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

        if (_showDebug)
        {
            string debugText = 
            $"Parametro sendo editado: {_waveManager.CurrentParameter}\n" +
            $"---------Player----------\n" +
            $"Amplitude: {_waveManager.PlayerParameters.Amplitude:F1}\n" +
            $"Frequencia: {_waveManager.PlayerParameters.Frequency:F4}\n" +
            $"Offset de Fase: {_waveManager.PlayerParameters.PhaseOffset:F2}\n" +
            $"---------Diff----------\n" +
            $"Amplitude: {_waveManager.AmplitudeDiff:F1}\n" +
            $"Frequencia: {_waveManager.FrequencyDiff:F4}\n" +
            $"Offset de Fase: {_waveManager.PhaseDiff:F2}\n" +
            $"---------Audio----------\n" +
            $"Progresso: {(_waveManager.GetMatchProgress() * 100f):F0}%\n" +
            $"Volume Ambiente: {_audioManager.CurrentVolume:F2}\n" +
            $"Volume Estática: {_audioManager.CurrentStaticVolume:F2}";

            Core.SpriteBatch.DrawString(_font, debugText, new Vector2(10, 10), Color.Yellow);
        }

        Core.SpriteBatch.End();

        _waveManager.Draw(Core.SpriteBatch);
        _overlayManager.Draw(Core.SpriteBatch);

        Core.SpriteBatch.Begin();
        _overlay.Draw(Core.SpriteBatch, Vector2.Zero, Color.White);
        _radialButtonAmplitude.Draw();
        _radialButtonFrequency.Draw();
        _radialButtonPhase.Draw();
        Core.SpriteBatch.End();
    }
}