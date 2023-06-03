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
        public List<Vegetation>? Vegetation { get; private set; }

        public Hex(Point3Int gridPosition, Context context) : base(context)
        {
            this.GridPosition = gridPosition;
            this.Vegetation = new List<Vegetation>();
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

        public override Schema.Entity ToSchema()
        {
            var schemaHex = (Schema.Hex)base.ToSchema();
            schemaHex.GridPosition = this.GridPosition;
            schemaHex.Id = this.Id;
            schemaHex.Vegetation =
                this.Vegetation?
                .Select((Entity e) => (Schema.Vegetation)e.ToSchema())
                .ToList();
            return schemaHex;
        }

        public void SetGridPosition(Point3Int gridPosition)
        {
            this.GridPosition = gridPosition;
        }


        public void AddVegetation(Vegetation vegetation)
        {
            if (this.Vegetation == null)
            {
                this.Vegetation = new List<Vegetation>();
            }

            if (this.Vegetation.Contains(vegetation))
            {
                return;
            }

            this.Vegetation.Add(vegetation);
            vegetation.SetContainedBy(this);
        }

        public void RemoveVegetation(Vegetation vegetation)
        {
            if (this.Vegetation == null)
            {
                return;
            }

            this.Vegetation.Remove(vegetation);
            vegetation.SetContainedBy(null);

            if (this.Vegetation.Count == 0)
            {
                this.Vegetation = null;
            }
        }

        public override void Destroy()
        {
            if (this.Indestructible)
            {
                return;
            }

            if (this.Vegetation != null)
            {
                for (int i = 0; i < this.Vegetation?.Count; i++)
                {
                    this.Vegetation[i].Destroy();
                }
                this.Vegetation = null;
            }

            this.Context.World.RemoveHex(this.GridPosition);
        }
    }
}