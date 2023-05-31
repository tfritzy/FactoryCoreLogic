using Schema;

namespace Core
{
    public abstract class Vegetation : Entity
    {
        public abstract VegetationType Type { get; }

        protected Vegetation(Context context) : base(context)
        {
        }
    }
}