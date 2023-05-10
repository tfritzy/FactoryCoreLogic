using System.Collections.Generic;

namespace Core
{
    public class Quarry : Building
    {
        public override CharacterType Type => CharacterType.Quarry;

        public Quarry(Context context) : base(context)
        {
        }

        public override Schema.Character ToSchema()
        {
            throw new System.NotImplementedException();
        }

        protected override void InitComponents()
        {
            this.Components = new Dictionary<System.Type, Component>
            {
                { typeof(Inventory), new Inventory(this, 3, 2) },
                { typeof(Worksite), new QuarryWorksite(this) },
            };
        }
    }
}