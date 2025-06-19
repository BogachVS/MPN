using MPN.Models;

namespace MPN.Services
{
    public class SpatialGrid
    {
        private readonly List<ParticleModel>[,,] grid;
        private readonly float cellSize;
        private readonly int gridSize;
        private readonly float system;

        public SpatialGrid(ParticleModel[] particles, float systemSize, int gridResolution)
        {
            gridSize = gridResolution;
            cellSize = systemSize / gridResolution;
            grid = new List<ParticleModel>[gridSize, gridSize, gridSize];
            system = systemSize;
            for (int x = 0; x < gridSize; x++)
                for (int y = 0; y < gridSize; y++)
                    for (int z = 0; z < gridSize; z++)
                        grid[x, y, z] = new List<ParticleModel>();

            foreach (var p in particles)
            {
                int x = (int)((p.Center.X + systemSize / 2) / cellSize);
                int y = (int)((p.Center.Y + systemSize / 2) / cellSize);
                int z = (int)((p.Center.Z + systemSize / 2) / cellSize);

                if (x >= 0 && x < gridSize && y >= 0 && y < gridSize && z >= 0 && z < gridSize)
                    grid[x, y, z].Add(p);
            }
        }

        public IEnumerable<ParticleModel> GetNearbyParticles(PointModel point)
        {
            int x = (int)((point.X + system / 2) / cellSize);
            int y = (int)((point.Y + system / 2) / cellSize);
            int z = (int)((point.Z + system / 2) / cellSize);

            for (int i = Math.Max(0, x - 1); i <= Math.Min(gridSize - 1, x + 1); i++)
                for (int j = Math.Max(0, y - 1); j <= Math.Min(gridSize - 1, y + 1); j++)
                    for (int k = Math.Max(0, z - 1); k <= Math.Min(gridSize - 1, z + 1); k++)
                        foreach (var p in grid[i, j, k])
                            yield return p;
        }
    }
}
