using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    internal static class Force
    {
        public static Vector2 GenerateDragForce(Particle particle, float k)
        {
            var dragForce = new Vector2(0, 0);
            if (particle.velocity.MagnitudeSquared() <= 0) return dragForce;
            
            // calculate drag direction, which is the inverse of velocity unit vector
            var dragDirection = Vector2.Multiply(particle.velocity.Normalized(), -1);
            
            // calculate the magnitude of the drag force k * ||v||^2
            var dragMagnitude = k * particle.velocity.MagnitudeSquared();
            
            // calculate the final drag force
            dragForce = dragDirection * dragMagnitude;

            return dragForce;
        }
    }
    
    internal static class Vector2Extenstions
    {
        public static Vector2 Normalized(this Vector2 vector) =>
            vector / vector.Magnitude();
        public static float Magnitude(this Vector2 vector) =>
            (float) Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        
        public static float MagnitudeSquared(this Vector2 vector) =>
            vector.X * vector.X + vector.Y * vector.Y;
    }
}
