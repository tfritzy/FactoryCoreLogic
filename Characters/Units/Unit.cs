namespace Core
{
    public abstract class Unit : Character
    {
        private Point3Float _location;
        public override Point3Float Location => _location;
        public override Point3Int GridPosition
        {
            get
            {
                return GridHelpers.PixelToEvenRPlusHeight(Location);
            }
            set
            {
                _location = GridHelpers.EvenRToPixelPlusHeight(value);
            }
        }
        public CommandComponent? Command => GetComponent<CommandComponent>();


        public Unit(Context context, int alliance) : base(context, alliance)
        {
        }

        public void SetLocation(Point3Float location)
        {
            _location = location;
        }
    }
}