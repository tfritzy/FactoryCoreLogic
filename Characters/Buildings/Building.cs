using Core;
using Newtonsoft.Json;

namespace Core
{
    public abstract class Building : Character
    {
        public override Point3Float Location => WorldConversions.HexToUnityPosition(GridPosition);
        public override Point3Int GridPosition => gridPosition;

        private Point3Int gridPosition;

        protected Building(Context context, int alliance) : base(context, alliance)
        {
        }

        public void SetGridPosition(Point3Int gridPosition)
        {
            this.gridPosition = gridPosition;
        }

        public virtual void OnAddToGrid(Point2Int gridPosition)
        {
            int? height = World.GetTopHexHeight(gridPosition);

            if (height == null)
            {
                throw new System.Exception("Cannot add building out of bounds grid position.");
            }

            this.gridPosition = new Point3Int(gridPosition.x, gridPosition.y, height ?? 0);
            foreach (var cell in Components.Values)
            {
                cell.OnAddToGrid();
            }
        }

        public virtual void OnRemoveFromGrid()
        {
            foreach (var cell in Components.Values)
            {
                cell.OnRemoveFromGrid();
            }
        }

        public override void Destroy()
        {
            this.World.RemoveBuilding((Point2Int)this.GridPosition);
            base.Destroy();
        }
    }
}