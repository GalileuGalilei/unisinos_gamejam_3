using game.Scenes;
using Microsoft.Xna.Framework;
using MonoGameLibrary;

namespace game;

public class Game1 : Core
{
    public Game1() : base("Game Title", 768, 768, false, new Point(256, 256))
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        ChangeScene(new WaveScene());
    }
}
