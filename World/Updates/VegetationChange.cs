using Newtonsoft.Json;
using Schema;

namespace Core
{
    public class TerrainObjectChange : Update
    {
        public override UpdateType Type => UpdateType.TerrainObjectChange;
        public Point2Int GridPosition { get; private set; }
        public TerrainObjectType NewVegeType { get; private set; }

        public TerrainObjectChange(Point2Int location, TerrainObjectType newType)
        {
            GridPosition = location;
            NewVegeType = newType;
        }
    }
}