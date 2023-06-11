using System;
using System.Collections.Generic;

namespace Core
{
    public class Spawner : Component
    {
        public new Character Owner => (Character)base.Owner;
        public override ComponentType Type => ComponentType.Spawner;
        public int Range { get; private set; }
        public float Power { get; private set; }
        public float PowerAccrualRate { get; private set; }
        public List<CharacterType> SpawnableTypes { get; private set; }

        private List<Point3Int> spawnLocations = new List<Point3Int>();
        private Queue<CharacterType> spawnQueue = new Queue<CharacterType>();
        private Random r = new Random();
        private Dictionary<CharacterType, float> powerMap = new Dictionary<CharacterType, float>();

        public Spawner(
            Entity owner,
            int range,
            float power,
            float powerAccrualRate,
            List<CharacterType> spawnableTypes) : base(owner)
        {
            Range = range;
            PowerAccrualRate = powerAccrualRate;
            Power = power;
            SpawnableTypes = spawnableTypes;
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.Spawner()
            {
                Range = Range,
                PowerAccrualRate = PowerAccrualRate,
                Power = Power,
                SpawnableTypes = SpawnableTypes,
            };
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            Power += deltaTime * PowerAccrualRate;
            Spawn();
        }

        private void Spawn()
        {
            if (spawnQueue.Count == 0)
            {
                PopulateSpawnQueue();
            }

            if (spawnLocations.Count == 0)
            {
                FindSpawnLocations();
            }

            if (spawnQueue.Count == 0 || spawnLocations.Count == 0)
            {
                return;
            }

            if (powerMap[spawnQueue.Peek()] > Power)
            {
                return;
            }

            Point3Int location = spawnLocations[r.Next(spawnLocations.Count)];
            Mob toSpawn = (Mob)Character.Create(
                spawnQueue.Dequeue(),
                this.Owner.Context,
                alliance: this.Owner.Alliance
            );
            toSpawn.SetLocation(GridHelpers.oddq_offset_to_pixel(location));
            this.Owner.Context.World.AddCharacter(toSpawn);
            this.Power -= toSpawn.GetPower();
        }

        private void PopulateSpawnQueue()
        {
            List<CharacterType> reasonableToSpawn = new List<CharacterType>();
            for (int i = 0; i < SpawnableTypes.Count; i++)
            {
                Character c = Character.Create(SpawnableTypes[i], this.Owner.Context, alliance: 1);
                if (!(c is Mob))
                {
                    SpawnableTypes.RemoveAt(i);
                    i--;
                    continue;
                }
                Mob mob = (Mob)c;

                float secondsToAfford = mob.GetPower() / PowerAccrualRate;
                if (secondsToAfford > .5f && secondsToAfford < 10f)
                {
                    reasonableToSpawn.Add(SpawnableTypes[i]);
                }

                powerMap[SpawnableTypes[i]] = mob.GetPower();
            }

            if (reasonableToSpawn.Count == 0)
            {
                return;
            }

            this.spawnQueue = new Queue<CharacterType>();
            for (int i = 0; i < 30; i++)
            {
                this.spawnQueue.Enqueue(reasonableToSpawn[r.Next(reasonableToSpawn.Count)]);
            }
        }

        private void FindSpawnLocations()
        {
            Keep? keep = this.Owner.Context.World.FindCharacter((Character c) => c is Keep) as Keep;

            if (keep == null)
            {
                this.spawnLocations = new List<Point3Int>();
                return;
            }

            this.spawnLocations = GridHelpers.BFS(
                this.Owner.Context.World,
                keep.GridPosition,
                (Point3Int p, int d) => d == Range,
                Range
            );
        }
    }
}