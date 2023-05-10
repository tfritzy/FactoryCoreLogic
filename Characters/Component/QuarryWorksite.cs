using System.Collections.Generic;

namespace Core
{
    public class QuarryWorksite : Worksite
    {
        public override int MaxEmployable => 3;
        private const int Radius = 4;
        private Point2Int centerOffset { get; } = new Point2Int(2, 2);

        public QuarryWorksite(Entity owner) : base(owner)
        {
        }

        public override List<Harvestable> RefreshEligible()
        {
            Point2Int center = ((Building)this.Owner).GridPosition + centerOffset;
            List<Point2Int> points = GridHelpers.GetHexInRange(center, Radius);
            List<Harvestable> harvestables = new List<Harvestable>(points.Count);

            foreach (Point2Int point in points)
            {
                Hex? topHex = this.Owner.Context.World.GetTopHex(point);

                if (topHex == null)
                {
                    continue;
                }

                if (topHex != null && topHex.HasComponent<Harvestable>())
                {
                    harvestables.Add(topHex.GetComponent<Harvestable>());
                }
            }

            return harvestables;
        }
    }
}