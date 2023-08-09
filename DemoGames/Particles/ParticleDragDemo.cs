using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics.DemoGames.BaseGames;
using static Physics.Constants;

namespace Physics.DemoGames.Particles;

public class ParticleDragDemo : ParticleGame
{
    private Rectangle _liquid;
    private Texture2D _waterTexture;

    private static void CreateParticleUnderMouse()
    {
        var newParticle = new Particle(Mouse.GetState().X, Mouse.GetState().Y, 1.0f, 8);
        newParticle.Initialize(PhysicsDemo.GetGraphicsDevice());
        SParticles.Add(newParticle);
    }

    private void CreateWaterTexture()
    {
        _waterTexture = new Texture2D(GraphicsDevice(), _liquid.Width, _liquid.Height);
        var numPixels = _liquid.Width * _liquid.Height;
        var colorData = new Color[numPixels];

        for (var i = 0; i < numPixels; i++)
            colorData[i] = Color.White;

        _waterTexture.SetData(colorData);
    }

    #region Overrides

    protected override void DemoDeltaUpdate(Particle particle, float demoTime)
    {
        // todo, register input in regular update, with force buffer.
        if (GetKeyDown(Keys.Up) || GetKeyDown(Keys.W)) particle.AddForce(upForce);
        if (GetKeyDown(Keys.Left) || GetKeyDown(Keys.A)) particle.AddForce(leftForce);
        if (GetKeyDown(Keys.Right) || GetKeyDown(Keys.D)) particle.AddForce(rightForce);
        if (GetKeyDown(Keys.Down) || GetKeyDown(Keys.S)) particle.AddForce(downForce);

        var weight = new Vector2(0, particle.mass * GRAVITY_MPS * PIXELS_PER_METER);
        particle.AddForce(weight);

        if (particle.position.Y >= _liquid.Y)
            particle.AddForce(Force.GenerateDragForce(particle, 0.03f));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_waterTexture, _liquid, Color.Blue);
        base.Draw(spriteBatch);
    }

    public override void LoadContent(Game game)
    {
        base.LoadContent(game);

        // sets the bottom half of the screen as the liquid
        _liquid.X = 0;
        _liquid.Y = ScreenBounds().Height / 2;
        _liquid.Width = ScreenBounds().Width;
        _liquid.Height = ScreenBounds().Height / 2;

        CreateWaterTexture();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!GetLeftMouseButton()) return;
        CreateParticleUnderMouse();
    }

    #endregion
}