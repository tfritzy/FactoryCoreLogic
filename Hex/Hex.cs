using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System; // Needed in 4.7.1
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core
{
    public abstract class Hex : Entity
    {
        public abstract HexType Type { get; }
        public Point3Int GridPosition { get; protected set; }
        public List<ulong> ContainedEntities { get; set; }

        public Hex(Point3Int gridPosition, Context context) : base(context)
        {
            this.GridPosition = gridPosition;
            this.ContainedEntities = new List<ulong>();
        }

        public static Hex? Create(HexType? type, Point3Int gridPosition, Context context)
        {
            if (type == null)
                return null;

            switch (type)
            {
                case HexType.Dirt:
                    return new DirtHex(gridPosition, context);
                case HexType.Stone:
                    return new StoneHex(gridPosition, context);
                default:
                    throw new ArgumentException("Invalid hex type " + type);
            }
        }

        public void AddContainedEntity(Character entity)
        {
            this.ContainedEntities.Add(entity.Id);
            this.Context.World.AddCharacter(entity);
        }

        public abstract Schema.Hex ToSchema();

        public void Destroy()
        {

        }
    }
}