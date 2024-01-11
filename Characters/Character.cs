using System;
using System.Linq;
using Schema;

namespace Core
{
    public abstract class Character : Entity
    {
        public abstract CharacterType Type { get; }
        public abstract Point3Int GridPosition { get; set; }
        public abstract Point3Float Location { get; }
        public bool IsPreview { get; private set; }
        public int Alliance { get; private set; }
        private static Point3Float defaultProjectileOffset = new Point3Float();
        public virtual Point3Float ProjectileSpawnOffset => defaultProjectileOffset;

        public Character(Schema.Character character, Context context) : base(character.Entity, context)
        {
            this.Alliance = character.Alliance;
            this.GridPosition = Point3Int.FromSchema(character.Pos);
        }

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
                case CharacterType.GuardTower:
                    return new GuardTower(context, alliance);
                case CharacterType.DummyMob:
                    return new DummyMob(context, alliance);
                case CharacterType.Pikeman:
                    return new Pikeman(context, alliance);
                case CharacterType.Keep:
                    return new Keep(context, alliance);
                case CharacterType.MineShaft:
                    return new Mineshaft(context, alliance);
                case CharacterType.Depot:
                    return new Depot(context, alliance);
                case CharacterType.Sorter:
                    return new Sorter(context, alliance);
                case CharacterType.ClayFurnace:
                    return new ClayFurnace(context, alliance);
                default:
                    throw new ArgumentException("Invalid character type " + character);
            }
        }

        public static Character FromSchema(Context context, OneofCharacter character)
        {
            if (character.Conveyor != null)
                return new Conveyor(context, character.Conveyor);
            else if (character.Dummy != null)
                return new Dummy(context, character.Dummy);
            else if (character.DummyBuilding != null)
                return new DummyBuilding(context, character.DummyBuilding);
            else if (character.Player != null)
                return new Player(context, character.Player);
            else if (character.GuardTower != null)
                return new GuardTower(context, character.GuardTower);
            else if (character.Pikeman != null)
                return new Pikeman(context, character.Pikeman);
            else if (character.DummyMob != null)
                return new DummyMob(context, character.DummyMob);
            else if (character.Keep != null)
                return new Keep(context, character.Keep);
            else if (character.Mineshaft != null)
                return new Mineshaft(context, character.Mineshaft);
            else if (character.Depot != null)
                return new Depot(context, character.Depot);
            else if (character.Sorter != null)
                return new Sorter(context, character.Sorter);
            else if (character.ClayFurnace != null)
                return new ClayFurnace(context, character.ClayFurnace);
            else
                throw new ArgumentException("Invalid character type " + character);

        }

        public abstract OneofCharacter Serialize();

        public new Schema.Character ToSchema()
        {
            return new Schema.Character()
            {
                Alliance = this.Alliance,
                Pos = this.GridPosition.ToSchema(),
                Type = this.Type,
                Entity = base.ToSchema(),
            };
        }

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

        public override void Destroy()
        {
            Context.World.RemoveCharacter(this.Id);
        }
    }
}