
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace Sibz.Sentry.Lobby.Client
{
    [UpdateInGroup(typeof(InitialisationGroup))]
    [DisableAutoCreation]
    public class ConnectSystem : ComponentSystem
    {
        public bool IsConnected => networkStreamInGameQuery.CalculateEntityCount() > 0;

        private EntityQuery networkStreamInGameQuery;
        protected override void OnCreate()
        {
            networkStreamInGameQuery = GetEntityQuery(new EntityQueryDesc
            {
                Any  = new []
                {
                    ComponentType.ReadOnly<NetworkStreamInGame>(),
                }
            });
            RequireSingletonForUpdate<Connect>();
        }

        protected override void OnUpdate()
        {
            EntityManager.DestroyEntity(GetSingletonEntity<Connect>());
            NetworkStreamReceiveSystem network = World.GetExistingSystem<NetworkStreamReceiveSystem>();
            if (network == null || World.GetExistingSystem<ClientSimulationSystemGroup>() == null)
            {
                return;
            }

            NetworkEndPoint ep = NetworkEndPoint.LoopbackIpv4;
            ep.Port = 21650;
            network.Connect(ep);
            Debug.Log("Connecting to LobbyWorld...");
        }
    }
}