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

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (var particle in _SParticles) particle.Initialize(GraphicsDevice);
            ScreenBounds = GraphicsDevice.Viewport.Bounds;
           liquid.X = 0;
           liquid.Y = ScreenBounds.Height / 2;
           liquid.Width = ScreenBounds.Width;
           liquid.Height = ScreenBounds.Height / 2;
        }

        #endregion

        #region Drawing

        protected override void Draw(GameTime gameTime)
        {
            // todo, should we set the background via a local property or a const?
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            
            // draw calls
            DrawParticles();
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawParticles() =>
            _SParticles.ForEach(particle => particle.Draw(_spriteBatch));
        
        #endregion

        #region updating
        private Vector2 UpForce = new(0, -30 * PIXELS_PER_METER);
        private Vector2 LeftForce = new(-30 * PIXELS_PER_METER, 0);
        private Vector2 RightForce = new(30 * PIXELS_PER_METER, 0);
        private Vector2 DownForce = new(0, 30 * PIXELS_PER_METER);
        private Rectangle liquid;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DeltaUpdate(gameTime);
            foreach (var particle in _SParticles)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up)) particle.AddForce(UpForce);
                else if (Keyboard.GetState().IsKeyDown(Keys.Left)) particle.AddForce(LeftForce);
                else if (Keyboard.GetState().IsKeyDown(Keys.Right)) particle.AddForce(RightForce);
                else if (Keyboard.GetState().IsKeyDown(Keys.Down)) particle.AddForce(DownForce);
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
                
                if (particle.position.Y >= liquid.Y)
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