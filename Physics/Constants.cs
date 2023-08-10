
using Microsoft.Xna.Framework;

namespace Physics
{
    public static class Constants
    {
        /// <summary>
        /// The maximum time between steps in the demo.
        /// </summary>
        public const float MAX_DELTA_TIME = 0.016f;
        
        /// <summary>
        /// The number of frames per second in the demo.
        /// </summary>
        public const int FPS = 120;
        
        /// <summary>
        /// The number of milliseconds per frame in the demo.
        /// </summary>
        public const int MS_PER_FRAME = 1000 / FPS;

        /// <summary>
        /// The number of pixels per meter in the demo.
        /// </summary>
        public const int PIXELS_PER_METER = 50;
        
        /// <summary>
        /// The number of meters per pixel in the demo.
        /// </summary>
        public const float GRAVITY_MPS = 9.8f;
        
        // ! remove
        /// <summary>
        /// temporary particle acceleration object
        /// </summary>
        public static readonly Vector2 ParticleAcceleration = new(0.0f, 10.8f * PIXELS_PER_METER);
    }
}
