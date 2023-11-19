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
            int z = 0;
            for (; z < Hexes.GetLength(2) / 2; z++)
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
                    Hexes[x, y, z] = TriangleType.Dirt;
                }
            }

            return Hexes;
        }

        public TriangleType?[,,] GenerateRollingHills(Context context)
        {
            // OpenSimplexNoise noise = new OpenSimplexNoise();

            // for (int x = 0; x < Hexes.GetLength(0); x++)
            // {
            //     for (int y = 0; y < Hexes.GetLength(1); y++)
            //     {
            //         int height = (int)(noise.Evaluate(x / 125f, y / 125f) * (Hexes.GetLength(2) / 2));
            //         for (int z = 0; z < height; z++)
            //         {
            //             Hexes[x, y, z] = TriangleType.Stone;
            //         }
            //     }
            // }

            // for (int x = 0; x < Hexes.GetLength(0); x++)
            // {
            //     for (int y = 0; y < Hexes.GetLength(1); y++)
            //     {
            //         Hexes[x, y, 0] = TriangleType.Stone;
            //     }
            // }

            return null;
        }
    }
}