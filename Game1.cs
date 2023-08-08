using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Physics.Constants;

namespace Physics
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static readonly List<Particle> _SParticles = new();
        public static Rectangle ScreenBounds { get; private set; }

        private Vector2 _upForce = new(0, -30 * PIXELS_PER_METER);
        private Vector2 _leftForce = new(-30 * PIXELS_PER_METER, 0);
        private Vector2 _rightForce = new(30 * PIXELS_PER_METER, 0);
        private Vector2 _downForce = new(0, 30 * PIXELS_PER_METER);
        
        private Rectangle _liquid;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #region init

        protected override void Initialize()
        {
            var particle = new Particle(50, 100, 1.0f, 8);
            var bigParticle = new Particle(150, 100, 10.0f, 20);
           _SParticles.Add(particle);
           _SParticles.Add(bigParticle);
           
            base.Initialize();
        }
        private Texture2D _waterTexture;
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

        private void CreateWaterTexture()
        {
            _waterTexture = new Texture2D(GraphicsDevice, _liquid.Width, _liquid.Height);
            var numPixels = _liquid.Width * _liquid.Height;
            var colorData = new Color[numPixels];

            for (var i = 0; i < numPixels; i++)
                colorData[i] = Color.White;

            _waterTexture.SetData(colorData);
        }

        #endregion

        #region Drawing

        protected override void Draw(GameTime gameTime)
        {
            // todo, should we set the background via a local property or a const?
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            
            // draw calls
            _spriteBatch.Draw(_waterTexture, _liquid, Color.Blue); 
            DrawParticles();
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawParticles() =>
            _SParticles.ForEach(particle => particle.Draw(_spriteBatch));
        
        #endregion

        #region updating

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

            DeltaUpdate(gameTime);
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

        #endregion

        #region Delta Time

        private int deltaTimeNextStep = MS_PER_FRAME;

        private float GetDeltaTime(GameTime gameTime) =>
            ((float)gameTime.TotalGameTime.TotalMilliseconds - deltaTimeNextStep) / 100;

        private void DeltaUpdate(GameTime gameTime)
        {
            if (Waiting(gameTime)) return;

            var deltaTime = GetDeltaTime(gameTime);
            ClampDeltaTime(ref deltaTime);
            CreateDeltaTimeNextStep(gameTime);
            DeltaUpdate(deltaTime);
        }

        private bool Waiting(GameTime gameTime) =>
            (int)gameTime.TotalGameTime.TotalMilliseconds < deltaTimeNextStep;

        private void CreateDeltaTimeNextStep(GameTime gameTime) =>
            deltaTimeNextStep = MS_PER_FRAME + (int)gameTime.TotalGameTime.TotalMilliseconds;

        private static void ClampDeltaTime(ref float deltaTime) =>
            deltaTime = deltaTime > MAX_DELTA_TIME ? MAX_DELTA_TIME : deltaTime;

        #endregion
    }
}