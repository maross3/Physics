using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    internal static class Force
    {
        // todo, handle the case that the velocity is less than the drag force
        /// <summary>
        /// Generate particle drag force: 1/2 * p * Kd * A * ||v||^2 * -v
        /// </summary>
        /// <param name="particle">The particle to generate the force for.</param>
        /// <param name="k">The constant value 1/2*p*Kd*A</param>
        /// <info>
        /// p = density of the fluid<br></br>
        /// Kd = drag coefficient<br></br>
        /// A = cross sectional area of the particle<br></br>
        /// ||v||^2 = magnitude of the velocity squared<br></br>
        /// generates the force in the opposite direction of the velocity.
        /// </info>
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
    
    internal static class Vector2Extensions
    {
        /// <summary>
        /// Returns a new vector that is normalized.
        /// </summary>
        public static Vector2 Normalized(this Vector2 vector) =>
            vector / vector.Magnitude();
        
        /// <summary>
        /// Returns the magnitude of the vector.
        /// </summary>
        public static float Magnitude(this Vector2 vector) =>
            (float) Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        
        /// <summary>
        /// Returns the magnitude of the vector squared. Does not calculate the square root.
        /// </summary>
        public static float MagnitudeSquared(this Vector2 vector) =>
            vector.X * vector.X + vector.Y * vector.Y;
    }
}
