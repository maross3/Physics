using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Physics.Constants;

namespace Physics
{
    public class Game1 : Game
    {
        // do we need this?
        private GraphicsDeviceManager _graphics;

        private SpriteBatch _spriteBatch;
        public static Particle particle;

        public static Rectangle ScreenBounds { get; set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #region init

        protected override void Initialize()
        {
            particle = new Particle(50, 100, 1.0f);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            particle.Initialize(GraphicsDevice);
            ScreenBounds = GraphicsDevice.Viewport.Bounds;

            // look into GraphicsDevice.DisplayMode
        }

        #endregion

        #region Drawing

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            particle.Draw(_spriteBatch);
            // _spriteBatch.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region updating

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            DeltaUpdate(gameTime);
            particle.Update(gameTime);
            base.Update(gameTime);
        }

        private void DeltaUpdate(float deltaTime)
        {
            particle.DeltaUpdate(deltaTime);
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