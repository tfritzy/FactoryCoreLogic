using System;
using System.Collections.Generic;

namespace Core
{
    public abstract class Worksite : Component
    {
        public override ComponentType Type => ComponentType.Worksite;
        public abstract int MaxEmployable { get; }
        protected abstract List<Point2Int> GetEligibleHex();
        protected abstract Point2Int GetStartPoint();
        protected abstract Harvestable? GetHarvestable(World world, Point2Int point2Int);
        protected abstract void InitInRangeHex();
        private List<List<Harvestable>> eligibleHarvestables = new List<List<Harvestable>>();
        private List<Point2Int> hexInRange = new List<Point2Int>();
        private bool initialized = false;
        public int Employed => this.World.Villagers.FindAll(
            v => World.GetCharacter(v)?.GetComponent<VillagerBehavior>().PlaceOfEmployment == this).Count;

        public Worksite(Entity owner) : base(owner) { }

        public override Schema.Component ToSchema()
        {
            throw new System.NotImplementedException();
        }

        public override void OnAddToGrid()
        {
            base.OnAddToGrid();
            InitInRangeHex();
            initialized = true;
        }

        public void RefreshEligible()
        {
            this.eligibleHarvestables = new List<List<Harvestable>>();
            HashSet<Point2Int> inRangeHex = new HashSet<Point2Int>(GetEligibleHex());
            Queue<Tuple<Point2Int, int>> queue = new Queue<Tuple<Point2Int, int>>();
            HashSet<Point2Int> v = new HashSet<Point2Int>();

            queue.Enqueue(Tuple.Create(GetStartPoint(), 0));

            while (queue.Count > 0)
            {
                Tuple<Point2Int, int> current = queue.Dequeue();

                if (v.Contains(current.Item1))
                {
                    continue;
                }

                v.Add(current.Item1);

                while (eligibleHarvestables.Count <= current.Item2)
                {
                    eligibleHarvestables.Add(new List<Harvestable>());
                }

                Harvestable? harvestable = GetHarvestable(this.World, current.Item1);
                if (harvestable != null)
                {
                    eligibleHarvestables[current.Item2].Add(harvestable);
                }

                for (int i = 0; i < 6; i++)
                {
                    Point2Int neighborPos = GridHelpers.GetNeighbor(current.Item1, (HexSide)i);
                    if (!v.Contains(neighborPos) && inRangeHex.Contains(neighborPos))
                    {
                        queue.Enqueue(Tuple.Create(neighborPos, current.Item2 + 1));
                    }
                }
            }
        }

        private Harvestable? FindClosestHarvestable()
        {
            if (!initialized)
            {
                throw new InvalidOperationException("Should not be trying to use a Worksite before it has been initialized.");
            }

            for (int d = 0; d < this.eligibleHarvestables.Count; d++)
            {
                for (int i = 0; i < this.eligibleHarvestables[d].Count; i++)
                {
                    if (this.eligibleHarvestables[d][i].IsDepleted)
                    {
                        this.eligibleHarvestables[d].RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        return this.eligibleHarvestables[d][i];
                    }
                }
            }

            return null;
        }

        public Harvestable? GetNextHarvestable()
        {
            Harvestable? closest = FindClosestHarvestable();

            if (closest == null)
            {
                RefreshEligible();
                closest = FindClosestHarvestable();
            }

            return closest;
        }

        private void RemoveNonExistantVillagers()
        {
            for (int i = 0; i < this.World.Villagers.Count; i++)
            {
                ulong id = this.World.Villagers[i];

                if (this.World.GetCharacter(id) == null)
                {
                    this.World.Villagers.RemoveAt(i);
                    id--;
                }
            }
        }

        public void IncrementEmployed()
        {
            if (Employed >= this.MaxEmployable)
            {
                throw new System.InvalidOperationException("Max employed reached");
            }

            RemoveNonExistantVillagers();

            var unemployedVillagers =
                this.World.Villagers.FindAll(
                    v => World.GetCharacter(v)?.GetComponent<VillagerBehavior>().PlaceOfEmployment == null);

            if (unemployedVillagers.Count == 0)
            {
                throw new System.InvalidOperationException("No unemployed villagers");
            }

            Villager? villager = (Villager?)this.World.GetCharacter(unemployedVillagers[0]);

            if (villager == null)
            {
                throw new System.InvalidOperationException("villager should not be null here.");
            }

            villager?.Behavior.SetPlaceOfEmployment(this);
        }

        public void DecrementEmployed()
        {
            RemoveNonExistantVillagers();

            var employedVillagers =
                this.World.Villagers.FindAll(
                    v => World.GetCharacter(v)?.GetComponent<VillagerBehavior>().PlaceOfEmployment == this);

            if (employedVillagers.Count <= 0)
            {
                throw new System.InvalidOperationException("No employed villagers");
            }

            Villager? gettingTheCan = (Villager?)this.World.GetCharacter(employedVillagers[0]);

            if (gettingTheCan == null)
            {
                throw new System.InvalidOperationException("villager should not be null here.");
            }

            gettingTheCan.Behavior.SetPlaceOfEmployment(null);
        }
    }
}