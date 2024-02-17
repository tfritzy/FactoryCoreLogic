namespace Core
{
    public abstract class Unit : Character
    {
        private Point3Float _location;
        public override Point3Float Location => _location;
        public Point3Float Velocity { get; set; }
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

        public Unit(Schema.Unit unit, Context context) : base(unit.Character, context)
        {
            _location = Point3Float.FromSchema(unit.Location);
        }

        public Unit(Context context, int alliance) : base(context, alliance)
        {
        }

        public new Schema.Unit ToSchema()
        {
            return new Schema.Unit()
            {
                Character = base.ToSchema(),
                Location = Location.ToSchema(),
            };
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
        }

        public void SetLocation(Point3Float location)
        {
            _location = location;
        }

        public void ClientTick(float deltaTime)
        {
            _location += Velocity * deltaTime;
        }
    }
}