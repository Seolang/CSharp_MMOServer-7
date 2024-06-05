using Google.Protobuf.Protocol;
using Server.GameRepository.Room;

namespace Server.GameRepository.Object
{
    public class Player : GameObject
    {
        public ClientSession Session { get; set; }

        public Player()
        {
            ObjectType = GameObjectType.Player;
        }
    }
}
