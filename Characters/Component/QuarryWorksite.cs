using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class QuarryWorksite : Worksite
    {
        public override ComponentType Type => ComponentType.QuarryWorksite;
        public override int MaxEmployable => 3;
        protected override List<Point2Int> GetEligibleHex() => inRangeHex;
        protected override Point2Int GetStartPoint() => ((Building)this.Owner).GridPosition + startMineOffset;
        protected override bool OnlyIncludeTopLayer => true;
        protected override Schema.Worksite BuildSchemaObject() => new Schema.QuarryWorksite();

        public const int Radius = 3;
        public static Point2Int startMineOffset { get; } = new Point2Int(1, 1);
        public static Point2Int centerOffset { get; } = new Point2Int(4, 2);
        private const int MAX_DEPTH = 100;

        private List<Point2Int> inRangeHex = new List<Point2Int>();
        private HashSet<Point3Int> stairPositions = new HashSet<Point3Int>();

        public QuarryWorksite(Entity owner) : base(owner) { }

        protected override Harvestable? GetHarvestable(Hex hex)
        {
            return hex?.Harvestable;
        }

        protected override void InitInRangeHex()
        {
            Point2Int center = ((Building)this.Owner).GridPosition + centerOffset;
            inRangeHex = GridHelpers.GetHexInRange(center, Radius);
        }

        private void InitStairPositions()
        {
            Point2Int buildingPos = ((Building)this.Owner).GridPosition;
            stairPositions = new HashSet<Point3Int>();
            int buildingHeight = this.World.GetTopHexHeight(buildingPos);
            var ring = GridHelpers.GetHexRing(buildingPos + centerOffset, Radius);
            GridHelpers.SortHexByAngle(ring, buildingPos + centerOffset, clockwise: true);
            int startIndex = ring.IndexOf(buildingPos + startMineOffset);
            ring = ring.Skip(startIndex).Concat(ring.Take(startIndex)).ToList();

            for (int i = 0; i < MAX_DEPTH; i++)
            {
                int heightOffset = buildingHeight - i;
                while (heightOffset < 0)
                {
                    heightOffset += ring.Count;
                }
                Point2Int stairPos = ring[heightOffset % ring.Count];
                stairPositions.Add(new Point3Int(stairPos.x, stairPos.y, i));
            }
        }

        public override void OnAddToGrid()
        {
            base.OnAddToGrid();
            InitStairPositions();
        }

        protected override void RefineFoundHarvestables(List<List<Harvestable>> eligibleHarvestables)
        {
            for (int d = 0; d < eligibleHarvestables.Count; d++)
            {
                for (int i = 0; i < eligibleHarvestables[d].Count; i++)
                {
                    if (!(eligibleHarvestables[d][i].Owner is Hex))
                    {
                        continue;
                    }

                    Point3Int pos = ((Hex)eligibleHarvestables[d][i].Owner).GridPosition;
                    if (stairPositions.Contains(pos))
                    {
                        Hex? newHex = Hex.Create(HexType.StoneStairs, pos, this.Owner.Context);
                        this.World.SetHex(pos, newHex);

                        eligibleHarvestables[d].RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}