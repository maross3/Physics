using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Physics.DemoGames;
using Physics.DemoGames.BaseGames;
using Physics.DemoGames.Particles;
using static Physics.Constants;

namespace Physics
{
    public class PhysicsDemo : Game
    {
        #region Singleton Access for Demo Games
       
        private static Game _game;
        
        public static Rectangle GetScreenBounds() =>
            ScreenBounds;
        
        public static GraphicsDevice GetGraphicsDevice() =>
            _game.GraphicsDevice;
        
        public static void ExitGame() =>
            _game.Exit();
        
        #endregion
        
        /// <summary>
        /// The bounds of the demo game's screen.
        /// </summary>
        public static Rectangle ScreenBounds { get; private set; }

        private static readonly BaseDemoGame _SCurrentGame =
            new ParticleFrictionDemo();
            //new ParticleDragDemo();
        
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

        /// <summary>
        /// Initializes the demo.
        /// </summary>
        protected override void Initialize()
        {
            _game = this;
            
            _SCurrentGame.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// The load content method, responsible for instantiating all of the demo's content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenBounds = GraphicsDevice.Viewport.Bounds;
            _SCurrentGame.LoadContent(this);
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
            _SCurrentGame.Draw(_spriteBatch);
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }


        #endregion

        #region Updating

        /// <summary>
        /// Delta Update method, intended to be used for physics calculations.
        /// </summary>
        /// <param name="deltaTime">the delta time to apply to the physics simulations.</param>
        private void DeltaUpdate(float deltaTime) =>
            _SCurrentGame.DeltaUpdate(deltaTime);

        // ! remove
        private void TopDownDemo(Particle particle) =>
                particle.AddForce(Force.GenerateFrictionForce(particle, 10 * Constants.PIXELS_PER_METER));

        /// <summary>
        /// The demo's update method, which is called every frame.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/> object.</param>
        protected override void Update(GameTime gameTime)
        {
            CheckAndDoDeltaUpdate(gameTime);
            _SCurrentGame.Update(gameTime);
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