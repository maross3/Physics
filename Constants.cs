
using Microsoft.Xna.Framework;

namespace Physics
{
    public static class Constants
    {
        public const float MAX_DELTA_TIME = 0.016f;
        public const int FPS = 120;
        public const int MS_PER_FRAME = 1000 / FPS;

        public const int PIXELS_PER_METER = 50;
        public const float GRAVITY_MPS = 9.8f;
        public static readonly Vector2 ParticleAcceleration = new(0.0f, 10.8f * PIXELS_PER_METER);
    }
}
