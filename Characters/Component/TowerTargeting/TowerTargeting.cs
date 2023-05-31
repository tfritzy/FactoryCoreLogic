using System.Collections.Generic;

namespace Core
{
    public class TowerTargeting : Component
    {
        public override ComponentType Type => ComponentType.TowerTargeting;
        public TowerTargetingMode Mode { get; private set; } = TowerTargetingMode.Closest;
        public PhysicsRequest? PhysicsRequest { get; private set; }
        public Character? Target { get; private set; }
        public const float TARGET_CHECK_COOLDOWN = 1f;
        protected new Character Owner => (Character)base.Owner;

        public override Schema.Component ToSchema()
        {
            return new Schema.TowerTargeting
            {
                Mode = Mode,
            };
        }

        public TowerTargeting(Entity owner) : base(owner)
        {
        }

        public void SetMode(TowerTargetingMode mode)
        {
            Mode = mode;
        }

        private float timeSinceLastTargetCheck = 0f;
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            timeSinceLastTargetCheck += deltaTime;

            if (timeSinceLastTargetCheck > TARGET_CHECK_COOLDOWN)
            {
                PhysicsRequest = new SpherePhysicsRequest(
                    Owner.Location,
                    Owner.GetComponent<Attack>().Range);
            }
        }

        public void FulfillPhysicsRequest(List<Character> inRange)
        {
            PhysicsRequest = null;
        }
    }
}