using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Core
{
    public class Sorter : Building
    {
        public override int Height => 1;
        public override CharacterType Type => CharacterType.Sorter;
        public override string Name => "Sorter";
        private ItemType? _filter;
        public ItemType? Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                if (_filter != null)
                {
                    ItemPort!.SideOffsetToFilter = new Dictionary<int, ItemType> { { 0, _filter.Value } };
                }
                else
                {
                    ItemPort!.SideOffsetToFilter = new Dictionary<int, ItemType>();
                }
            }
        }

        public Sorter(Context context, int alliance) : base(context, alliance)
        {
        }

        protected override void InitComponents()
        {
            base.InitComponents();
            SetComponent(new ItemPort(this));
        }

        public override void ConfigureComponents()
        {
            base.ConfigureComponents();
            ItemPort!.OutputSideOffsets = new List<int> { 0, 1 };
            ItemPort!.InputSideOffsets = new List<int> { 3 };

            if (Filter != null)
            {
                ItemPort!.SideOffsetToFilter = new Dictionary<int, ItemType> { { 0, Filter.Value } };
            }
        }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Sorter()
            {
                Filter = Filter
            };
        }
    }
}