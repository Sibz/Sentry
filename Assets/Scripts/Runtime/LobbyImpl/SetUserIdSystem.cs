using Sibz.Lobby;
using Sibz.Lobby.Server;
using Sibz.NetCode.Server;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sibz.Sentry.Lobby
{
    [LobbyServerSystem]
    [UpdateAfter(typeof(AddLobbyUserComponentSystem))]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SetUserIdSystem : SystemBase
    {
        private int lastUserId = 1;
        private EntityQuery clientConnectedEventQuery;
        private EntityQuery lobbyAclBufferItemQuery;
        private bool haveSetAcl;

        protected override void OnCreate()
        {
            clientConnectedEventQuery = GetEntityQuery(ComponentType.ReadOnly<ClientConnectedEvent>());
            lobbyAclBufferItemQuery = GetEntityQuery(ComponentType.ReadOnly<LobbyAclBufferItem>());
            RequireForUpdate(clientConnectedEventQuery);
        }

        protected override void OnUpdate()
        {
            if (!haveSetAcl && lobbyAclBufferItemQuery.CalculateEntityCount() > 0)
            {
                GetBufferFromEntity<LobbyAclBufferItem>()[lobbyAclBufferItemQuery.GetSingletonEntity()]
                    .Add(new LobbyAclBufferItem(5, true));
                haveSetAcl = true;
            }

            Debug.Log($"Setting user id {lastUserId}");
            NativeArray<int> userIds =
                new NativeArray<int>(clientConnectedEventQuery.CalculateChunkCount(), Allocator.TempJob);

            for (int i = 0; i < userIds.Length; i++)
            {
                userIds[i] = lastUserId++;
            }

            Dependency = Entities.WithDeallocateOnJobCompletion(userIds)
                .ForEach((int entityInQueryIndex, ref ClientConnectedEvent clientConnectedEvent) =>
            {
                LobbyUser gameIdComponent =
                    GetComponent<LobbyUser>(clientConnectedEvent.ConnectionEntity);
                gameIdComponent.UserId = userIds[entityInQueryIndex];
                SetComponent(clientConnectedEvent.ConnectionEntity, gameIdComponent);
            }).Schedule(Dependency);
        }
    }
}