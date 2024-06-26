using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Schema;

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
                    ItemPort!.OnlyLetItemThroughSide(_filter.Value, 1);
                }
                else
                {
                    ItemPort!.RemoveAllFilters();
                }
            }
        }

        public Sorter(Context context, Schema.Sorter sorter) : base(sorter.Building, context)
        {
            Filter = sorter.Filter?.ItemType;
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
            Filter = _filter; // Refreshes the ItemPort's filters;
        }

        public override Schema.OneofCharacter Serialize()
        {
            return new Schema.OneofCharacter
            {
                Sorter = new Schema.Sorter()
                {
                    Building = base.ToSchema(),
                    Filter = Filter != null ? new NullableItemType { ItemType = Filter.Value } : null,
                }
            };
        }
    }
}