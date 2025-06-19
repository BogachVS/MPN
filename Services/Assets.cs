using MathNet.Numerics.Distributions;
using MPN.Models;
using static MathNet.Numerics.Distributions.Gamma;
using static System.Math;
namespace MPN.Services
{
    public static class Assets
    {
        public static bool ParticlesOverlap(ParticleModel a, ParticleModel b)
        {
            float dx = a.Center.X - b.Center.X;
            float dy = a.Center.Y - b.Center.Y;
            float dz = a.Center.Z - b.Center.Z;
            float minDistance = a.R + b.R;
            return dx * dx + dy * dy + dz * dz <= minDistance * minDistance;
        }
        public static bool IsParticleInsideSphere(ParticleModel particle, float sphereRadius)
        {
            float distanceFromCenterSquared = particle.Center.X * particle.Center.X
                                   + particle.Center.Y * particle.Center.Y
                                   + particle.Center.Z * particle.Center.Z;
            float maxAllowed = sphereRadius - particle.R;
            return distanceFromCenterSquared <= maxAllowed * maxAllowed;
        }
        public static bool IsParticleInsideCube(ParticleModel particle, float halfSize)
        {
            return Abs(particle.Center.X) + particle.R <= halfSize &&
                   Abs(particle.Center.Y) + particle.R <= halfSize &&
                   Abs(particle.Center.Z) + particle.R <= halfSize;
        }
        public static IEnumerable<PointModel> GetRandomPoint(float value)
        {
            while(true)
            {
                yield return new PointModel()
                {
                    X = (float)(value * (Random.Shared.NextSingle() * 2 - 1)),
                    Y = (float)(value * (Random.Shared.NextSingle() * 2 - 1)),
                    Z = (float)(value * (Random.Shared.NextSingle() * 2 - 1)),
                };
            }
        }
        public static IEnumerable<PointModel> GetRandomPointInSphere(float radius)
        {
            while (true)
            {
                float x = radius * (Random.Shared.NextSingle() * 2 - 1);
                float y = radius * (Random.Shared.NextSingle() * 2 - 1);
                float z = radius * (Random.Shared.NextSingle() * 2 - 1);
                if (x * x + y * y + z * z <= radius * radius)
                    yield return new PointModel { X = x, Y = y, Z = z };
            }
        }
        public static IEnumerable<float> GetRandomThickness()
        {
            while(true)
            {
                yield return (float)(Random.Shared.Next(0,101) / Pow(10,9));
            }
        }
        public static IEnumerable<float> GetRandomRadius(float rmin, float rmax, float K, float Theta)
        {
            Gamma gamma = WithShapeScale(K, Theta);
            double max_value = gamma.InverseCumulativeDistribution(0.9999);
            while(true)
            {
                yield return 
                    (float)Round(gamma.Sample() / max_value * (rmax - rmin) + rmin,4);
            }
        }
    }
}
