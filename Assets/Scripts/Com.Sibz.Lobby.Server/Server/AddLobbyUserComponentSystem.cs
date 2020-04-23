using Sibz.NetCode.Server;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Lobby.Server
{
    [LobbyServerSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class AddLobbyUserComponentSystem : SystemBase
    {
        private EntityQuery clientConnectedEventQuery;
        private EntityQuery nonUserNetworkConnectionsQuery;
        protected override void OnCreate()
        {
            clientConnectedEventQuery = GetEntityQuery(ComponentType.ReadOnly<ClientConnectedEvent>());
            nonUserNetworkConnectionsQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadOnly<NetworkIdComponent>(), },
                None = new[] { ComponentType.ReadOnly<LobbyUser>(), }
            });
            RequireForUpdate(clientConnectedEventQuery);
        }

        protected override void OnUpdate()
        {
            EntityManager.AddComponent<LobbyUser>(nonUserNetworkConnectionsQuery);
        }
    }
}