using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics.DemoGames.BaseGames;

namespace Physics.DemoGames.Particles;

public class ParticleFrictionDemo : ParticleGame
{
    private Vector2 _mouseCursor;
    private bool _leftMouseButtonDown;

    public override void LoadContent(Game game)
    {
        base.LoadContent(game);
        CreateAndAddParticle(100, 100, 20, 40);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        PhysicsDemo.GetGraphicsDevice().Clear(Color.Green);
        base.Draw(spriteBatch);
    }

    protected override void DemoDeltaUpdate(Particle particle, float demoTime)
    {
        var mouseState = Mouse.GetState();

        // Handle mouse motion
        _mouseCursor.X = mouseState.X;
        _mouseCursor.Y = mouseState.Y;

        // Handle mouse button down
        if (!_leftMouseButtonDown && mouseState.LeftButton == ButtonState.Pressed)
        {
            _leftMouseButtonDown = true;
            _mouseCursor.X = mouseState.X;
            _mouseCursor.Y = mouseState.Y;
        }

        // Handle mouse button up
        if (_leftMouseButtonDown && mouseState.LeftButton == ButtonState.Released)
        {
            _leftMouseButtonDown = false;
            var impulseDirection = Vector2.Normalize(SParticles[0].position - _mouseCursor);
            var impulseMagnitude = (SParticles[0].position - _mouseCursor).Length() * 5.0f;
            SParticles[0].velocity = impulseDirection * impulseMagnitude;
        }

        particle.AddForce(Force.GenerateFrictionForce(particle, 20 * Constants.PIXELS_PER_METER));
    }
}