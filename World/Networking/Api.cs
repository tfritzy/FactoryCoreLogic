namespace Core
{
    public abstract class Api
    {
        protected Context Context;
        public abstract void UpdateOwnPosition(ulong unitId, Point3Float location, Point3Float velocity);
        public abstract void SetItemObjectPos(ulong itemId, Point3Float pos, Point3Float rotation);

        public Api(Context context)
        {
            this.Context = context;
        }
    }
}