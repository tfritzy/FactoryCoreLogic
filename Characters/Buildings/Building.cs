using Core;
using Newtonsoft.Json;

namespace Core
{
    public abstract class Building : Character
    {
        public virtual int MaxEmployable => 0;

        public int Employed => this.World.Villigers.FindAll(
            v => World.GetCharacter(v).GetComponent<VilligerBehavior>().PlaceOfEmployment == this).Count;

        protected Building(Context context) : base(context)
        {
        }

        public void IncrementEmployed()
        {
            if (Employed >= this.MaxEmployable)
            {
                throw new System.InvalidOperationException("Max employed reached");
            }

            var unemployedVilligers =
                this.World.Villigers.FindAll(
                    v => World.GetCharacter(v).GetComponent<VilligerBehavior>().PlaceOfEmployment == null);

            if (unemployedVilligers.Count == 0)
            {
                throw new System.InvalidOperationException("No unemployed villigers");
            }

            Villiger villiger = (Villiger)this.World.GetCharacter(unemployedVilligers[0]);
            villiger.Behavior.SetPlaceOfEmployment(this);
        }

        public void DecrementEmployed()
        {
            var employedVilligers =
                this.World.Villigers.FindAll(
                    v => World.GetCharacter(v).GetComponent<VilligerBehavior>().PlaceOfEmployment == this);

            if (employedVilligers.Count <= 0)
            {
                throw new System.InvalidOperationException("No employed villigers");
            }

            Villiger gettingTheCan = (Villiger)this.World.GetCharacter(employedVilligers[0]);
            gettingTheCan.Behavior.SetPlaceOfEmployment(null);
        }
    }
}