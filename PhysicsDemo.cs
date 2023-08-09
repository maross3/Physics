using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Physics.Constants;

namespace Physics
{
    public class PhysicsDemo : Game
    {
        /// <summary>
        /// The bounds of the demo game's screen.
        /// </summary>
        public static Rectangle ScreenBounds { get; private set; }

        /// <summary>
        /// The graphics device manager for the demo game.
        /// </summary>
        private GraphicsDeviceManager _graphics;

        /// <summary>
        /// The sprite batch for drawing in the demo game.
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// The time since the last step, or the <see cref="Constants.MS_PER_FRAME"/>
        /// </summary>
        private int _deltaTimeNextStep = MS_PER_FRAME;

        // ! remove
        /// <summary>
        /// List of particles for testing.
        /// </summary>
        private static readonly List<Particle> _SParticles = new();

        // ! remove
        /// <summary>
        /// The texture of the water, which is just a blue rectangle. 
        /// </summary>
        private Texture2D _waterTexture;

        // ! remove
        /// <summary>
        /// Force applied to the particle when the up arrow is pressed.
        /// </summary>
        private readonly Vector2 _upForce = new(0, -30 * PIXELS_PER_METER);

        // ! remove
        /// <summary>
        /// Force applied to the particle when the left arrow is pressed.
        /// </summary>
        private readonly Vector2 _leftForce = new(-30 * PIXELS_PER_METER, 0);

        // ! remove
        /// <summary>
        /// Force applied to the particle when the right arrow is pressed.
        /// </summary>
        private readonly Vector2 _rightForce = new(30 * PIXELS_PER_METER, 0);

        // ! remove
        /// <summary>
        /// Force applied to the particle when the down arrow is pressed.
        /// </summary>
        private readonly Vector2 _downForce = new(0, 30 * PIXELS_PER_METER);

        // ! remove
        /// <summary>
        /// The rectangle that represents the liquid.
        /// </summary>
        private Rectangle _liquid;

        /// <summary>
        /// Create a new instance of the demo.
        /// </summary>
        public PhysicsDemo()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #region init

        // ! remove
        /// <summary>
        /// The method to create/initialize the water texture.
        /// </summary>
        private void CreateWaterTexture()
        {
            _waterTexture = new Texture2D(GraphicsDevice, _liquid.Width, _liquid.Height);
            var numPixels = _liquid.Width * _liquid.Height;
            var colorData = new Color[numPixels];

            for (var i = 0; i < numPixels; i++)
                colorData[i] = Color.White;

            _waterTexture.SetData(colorData);
        }

        /// <summary>
        /// Initializes the demo.
        /// </summary>
        protected override void Initialize()
        {
            var particle = new Particle(50, 100, 1.0f, 8);
            var bigParticle = new Particle(150, 100, 10.0f, 20);
            _SParticles.Add(particle);
            _SParticles.Add(bigParticle);

            base.Initialize();
        }

        /// <summary>
        /// The load content method, responsible for instantiating all of the demo's content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var particle in _SParticles) particle.Initialize(GraphicsDevice);
            ScreenBounds = GraphicsDevice.Viewport.Bounds;

            // sets the bottom half of the screen as the liquid
            _liquid.X = 0;
            _liquid.Y = ScreenBounds.Height / 2;
            _liquid.Width = ScreenBounds.Width;
            _liquid.Height = ScreenBounds.Height / 2;

            CreateWaterTexture();
        }

        #endregion

        #region Drawing

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Current <see cref="GameTime"/> object.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            // draw calls
            _spriteBatch.Draw(_waterTexture, _liquid, Color.Blue);
            DrawParticles();

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // ! remove
        /// <summary>
        /// Draws the particles in the demo game.
        /// </summary>
        private void DrawParticles() =>
            _SParticles.ForEach(particle => particle.Draw(_spriteBatch));

        #endregion

        #region Updating

        /// <summary>
        /// Delta Update method, intended to be used for physics calculations.
        /// </summary>
        /// <param name="deltaTime">the delta time to apply to the physics simulations.</param>
        private void DeltaUpdate(float deltaTime)
        {
            foreach (var particle in _SParticles)
            {
                var wind = new Vector2(1, 0);
                particle.AddForce(wind * PIXELS_PER_METER);

                var weight = new Vector2(0, particle.mass * GRAVITY_MPS * PIXELS_PER_METER);
                particle.AddForce(wind * PIXELS_PER_METER);
                particle.AddForce(weight);

                if (particle.position.Y >= _liquid.Y)
                    particle.AddForce(Force.GenerateDragForce(particle, 0.01f));

                particle.DeltaUpdate(deltaTime);
            }
        }

        /// <summary>
        /// The demo's update method, which is called every frame.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/> object.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // handles mouse input, spawns a new particle when the mouse is clicked.
            if (Mouse.GetState() is {LeftButton: ButtonState.Pressed})
            {
                var newParticle = new Particle(Mouse.GetState().X, Mouse.GetState().Y, 1.0f, 8);
                newParticle.Initialize(GraphicsDevice);
                _SParticles.Add(newParticle);
            }

            CheckAndDoDeltaUpdate(gameTime);
            foreach (var particle in _SParticles)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up)) particle.AddForce(_upForce);
                else if (Keyboard.GetState().IsKeyDown(Keys.Left)) particle.AddForce(_leftForce);
                else if (Keyboard.GetState().IsKeyDown(Keys.Right)) particle.AddForce(_rightForce);
                else if (Keyboard.GetState().IsKeyDown(Keys.Down)) particle.AddForce(_downForce);
                particle.Update(gameTime);
            }

            base.Update(gameTime);
        }

        #endregion

        #region Delta Time

        /// <summary>
        /// Creates the delta update method for the demo.
        /// </summary>
        /// <param name="gameTime">The current frame's <see cref="GameTime"/> object.</param>
        private void CheckAndDoDeltaUpdate(GameTime gameTime)
        {
            if (Waiting(gameTime)) return;

            var deltaTime = GetDeltaTime(gameTime);
            ClampDeltaTime(ref deltaTime);
            CreateDeltaTimeNextStep(gameTime);
            DeltaUpdate(deltaTime);
        }

        /// <summary>
        /// Clamps the delta time to the max delta time.
        /// </summary>
        /// <param name="deltaTime"></param>
        private static void ClampDeltaTime(ref float deltaTime) =>
            deltaTime = deltaTime > MAX_DELTA_TIME ? MAX_DELTA_TIME : deltaTime;

        /// <summary>
        /// Creates the next step for the delta update method.
        /// </summary>
        /// <param name="gameTime"></param>
        private void CreateDeltaTimeNextStep(GameTime gameTime) =>
            _deltaTimeNextStep = MS_PER_FRAME + (int) gameTime.TotalGameTime.TotalMilliseconds;

        /// <summary>
        /// Gets the delta time for the current frame.
        /// </summary>
        /// <formula>
        /// TotalMilliseconds - <see cref="_deltaTimeNextStep"/> / 100
        /// </formula>
        private float GetDeltaTime(GameTime gameTime) =>
            ((float) gameTime.TotalGameTime.TotalMilliseconds - _deltaTimeNextStep) / 100;

        /// <summary>
        /// Returns if our demo's delta updates are waiting for the next frame.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private bool Waiting(GameTime gameTime) =>
            (int) gameTime.TotalGameTime.TotalMilliseconds < _deltaTimeNextStep;

        #endregion
    }
}