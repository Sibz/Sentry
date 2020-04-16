using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Client
{
    [UpdateInGroup(typeof(InitialisationGroup))]
    public class GoInGameSystem : ComponentSystem
    {
        protected override void OnCreate() => Enabled = true;

        protected override void OnUpdate()
        {
            Entities.WithNone<NetworkStreamInGame>()
                .ForEach((Entity ent, ref NetworkIdComponent id) =>
                {
                    PostUpdateCommands.AddComponent<NetworkStreamInGame>(ent);
                    var req = PostUpdateCommands.CreateEntity();
                    PostUpdateCommands.AddComponent<LobbyServerInGameRequest>(req);
                    PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
                });
        }
    }
}