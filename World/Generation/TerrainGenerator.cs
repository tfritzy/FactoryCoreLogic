namespace FactoryCore
{
    public class TerrainGenerator
    {
        private Hex[,,] Hexes;

        public TerrainGenerator(int dimX, int dimY, int dimZ)
        {
            Hexes = new Hex[dimX, dimY, dimZ];
        }

        public Hex[,,] GenerateFlatWorld()
        {
            int z = 0;
            for (; z < Hexes.GetLength(2) / 2; z++)
            {
                for (int x = 0; x < Hexes.GetLength(0); x++)
                {
                    for (int y = 0; y < Hexes.GetLength(1); y++)
                    {
                        Hexes[x, y, z] = new Stone();
                    }
                }
            }

            for (int x = 0; x < Hexes.GetLength(0); x++)
            {
                for (int y = 0; y < Hexes.GetLength(1); y++)
                {
                    Hexes[x, y, z] = new Stone();
                }
            }

            return Hexes;
        }
    }
}