using System;
using System.Linq;

namespace Core
{
    public abstract class Character : Entity, Schema.SerializesTo<Schema.Character>
    {
        public abstract CharacterType Type { get; }
        public abstract Point3Int GridPosition { get; }
        public virtual Point3Float Location { get; }
        public bool IsPreview { get; private set; }
        public int Alliance { get; private set; }
        private static Point3Float defaultProjectileOffset = new Point3Float();
        public virtual Point3Float ProjectileSpawnOffset => defaultProjectileOffset;

        public Character(Context context, int alliance) : base(context)
        {
            this.Alliance = alliance;
        }

        public virtual void Tick(float deltaTime)
        {
            foreach (var cell in Components.Values)
            {
                cell.Tick(deltaTime);
            }
        }

        public void UpdateOwnerOfCells()
        {
            foreach (var cell in Components.Values)
            {
                cell.Owner = this;
            }
        }

        public static Character Create(
            CharacterType character,
            Context context,
            int alliance = Constants.Alliance.NEUTRAL)
        {
            switch (character)
            {
                case CharacterType.Dummy:
                    return new Dummy(context, alliance);
                case CharacterType.DummyBuilding:
                    return new DummyBuilding(context, alliance);
                case CharacterType.Conveyor:
                    return new Conveyor(context, alliance);
                case CharacterType.Player:
                    return new Player(context, alliance);
                case CharacterType.Villager:
                    return new Villager(context, alliance);
                case CharacterType.Quarry:
                    return new Quarry(context, alliance);
                case CharacterType.GuardTower:
                    return new GuardTower(context, alliance);
                default:
                    throw new ArgumentException("Invalid character type " + character);
            }
        }

        protected Schema.Character PopulateSchema(Schema.Character character)
        {
            character.Id = this.Id;
            character.GridPosition = this.GridPosition;
            character.Alliance = this.Alliance;
            character.Components = this.Components.ToDictionary(
                x => Component.ComponentTypeMap[x.Key], x => x.Value.ToSchema());
            return character;
        }

        public abstract Schema.Character ToSchema();

        public override void SetComponent(Component component)
        {
            base.SetComponent(component);

            if (IsPreview)
            {
                component.Disabled = true;
            }
        }

        public void MarkPreview()
        {
            this.IsPreview = true;

            foreach (Component component in this.Components.Values)
            {
                component.Disabled = true;
            }
        }

        public void ClearPreview()
        {
            this.IsPreview = false;

            foreach (Component component in this.Components.Values)
            {
                component.Disabled = false;
            }
        }

        public virtual void Destroy()
        {
            Context.World.RemoveCharacter(this.Id);
        }
    }
}