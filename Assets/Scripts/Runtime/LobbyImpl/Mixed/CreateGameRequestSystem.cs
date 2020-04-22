using Sibz.Lobby.Requests;
using Sibz.NetCode;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby
{
    [ClientAndServerSystem]
    public class CreateGameRequestSystem : RpcCommandRequestSystem<CreateGameRequest>
    {
    }

}