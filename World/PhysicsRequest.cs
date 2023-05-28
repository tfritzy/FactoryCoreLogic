namespace Core
{
    public enum PhysicsRequestType
    {
        Sphere,
    }

    public abstract class PhysicsRequest
    {
        public abstract PhysicsRequestType Type { get; }
    }

    public class SpherePhysicsRequest : PhysicsRequest
    {
        public override PhysicsRequestType Type => PhysicsRequestType.Sphere;
        public Point3Float Location { get; private set; }
        public float Radius { get; private set; }

        public SpherePhysicsRequest(Point3Float location, float radius)
        {
            Location = location;
            Radius = radius;
        }
    }
}