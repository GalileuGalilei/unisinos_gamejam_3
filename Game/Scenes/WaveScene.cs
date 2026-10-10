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
using System.Diagnostics;

namespace game.Scenes;

public class WaveScene : Scene
{
    private GradientManager _gradientManager;
    private WaveManager _waveManager;

    private KeyboardState _previousKeyboard;
    private MouseState _previousMouse;

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
    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboard = Keyboard.GetState();
        MouseState currentMouse = Mouse.GetState();

        // VOLTA PRA TELA INICIAL
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Core.ChangeScene(new TitleScene());
        }

        // PULA PRA PROXIMA FASE (SIPA REMOVER DEPOIS)
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            _gradientManager.ResetTexture();
            _waveManager.Reset();
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

        // A. Verifica o Scroll do Mouse (Normalizado via Math.Sign para retornar apenas -1, 0 ou 1)
        int scrollDelta = currentMouse.ScrollWheelValue - _previousMouse.ScrollWheelValue;
        if (scrollDelta != 0)
        {
            incrementValue = Math.Sign(scrollDelta) * 5f; // Peso do clique do scroll
        }
        // B. Fallback para Teclado (Input Contínuo - Segurar a tecla mantém incrementando)
        else 
        {
            if (currentKeyboard.IsKeyDown(Keys.W) || currentKeyboard.IsKeyDown(Keys.Up))
                incrementValue = 1f;
            if (currentKeyboard.IsKeyDown(Keys.S) || currentKeyboard.IsKeyDown(Keys.Down))
                incrementValue = -1f;
        }

        // Envia a instrução para o manager apenas se houve alguma ação
        if (incrementValue != 0f)
        {
            Console.WriteLine($"[incrementValue] {incrementValue}");
            _waveManager.Increment(incrementValue);
        }

        _waveManager.Update(gameTime);

        // Salva o estado atual para comparar no próximo frame
        _previousKeyboard = currentKeyboard;
        _previousMouse = currentMouse;
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
    }
}
