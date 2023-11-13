using System.Numerics;
using Core;
using Newtonsoft.Json;

namespace Core
{
    public abstract class Building : Character
    {
        public abstract int Height { get; }
        public override Point3Float Location => GridHelpers.EvenRToPixelPlusHeight(GridPosition);
        public override Point3Int GridPosition
        {
            get
            {
                return gridPosition;
            }
            set
            {
                gridPosition = value;
            }
        }

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
            Point3Int top = this.World.GetTopHex(gridPosition);
            this.gridPosition = top;
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