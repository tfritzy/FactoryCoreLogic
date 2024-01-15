using Google.Protobuf;

namespace Core
{
    public class ClientApi : Api
    {
        private Context context;
        private ClientConnection clientConnection => (ClientConnection)context.Connection;

        public ClientApi(Context context) : base(context)
        {
            this.context = context;
        }

        public override void UpdateOwnPosition(ulong unitId, Point3Float pos, Point3Float velocity)
        {
            var update = new Schema.OneofRequest
            {
                UpdateOwnLocation = new Schema.UpdateOwnLocation
                {
                    PlayerId = unitId,
                    Position = pos.ToSchema(),
                    Velocity = velocity.ToSchema(),
                    Type = Schema.RequestType.UpdateOwnLocation,
                }
            };
            clientConnection.SendMessage(update);
        }

        public override void SetItemObjectPos(ulong itemId, Point3Float pos, Point3Float rotation)
        {
            context.World.SetItemObjectPos(itemId, pos, rotation);
        }
    }
}