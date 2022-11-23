using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

namespace Physics
{
    public class Particle
    {
        private Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 acceleration;
        public float mass;
        public GraphicsDevice device;
        public int radius;

        public bool start;
        // slope is velocity, 100/30
        // const acceleration allows velocity to change
        public Particle(float x, float y, float mass)
        {
            radius = 8;
            position = new Vector2(x, y);
            velocity = new Vector2(140, 30);
            this.mass = mass;
        }
        public Particle(float x, float y, float mass, int radius)
        {
            this.radius = radius;
            position = new Vector2(x, y);
            velocity = new Vector2(140, 30);
            this.mass = mass;
        }

        public void Initialize(GraphicsDevice gDevice)
        {
            device = gDevice;
            texture = CreateCircle(radius);
        }

        // constant acceleration defferential & integral calculus:
        // differential:
        // from position ->
        // velocity = delta position / delta time
        // acceleration = velocity / delta time

        // differentialtion
        // position = p0 + velocity0 * time + ((acceleration * time ^ 2) / 2)
        // derive velocity -> v = delta pos / deta time
        // velocity = v0 + acceleration * time
        // derive acceleration -> delta velocity / delta time

        // integral:
        // from acceleration ->
        // velocity = integral of acelleration at a(*) delta time
        // position = integral of velocity at a(*) delta time

        // integral -> acceleration * dt = v0 + acceleration * time
        // integral -> veleocity * dt = p0 + v0 * time + ((acceleration * time ^ 2) / 2) 

        // integrate (estimate) position:
        // velocity += acceleration * delta time,
        // position += velocity * delta time

        // notation:
        // newton notation p(with a dot above it) =
        // Leibniz notation dp/dt =
        // velocity

        // newton notation p (with 2 dots) = 
        // Leibniz notation d^2p / dt^2 = (would this be v or p?)
        // acceleration
        
        public void Update(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) start = true;
            if (position.Y  + texture.Height> Game1.ScreenBounds.Bottom)
            {
                position.Y = Game1.ScreenBounds.Bottom - texture.Height;
                velocity.Y *= -0.8f;
            }
            
            if (position.X + texture.Width > Game1.ScreenBounds.Right)
            {
                position.X = Game1.ScreenBounds.Right - texture.Width;
                velocity.X *= -0.9f;
            }
            else if (position.X < Game1.ScreenBounds.Left)
            {
                position.X = Game1.ScreenBounds.Left + 1;
                velocity.X *= -0.9f;
            }
        }

        public void DeltaUpdate(float deltaTime)
        {
            if (! start) return;
            FindNewPosition(deltaTime);
        }

        // integration
        private void FindNewPosition(float deltaTime)
        {
            velocity += Constants.PARTICLE_ACCELERATION * deltaTime;
            position += velocity * deltaTime;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(
                texture,
                position,
                null,
                Color.White,
                0f,
                new Vector2(0, 0),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }

        private Texture2D CreateCircle(int radius)
        {
            var texture = new Texture2D(device, radius, radius);
            var colorData = new Color[radius * radius];
            var diam = radius / 2f;
            var diamsq = diam * diam;

            for (var x = 0; x < radius; x++)
            {
                for (var y = 0; y < radius; y++)
                {
                    var index = x * radius + y;
                    var pos = new Vector2(x - diam, y - diam);

                    if (pos.LengthSquared() < diamsq)
                        colorData[index] = Color.White;
                    else
                        colorData[index] = Color.Transparent;
                }
            }

            texture.SetData(colorData);
            return texture;
        }
    }
}