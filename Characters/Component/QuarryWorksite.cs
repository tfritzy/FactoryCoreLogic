using System.Collections.Generic;

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

        public const int Radius = 4;
        public static Point2Int startMineOffset { get; } = new Point2Int(2, 1);
        public static Point2Int centerOffset { get; } = new Point2Int(6, 3);

        private List<Point2Int> inRangeHex = new List<Point2Int>();

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
    }
}