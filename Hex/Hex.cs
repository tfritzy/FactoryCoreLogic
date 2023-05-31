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
        public List<ulong> ContainedEntities { get; set; }

        public Hex(Point3Int gridPosition, Context context) : base(context)
        {
            this.GridPosition = gridPosition;
            this.ContainedEntities = new List<ulong>();
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

        public void AddContainedEntity(Entity entity)
        {
            this.ContainedEntities.Add(entity.Id);
        }

        public void RemoveContainedEntity(ulong entity)
        {
            this.ContainedEntities.Remove(entity);
        }

        public abstract Schema.Hex BuildSchemaObject();
        public Schema.Hex ToSchema()
        {
            var schemaHex = BuildSchemaObject();
            schemaHex.ContainedEntities = this.ContainedEntities.FindAll(
                (e) => World.GetCharacter(e) != null)
                .ToList();
            schemaHex.GridPosition = this.GridPosition;
            schemaHex.Id = this.Id;
            return schemaHex;
        }

        public void Destroy()
        {
            if (this.Indestructible)
            {
                return;
            }

            this.World.RemoveHex(this.GridPosition);

            for (int i = 0; i < this.ContainedEntities.Count; i++)
            {
                ulong id = this.ContainedEntities[i];
                Character? character = this.Context.World.GetCharacter(id);
                if (character != null)
                {
                    character.Destroy();
                }
                else
                {
                    this.ContainedEntities.RemoveAt(i);
                }

                i--;
            }
        }
    }
}