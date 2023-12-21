using Noise;

namespace Core
{
    public class TerrainGenerator
    {
        private TriangleType?[,,] Hexes;

        public TerrainGenerator(int dimX, int dimY, int dimZ)
        {
            Hexes = new TriangleType?[dimX, dimY, dimZ];
        }

        public TriangleType?[,,] GenerateFlatWorld(Context context)
        {
            for (int z = 0; z < Hexes.GetLength(2) / 2; z++)
            {
                for (int x = 0; x < Hexes.GetLength(0); x++)
                {
                    for (int y = 0; y < Hexes.GetLength(1); y++)
                    {
                        Hexes[x, y, z] = TriangleType.Stone;
                    }
                }
            }

            for (int x = 0; x < Hexes.GetLength(0); x++)
            {
                for (int y = 0; y < Hexes.GetLength(1); y++)
                {
                    Hexes[x, y, Hexes.GetLength(2) / 2] = TriangleType.Dirt;
                }
            }

            return Hexes;
        }

        public TriangleType?[,,] GenerateRollingHills(Context context)
        {
            OpenSimplexNoise noise = new OpenSimplexNoise();
            for (int x = 0; x < Hexes.GetLength(0); x++)
            {
                for (int y = 0; y < Hexes.GetLength(1); y++)
                {
                    double noiseVal = noise.Evaluate(x / 50f, y / 50f);
                    noiseVal = (noiseVal + 1) / 2;
                    int height = (int)(noiseVal * 10);
                    for (int z = 0; z < height; z++)
                    {
                        Hexes[x, y, z] = TriangleType.Stone;
                    }
                }
            }

            return Hexes;
        }
    }
}