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
    private Wave _senoidalWave;
    private GradientManager _gradientManager;

    public override void Initialize()
    {
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;
    }

    public override void LoadContent()
    {
        _senoidalWave = new Wave(Core.GraphicsDevice);
        _gradientManager = new GradientManager();
    }

    public override void Update(GameTime gameTime)
    {
        // Check for keyboard input and handle it.
        CheckKeyboardInput();

        _senoidalWave.Update(gameTime);
    }

    private void CheckKeyboardInput()
    {
        // Get a reference to the keyboard info
        KeyboardInfo keyboard = Core.Input.Keyboard;

        // If the escape key is pressed, return to the title screen.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Core.ChangeScene(new TitleScene());
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
        {
            _senoidalWave.Reset();
            _gradientManager.ResetTexture();
        }
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
        _senoidalWave.Draw(gameTime, Core.SpriteBatch);

        Core.SpriteBatch.End();
    }
}
