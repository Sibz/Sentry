using System;
using Sibz.Lobby.Server.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.Lobby.Server
{
    public abstract class CreateGameSystem<TCreateGameRequest, TCreateGameInfoJob> : SystemBase
        where TCreateGameRequest : struct, IRpcCommand
        where TCreateGameInfoJob : struct, ICreateGameInfoJob<TCreateGameRequest>
    {
        private Entity prefab;
        private EntityQuery required;
        private EntityQuery gameIdsQuery;
        private EntityQuery networkConnectionsQuery;
        private EndSimulationEntityCommandBufferSystem bufferSystem;

        protected virtual int GetPrefabIndex()
        {
            return -1;
        }

        protected virtual Entity GetPrefab()
        {
            int index = GetPrefabIndex();
            if (index == -1)
            {
                throw new InvalidOperationException("When using ghost object prefab, must override GetPrefabIndex" +
                                                    " to provide the index of prefab in ghost collection\n" +
                                                    "i.e. GhostSerializerCollection.FindGhostType<SnapshotData>()");
            }

            GhostPrefabCollectionComponent prefabs =
                GetEntityQuery(typeof(GhostPrefabCollectionComponent)).GetSingleton<GhostPrefabCollectionComponent>();
            DynamicBuffer<GhostPrefabBuffer> serverPrefabs =
                EntityManager.GetBuffer<GhostPrefabBuffer>(prefabs.serverPrefabs);
            return serverPrefabs[GetPrefabIndex()].Value;
        }

        protected override void OnCreate()
        {
            bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            required = GetEntityQuery(new EntityQueryDesc()
            {
                All = new[]
                {
                    ComponentType.ReadOnly<TCreateGameRequest>(),
                    ComponentType.ReadOnly<ReceiveRpcCommandRequestComponent>()
                }
            });
            networkConnectionsQuery = GetEntityQuery(typeof(NetworkIdComponent), typeof(LobbyUser));
            RequireForUpdate(required);

            gameIdsQuery = GetEntityQuery(typeof(GameIdComponent));
        }

        protected override void OnStartRunning()
        {
            if (prefab != Entity.Null)
            {
                return;
            }

            prefab = GetPrefab();
            if (!EntityManager.HasComponent<GameIdComponent>(prefab))
            {
                EntityManager.AddComponent<GameIdComponent>(prefab);
            }
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent commandBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent();

            NativeArray<int> newIds = new NativeArray<int>(required.CalculateEntityCount(), Allocator.TempJob);
            NativeArray<int> userIds = new NativeArray<int>(required.CalculateEntityCount(), Allocator.TempJob);
                Dependency = new GetNewGameIdsJob
            {
                Ids = newIds,
                GameIds = gameIdsQuery.ToComponentDataArrayAsync<GameIdComponent>(Allocator.TempJob, out JobHandle jh1)
            }.Schedule(JobHandle.CombineDependencies(Dependency, jh1));



            Dependency = new GetUserIds
            {
                Requests = required.ToComponentDataArrayAsync<ReceiveRpcCommandRequestComponent>(Allocator.TempJob,
                    out JobHandle jh1B),
                ConnectionEntities = networkConnectionsQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle jh1C),
                LobbyUsers =
                    networkConnectionsQuery.ToComponentDataArrayAsync<LobbyUser>(Allocator.TempJob, out JobHandle jh1D),
                UserIds = userIds
            }.Schedule(required.CalculateEntityCount(), 4,
                JobHandle.CombineDependencies(Dependency, JobHandle.CombineDependencies(jh1B, jh1C, jh1D)));

            Dependency = new CreateGameJob<TCreateGameRequest, TCreateGameInfoJob>
            {
                Prefab = prefab,
                NewGameIds = newIds,
                CreateGameInfoJob = default,
                CreateGameRpcRequests =
                    required.ToComponentDataArrayAsync<TCreateGameRequest>(Allocator.TempJob, out JobHandle jh2),
                CommandBuffer = commandBuffer,
                UserIds = userIds
            }.Schedule(required.CalculateEntityCount(), 8, JobHandle.CombineDependencies(Dependency, jh2));

            bufferSystem.CreateCommandBuffer().DestroyEntity(required);

            bufferSystem.AddJobHandleForProducer(Dependency);
        }

        [BurstCompile]
        public struct GetUserIds : IJobParallelFor
        {
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<ReceiveRpcCommandRequestComponent> Requests;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> ConnectionEntities;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<LobbyUser> LobbyUsers;

            [WriteOnly] [NativeDisableParallelForRestriction]
            public NativeArray<int> UserIds;

            public void Execute(int index)
            {
                for (int i = 0; i < ConnectionEntities.Length; i++)
                {
                    if (!Requests[index].SourceConnection.Equals(ConnectionEntities[i]))
                    {
                        continue;
                    }

                    UserIds[index] = LobbyUsers[i].UserId;
                    return;
                }
            }
        }
    }
}