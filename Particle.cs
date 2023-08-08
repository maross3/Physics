using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


// look into supporting other integration techniques and their tradeoffs:
// Euler integration is 'good enough' for most games 
// Verlet for simulating large number of particles
// Runge-Kutta family (family of integrators) for accuracy


// Differentiating a function at a certain time 't', is the process of finding its rate of change (slope/tangent).
// If we need to predict the position of an object, we can integrate the velocity by a certain delta time 'dt'

// net force, sum of all forces (to apply)

// acceleration = force / mass

// kinematics vs kenetics
// kinematics only deals with acceleration and velocity, but not forces.
// we also consider the effects of mass and forces, then we are talking about Kinetics.
namespace Physics
{
    public class Particle
    {
        private Texture2D _texture;
        public Vector2 position;
        public Vector2 velocity;
        private Vector2 acceleration;
        public float mass;
        public float inverseMass;
        public GraphicsDevice device;
        public int radius;

        public bool start;
        private Vector2 _sumForce;

        public void AddForce(Vector2 force) =>
            _sumForce += force;

        public void ClearForces() =>
            _sumForce = Vector2.Zero;

        // slope is velocity, 100/30
        // const acceleration allows velocity to change
        public Particle(float x, float y, float mass)
        {
            radius = 8;
            position = new Vector2(x, y);
            velocity = new Vector2(140, 30);
            this.mass = mass;
            SetInverseMass();
        }

        public Particle(float x, float y, float mass, int radius)
        {
            this.radius = radius;
            position = new Vector2(x, y);
            velocity = new Vector2(140, 30);
            this.mass = mass;
            SetInverseMass();
        }

        private void SetInverseMass() =>
            inverseMass = mass != 0 ? 1 / mass : 0; 
        public void Initialize(GraphicsDevice gDevice)
        {
            device = gDevice;
            _texture = CreateCircle(radius);
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
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !start) start = true;
            else if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                position.X = 50;
                position.Y = 100;
                velocity = new Vector2(140, 30);
            }
            */


            if (position.Y + _texture.Height > Game1.ScreenBounds.Bottom)
            {
                position.Y = Game1.ScreenBounds.Bottom - _texture.Height;
                velocity.Y *= -0.8f;
            }

            if (position.X + _texture.Width > Game1.ScreenBounds.Right)
            {
                position.X = Game1.ScreenBounds.Right - _texture.Width;
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
            //  if (!start) return;
            Integrate(deltaTime);
        }

        // euler
        private void Integrate(float dt)
        {
            acceleration = _sumForce / mass;
            velocity += acceleration * dt;
            position += velocity * dt;
            ClearForces();
        }

        public void Draw(SpriteBatch batch)
        {
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