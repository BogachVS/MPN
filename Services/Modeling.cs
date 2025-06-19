using MPN.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static MPN.Services.Assets;
namespace MPN.Services

{
    public static class Modeling
    {
        private static readonly string particlesPath = "DataFiles/particles.yaml";
        private static readonly string parametersPath = "DataFiles/parameters.yaml";
        private static readonly string pointsPath = "DataFiles/points.yaml";
        private static readonly string analyticsDistPath = "DataFiles/analyticsDist.yaml";
        private static readonly SerializerBuilder serializerBuilder = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance);

        public static readonly ISerializer serializer = serializerBuilder.Build();

        private static readonly DeserializerBuilder deserializerBuilder = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance);

        public static readonly IDeserializer deserializer = deserializerBuilder.Build();
        public static void CreateArrayOfParticles(ParametersModel model)
        {
            ParticleModel[] array = new ParticleModel[model.DiscCount];
            int maxAttempts = 1000;
            bool IsSphere = model.ShapeType == "Sphere"; 
            for (int i = 0; i < model.DiscCount; i++)
            {
                bool isPlaced = false;
                int attempts = 0;
                while (!isPlaced && attempts < maxAttempts)
                {
                    ParticleModel newParticle = new()
                    {
                        R = GetRandomRadius(model.R_min, model.R_max, model.K, model.Theta).First(),
                        Thickness = GetRandomThickness().First(),
                        Center = IsSphere ? GetRandomPointInSphere(model.Size).First() : GetRandomPoint(model.Size).First()
                    };

                    bool isValid = IsSphere
                        ? IsParticleInsideSphere(newParticle, model.Size)
                        : IsParticleInsideCube(newParticle, model.Size);

                    if (isValid)
                    {
                        bool overlaps = false;
                        for (int j = 0; j < i; j++)
                        {
                            if (ParticlesOverlap(newParticle, array[j]))
                            {
                                overlaps = true;
                                break;
                            }
                        }

                        if (!overlaps)
                        {
                            array[i] = newParticle;
                            isPlaced = true;
                        }
                    }
                    attempts++;
                }

                if (!isPlaced)
                    throw new Exception($"Не удалось разместить {i}-y частицу без перекрытий");
            }

            string ymlParticles = serializer.Serialize(array);
            string ymlParameters = serializer.Serialize(model);
            File.WriteAllText(particlesPath, ymlParticles);
            File.WriteAllText(parametersPath, ymlParameters);

        }
        public static void CreateArrayOfPoints(ParametersModel model)
        {
            IEnumerable<PointModel> points = model.ShapeType == "Sphere" ? GetRandomPointInSphere(model.Size) : GetRandomPoint(model.Size);
            PointModel[] array = [.. points.Take(1_000_000)];
            string ymlPoints = serializer.Serialize(array);
            File.WriteAllText(pointsPath, ymlPoints);
        }
        public static void CreateArrayOfLayers(ParametersModel model)
        {
            string ymlParticles = File.ReadAllText("DataFiles/particles.yaml");
            string ymlPoints = File.ReadAllText("DataFiles/points.yaml");
            ParticleModel[] particles = deserializer.Deserialize<ParticleModel[]>(ymlParticles);
            PointModel[] points = deserializer.Deserialize<PointModel[]>(ymlPoints); 
            float step = model.Size / model.LayerCount;
            LayerModel[] layers = new LayerModel[model.LayerCount];
            var particleGrid = new SpatialGrid(particles, model.Size, 50);
            int[] pointsInCount = new int[model.LayerCount];
            int[] pointsInDiscCount = new int[model.LayerCount];
            Parallel.ForEach(points, point =>
            {
                float distance = (float)Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z * point.Z);
                int layerIndex = (int)(distance / step);

                if (layerIndex >= 0 && layerIndex < model.LayerCount)
                {
                    Interlocked.Increment(ref pointsInCount[layerIndex]);

                    var nearbyParticles = particleGrid.GetNearbyParticles(point);
                    foreach (var particle in nearbyParticles)
                    {
                        float dx = point.X - particle.Center.X;
                        float dy = point.Y - particle.Center.Y;
                        float dz = point.Z - particle.Center.Z;
                        if (dx * dx + dy * dy + dz * dz <= particle.R * particle.R)
                        {
                            Interlocked.Increment(ref pointsInDiscCount[layerIndex]);
                            break;
                        }
                    }
                }
            });
            for (int i = 0; i < model.LayerCount; i++)
            {
                layers[i] = new LayerModel
                {
                    ID = i,
                    Radius = (i + 1) * step,
                    PointsInCount = pointsInCount[i],
                    PointsInDiscCount = pointsInDiscCount[i]
                };
            }
            string ymlAnalytics = serializer.Serialize(layers);
            File.WriteAllText(analyticsDistPath, ymlAnalytics);
        }
    }
}
