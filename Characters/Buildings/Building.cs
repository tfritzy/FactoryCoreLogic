using Core;
using Newtonsoft.Json;

namespace Core
{
    public abstract class Building : Character
    {
        private Point3Float location;
        public override Point3Float Location => location;

        protected Building(Context context, int alliance) : base(context, alliance)
        {
        }

        public Point3Float GetLocation()
        {
            int? topHeight = this.World.GetTopHexHeight(this.GridPosition);

            if (topHeight == null)
            {
                return new Point3Float();
            }

            return WorldConversions.HexToUnityPosition(
                this.GridPosition.x,
                this.GridPosition.y,
                topHeight.Value
            );
        }

        public override void OnAddToGrid(Point2Int gridPosition)
        {
            base.OnAddToGrid(gridPosition);
            location = GetLocation();
        }


        public override void Destroy()
        {
            this.World.RemoveBuilding(this.GridPosition);
            base.Destroy();
        }
    }
}