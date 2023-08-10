using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#region notes

// look into supporting other integration techniques and their tradeoffs:
// Euler integration is 'good enough' for most games 
// Verlet for simulating large number of particles
// Runge-Kutta family (family of integrators) for accuracy


// Particle Notes:
// Differentiating a function at a certain time 't', is the process of finding its rate of change (slope/tangent).
// If we need to predict the position of an object, we can integrate the velocity by a certain delta time 'dt'

// net force, sum of all forces (to apply)

// acceleration = force / mass

// kinematics vs kinetics
// kinematics only deals with acceleration and velocity, but not forces.
// we also consider the effects of mass and forces, then we are talking about Kinetics.

// constant acceleration differential & integral calculus:
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

// slope is velocity, 100/30
// const acceleration allows velocity to change

#endregion

namespace Physics
{
    public class Particle
    {
        /// <summary>
        /// The position of the particle.
        /// </summary>
        public Vector2 position;
        
        /// <summary>
        /// The velocity of the particle.
        /// </summary>
        public Vector2 velocity;
        
        /// <summary>
        /// The acceleration of the particle.
        /// </summary>
        private Vector2 _acceleration;
        
        /// <summary>
        /// The <see cref="GraphicsDevice"/> used to draw the particle.
        /// </summary>
        private GraphicsDevice _device;

        /// <summary>
        /// The inverse mass of the particle, which is 1/mass.
        /// </summary>
        public float inverseMass;
        
        // drawback: of being readonly leads to not being able to change at runtime.
        public readonly float mass;
        
        /// <summary>
        /// The radius of the particle determined at runtime during construction.
        /// </summary>
        private readonly int _radius;
        
        /// <summary>
        /// The sum of all forces applied to the particle.
        /// </summary>
        private Vector2 _sumForce;
        
        /// <summary>
        /// The texture of the particle, which is just a circle.
        /// </summary>
        private Texture2D _texture;

        private Color _color = Color.White;
        public Particle(float x, float y, float mass, int radius)
        {
            _radius = radius;
            position = new Vector2(x, y);
            this.mass = mass;
            SetInverseMass();
        }

        public Particle(float x, float y, float mass, int radius, Color color)
        {
            _color = color;
            _radius = radius;
            position = new Vector2(x, y);
            this.mass = mass;
            SetInverseMass();
        }

        #region Initialization
        
        /// <summary>
        /// Creates a circle, todo, move to a primitives static class.
        /// </summary>
        /// <param name="radius">The radius of the particle determined at runtime during construction.</param>
        /// <returns></returns>
        private Texture2D CreateCircle(int radius)
        {
            var texture = new Texture2D(_device, radius, radius);
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
                        colorData[index] = _color;
                    else
                        colorData[index] = Color.Transparent;
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        /// <summary>
        /// Assigns the graphic device and creates the texture.
        /// </summary>
        /// <param name="gDevice">The game's <see cref="GraphicsDevice"/></param>
        public void Initialize(GraphicsDevice gDevice)
        {
            _device = gDevice;
            _texture = CreateCircle(_radius);
        }
        
        #endregion

        #region GameLoop Calls
        
        public void DeltaUpdate(float deltaTime) =>
            Integrate(deltaTime);

        /// <summary>
        /// The draw method called from the games draw loop.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(SpriteBatch batch) =>
            batch.Draw(
                _texture,
                position,
                null,
                Color.White,
                0f,
                new Vector2(0, 0),
                Vector2.One,
                SpriteEffects.None,
                0f
            );

        /// <summary>
        /// Called from the games update loop.
        /// </summary>
        /// <param name="time">The game time object.</param>
        public void Update(GameTime time)
        {
            // Collisions for the bottom
            if (position.Y + _texture.Height > PhysicsDemo.ScreenBounds.Bottom)
            {
                position.Y = PhysicsDemo.ScreenBounds.Bottom - _texture.Height;
                velocity.Y *= -0.8f;
            }

            // Collisions for the top
            if (position.Y < PhysicsDemo.ScreenBounds.Top)
            {
                position.Y = PhysicsDemo.ScreenBounds.Top;
                velocity.Y *= -0.8f;
            }

            // Collisions for the right side
            if (position.X + _texture.Width > PhysicsDemo.ScreenBounds.Right)
            {
                position.X = PhysicsDemo.ScreenBounds.Right - _texture.Width;
                velocity.X *= -0.9f;
            }
            // Collisions for the left side
            else if (position.X < PhysicsDemo.ScreenBounds.Left)
            {
                position.X = PhysicsDemo.ScreenBounds.Left + 1;
                velocity.X *= -0.9f;
            }
        }

        #endregion
        
        /// <summary>
        /// Add a force to the sum of forces.
        /// </summary>
        /// <param name="force">The force to add.</param>
        public void AddForce(Vector2 force) =>
            _sumForce += force;
        
        /// <summary>
        /// Clears the sum of forces.
        /// </summary>
        private void ClearForces() =>
            _sumForce = Vector2.Zero;
        

        /// <summary>
        /// Integrate the physics using Euler integration.
        /// </summary>
        /// <param name="dt">deltaTime</param>
        private void Integrate(float dt)
        {
            _acceleration = _sumForce / mass;
            velocity += _acceleration * dt;
            position += velocity * dt;
            ClearForces();
        }
        
        /// <summary>
        /// Precompute the inverse of the mass value.
        /// </summary>
        private void SetInverseMass() =>
            inverseMass = mass != 0 ? 1 / mass : 0;
    }
}