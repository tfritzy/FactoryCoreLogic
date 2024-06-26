namespace Core
{
    public abstract class Building : Character
    {
        public abstract int Height { get; }
        public HexSide Rotation { get; private set; }
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
        public ItemPort? ItemPort => GetComponent<ItemPort>();
        public Smelt? Smelt => GetComponent<Smelt>();
        public OreInventory? OreInventory => GetComponent<OreInventory>();
        public FuelInventory? FuelInventory => GetComponent<FuelInventory>();

        private Point3Int gridPosition;

        public Building(Schema.Building building, Context context) : base(building.Character, context)
        {
            this.Rotation = building.Rotation;
        }

        protected Building(Context context, int alliance) : base(context, alliance)
        {
        }

        public void SetRotation(HexSide rotation)
        {
            Rotation = rotation;

            foreach (var component in Components.Values)
            {
                component.OnOwnerRotationChanged(rotation);
            }
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

        public new Schema.Building ToSchema()
        {
            var building = new Schema.Building()
            {
                Character = base.ToSchema(),
                Rotation = this.Rotation,
            };
            return building;
        }
    }
}