using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core
{
    public abstract class Character : Entity, Schema.SerializesTo<Schema.Character>
    {
        public abstract CharacterType Type { get; }
        public Point2Int GridPosition { get; protected set; }
        public bool IsPreview { get; private set; }
        public Hex? ContainedBy { get; set; }

        public Character(Context context) : base(context)
        {
        }

        public virtual void Tick(float deltaTime)
        {
            foreach (var cell in Components.Values)
            {
                cell.Tick(deltaTime);
            }
        }

        public void SetGridPosition(Point2Int gridPosition)
        {
            this.GridPosition = gridPosition;
        }

        public virtual void OnAddToGrid(Point2Int gridPosition)
        {
            this.GridPosition = gridPosition;
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

        public void UpdateOwnerOfCells()
        {
            foreach (var cell in Components.Values)
            {
                cell.Owner = this;
            }
        }

        public static Character Create(CharacterType character, Context context)
        {
            switch (character)
            {
                case CharacterType.Dummy:
                    return new DummyBuilding(context);
                case CharacterType.Conveyor:
                    return new Conveyor(context);
                case CharacterType.Tree:
                    return new Tree(context);
                case CharacterType.Player:
                    return new Player(context);
                default:
                    throw new ArgumentException("Invalid character type " + character);
            }
        }

        protected Schema.Character PopulateSchema(Schema.Character character)
        {
            character.Id = this.Id;
            character.GridPosition = this.GridPosition;
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

        public void Destroy()
        {

        }
    }
}