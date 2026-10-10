using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using game.Senoidal;
using game.Audio;
using System.Diagnostics;
using MonoGameLibrary.UI;

namespace game.Scenes;

public class WaveScene : Scene
{
    private const float AMP_THRESHOLD = 4f;
    private const float FRQ_THRESHOLD = 0.0001f;
    private const float PHA_THRESHOLD = 0.09f;

    private GradientManager _gradientManager;
    private WaveManager _waveManager;
    private AudioManager _audioManager;

    private InterferenceOverlayManager _overlayManager;

    private KeyboardState _previousKeyboard;
    private MouseState _previousMouse;

    private SpriteFont _font;

    private TextureAtlas _atlas;
    private TextureRegion _overlay;
    private TextureRegion _radialButtonRegion;

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
        _gradientManager = new GradientManager();

        Effect waveShader = Content.Load<Effect>("shaders/WaveShader");
        _waveManager = new WaveManager(waveShader);

        _atlas = TextureAtlas.FromFile(Core.Content, "sprites/atlas-definition.xml");
        _overlay = _atlas.GetRegion("overlay");
        _radialButtonRegion = _atlas.GetRegion("botao_radial");
        _radialButtonAmplitude = new RadialButton(_radialButtonRegion, new Rectangle(200, 20,  40, 40));
        _radialButtonPhase = new RadialButton(_radialButtonRegion,     new Rectangle(200, 70,  40, 40));
        _radialButtonFrequency = new RadialButton(_radialButtonRegion, new Rectangle(200, 120, 40, 40));
        _gradientManager = new GradientManager();
        _font = Content.Load<SpriteFont>("fonts/game-font");

        Effect interferenceShader = Content.Load<Effect>("shaders/interferenceShader");
        _overlayManager = new InterferenceOverlayManager(Core.GraphicsDevice, interferenceShader);
        
        // Associa as ondas criadas pelo WaveManager ao OverlayManager
        _overlayManager.UpdateWaves(_waveManager.TargetWave, _waveManager.PlayerWave);

        // Exemplo de como você pode setar os parâmetros visualmente!
        _overlayManager.TargetOpacity = 0.05f;
        _overlayManager.TargetBaseColor = Color.LightGray;

        _overlayManager.PlayerOpacity = 0.05f;
        _overlayManager.PlayerBaseColor = Color.Cyan; // Pode testar cores diferentes aqui
        _audioManager = new AudioManager();
        _audioManager.LoadContent(Content);
    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboard = Keyboard.GetState();
        MouseState currentMouse = Mouse.GetState();

        // VOLTA PRA TELA INICIAL
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            _audioManager.Stop();
            Core.ChangeScene(new TitleScene());
        }

        // PULA PRA PROXIMA FASE (SIPA REMOVER DEPOIS)
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            _audioManager.PlayLevelCompleteJuice();
            Reset();
        }

        // MOSTRA O DEBUG
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.F2))
        {
            _showDebug = !_showDebug;
        }

        // TROCAR PARÂMETRO
        if (currentKeyboard.IsKeyDown(Keys.D) && _previousKeyboard.IsKeyUp(Keys.D) || 
            currentKeyboard.IsKeyDown(Keys.Right) && _previousKeyboard.IsKeyUp(Keys.Right))
        {
            _waveManager.ChangeParameter(1);
        }
        else if (currentKeyboard.IsKeyDown(Keys.A) && _previousKeyboard.IsKeyUp(Keys.A) || 
                currentKeyboard.IsKeyDown(Keys.Left) && _previousKeyboard.IsKeyUp(Keys.Left))
        {
            _waveManager.ChangeParameter(-1);
        }

        // INCREMENTAR/DECREMENTAR VALOR
        float incrementValue = 0f;

        int scrollDelta = currentMouse.ScrollWheelValue - _previousMouse.ScrollWheelValue;
        if (scrollDelta != 0)
        {
            incrementValue = Math.Sign(scrollDelta) * 1f; // Peso do clique do scroll
        }
        else 
        {
            if (currentKeyboard.IsKeyDown(Keys.W) || currentKeyboard.IsKeyDown(Keys.Up))
                incrementValue = 1f;
            if (currentKeyboard.IsKeyDown(Keys.S) || currentKeyboard.IsKeyDown(Keys.Down))
                incrementValue = -1f;
        }

        if (incrementValue != 0f)
        {
            Console.WriteLine($"[incrementValue] {incrementValue}");
            _waveManager.Increment(incrementValue);
            CheckPrecision();
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

        float maxAmpDiff = 190f;
        float maxFreqDiff = 0.009f;
        float maxPhaseDiff = (float)Math.PI;

        float normAmp = MathHelper.Clamp(_waveManager.AmplitudeDiff / maxAmpDiff, 0f, 1f);
        float normFreq = MathHelper.Clamp(_waveManager.FrequencyDiff / maxFreqDiff, 0f, 1f);
        float normPhase = MathHelper.Clamp(_waveManager.PhaseDiff / maxPhaseDiff, 0f, 1f);

        float totalError = (normAmp + normFreq + normPhase) / 3f;

        if (_overlayManager != null)
        {
            _overlayManager.PlayerErrorLevel = totalError;
        }


        bool isInteracting = (incrementValue != 0f);
        _audioManager.Update(gameTime, _waveManager.GetMatchProgress(), isInteracting);

        // Salva o estado atual para comparar no próximo frame
        _previousKeyboard = currentKeyboard;
        _previousMouse = currentMouse;
    }

    private void CheckPrecision()
    {
        if (_waveManager.AmplitudeDiff < AMP_THRESHOLD &&
            _waveManager.FrequencyDiff < FRQ_THRESHOLD &&
            _waveManager.PhaseDiff < PHA_THRESHOLD)
        {
            _audioManager.PlayLevelCompleteJuice();
            Reset();
        }
    }

    private void Reset()
    {
        _gradientManager.ResetTexture();
        _waveManager.Reset();

        if (_overlayManager != null)
        {
            _overlayManager.UpdateWaves(_waveManager.TargetWave, _waveManager.PlayerWave);
        }
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

        if (_overlayManager != null)
        {
            _overlayManager.Draw(Core.SpriteBatch);
        }

        Core.SpriteBatch.Begin();
        _overlay.Draw(Core.SpriteBatch, Vector2.Zero, Color.White);
        _radialButtonAmplitude.Draw();
        _radialButtonPhase.Draw();
        _radialButtonFrequency.Draw();
        Core.SpriteBatch.End();
    }
}