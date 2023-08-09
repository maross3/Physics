using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Physics.DemoGames.BaseGames;

public abstract class BaseDemoGame
{
    public abstract void DeltaUpdate(float deltaTime);
    public abstract void Draw(SpriteBatch spriteBatch);

    /// <summary>
    /// load content
    /// </summary>
    /// <tip>
    /// game.Content.Load Texture2D ("your texture");
    /// </tip>
    public abstract void LoadContent(Game game);

    public abstract void Initialize();

    public abstract void Update(GameTime gameTime);
    protected static GraphicsDevice GraphicsDevice() =>
        PhysicsDemo.GetGraphicsDevice();
    protected static Rectangle ScreenBounds() =>
        PhysicsDemo.GetScreenBounds();
    
    protected static bool GetKeyDown(Keys key) =>
        Keyboard.GetState().IsKeyDown(key);
    
    protected static bool GetLeftMouseButton() =>
        Mouse.GetState().LeftButton == ButtonState.Pressed;
    protected static void ExitGame() =>
        PhysicsDemo.ExitGame();

    protected static bool BackButtonPressed() =>
        GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed;

    protected static bool EscapeButtonPressed() =>
        Keyboard.GetState().IsKeyDown(Keys.Escape);
}