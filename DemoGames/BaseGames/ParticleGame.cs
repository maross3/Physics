using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Physics.Constants;

namespace Physics.DemoGames.BaseGames;

public abstract class ParticleGame : BaseDemoGame
{
    /// <summary>
    /// List of particles for testing.
    /// </summary>
    protected static readonly List<Particle> SParticles = new();

    /// <summary>
    /// Force applied to the particle when the up arrow is pressed.
    /// </summary>
    protected readonly Vector2 upForce = new(0, -30 * PIXELS_PER_METER);

    /// <summary>
    /// Force applied to the particle when the left arrow is pressed.
    /// </summary>
    protected readonly Vector2 leftForce = new(-30 * PIXELS_PER_METER, 0);

    /// <summary>
    /// Force applied to the particle when the right arrow is pressed.
    /// </summary>
    protected readonly Vector2 rightForce = new(30 * PIXELS_PER_METER, 0);

    /// <summary>
    /// Force applied to the particle when the down arrow is pressed.
    /// </summary>
    protected readonly Vector2 downForce = new(0, 30 * PIXELS_PER_METER);

    public override void Update(GameTime gameTime)
    {
        if (BackButtonPressed() || EscapeButtonPressed())
            ExitGame();
        foreach (var particle in SParticles)
            particle.Update(gameTime);
    }

    public override void DeltaUpdate(float deltaTime)
    {
        foreach (var particle in SParticles)
        {
            DemoDeltaUpdate(particle, deltaTime);
            particle.DeltaUpdate(deltaTime);
        }
    }

    protected abstract void DemoDeltaUpdate(Particle particle, float demoTime);

    public override void Draw(SpriteBatch spriteBatch) =>
        DrawParticles(spriteBatch);

    public override void LoadContent(Game game)
    {
    }

    public override void Initialize()
    {
        foreach (var particle in SParticles) 
            particle.Initialize(GraphicsDevice());
        /*
         * * example usage:
            var particle = new Particle(50, 100, 1.0f, 8);
            var bigParticle = new Particle(150, 100, 10.0f, 20);
            _SParticles.Add(particle);
            _SParticles.Add(bigParticle);
         */
    }

    /// <summary>
    /// Draws the particles in the demo game.
    /// </summary>
    private static void DrawParticles(SpriteBatch spriteBatch) =>
        SParticles.ForEach(particle => particle.Draw(spriteBatch));
}