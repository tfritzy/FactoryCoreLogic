using FactoryCore;
using Newtonsoft.Json;

namespace FactoryCore
{
    public abstract class Building : Character
    {
        protected Building(World world) : base(world)
        {
        }
    }
}