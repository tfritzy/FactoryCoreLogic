using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System; // Needed in 4.7.1
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Core
{
    public abstract class Hex : Entity
    {
        public abstract HexType Type { get; }
        public virtual bool Transparent => false;
        public virtual bool Indestructible => false;
        public Point3Int GridPosition { get; protected set; }

        public Hex(Point3Int gridPosition, Context context) : base(context)
        {
            this.GridPosition = gridPosition;
        }

        public static Hex Create(HexType type, Point3Int gridPosition, Context context)
        {
            switch (type)
            {
                case HexType.Dirt:
                    return new DirtHex(gridPosition, context);
                case HexType.Stone:
                    return new StoneHex(gridPosition, context);
                case HexType.Water:
                    return new WaterHex(gridPosition, context);
                case HexType.StoneStairs:
                    return new StoneStairs(gridPosition, context);
                case HexType.Bedrock:
                    return new Bedrock(gridPosition, context);
                default:
                    throw new ArgumentException("Invalid hex type " + type);
            }
        }

        public override Schema.Hex ToSchema()
        {
            var schemaHex = (Schema.Hex)base.ToSchema();
            schemaHex.GridPosition = this.GridPosition;
            schemaHex.Id = this.Id;
            return schemaHex;
        }

        public override void Destroy()
        {
            if (this.Indestructible)
            {
                return;
            }

            base.Destroy();

            this.World.RemoveHex(this.GridPosition);
        }

        public void SetGridPosition(Point3Int gridPosition)
        {
            this.GridPosition = gridPosition;
        }
    }
}