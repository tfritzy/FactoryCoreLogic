using System;
using System.Collections.Generic;

namespace Core
{
    public class Spawner : Component
    {
        public override ComponentType Type => ComponentType.Spawner;
        public int Range { get; private set; }
        public float Power { get; private set; }
        public float PowerAccrualRate { get; private set; }

        private List<Point3Int> spawnLocations = new List<Point3Int>();
        private Random r = new Random();

        public Spawner(Entity owner, int range, float powerAccrualRate, float power) : base(owner)
        {
            Range = range;
            PowerAccrualRate = powerAccrualRate;
            Power = power;
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.Spawner()
            {
                Range = Range,
                PowerAccrualRate = PowerAccrualRate,
                Power = Power,
            };
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            Power += deltaTime * PowerAccrualRate;
        }

        private void Spawn()
        {
            Point3Int location = spawnLocations[r.Next(spawnLocations.Count)];
            Mob dummy = (Mob)Character.Create(CharacterType.DummyMob, this.Owner.Context, alliance: 1);
            this.Owner.Context.World.AddCharacter(dummy);
            this.Power -= dummy.GetPower();
        }

        private List<Point3Int> FindSpawnLocations()
        {
            Keep? keep = this.Owner.Context.World.FindCharacter((Character c) => c is Keep) as Keep;

            if (keep == null)
            {
                return new List<Point3Int>();
            }

            return GridHelpers.BFS(
                this.Owner.Context.World,
                keep.GridPosition,
                (Point3Int p, int d) => true,
                Range
            );
        }
    }
}