using System;
using Sibz.NetCode;
using Sibz.Sentry.Components;
using Sibz.Sentry.Lobby.Server.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Server
{
    [ServerSystem]
    public class MyCreateGameSystem : CreateGameSystem<CreateGameRequest, GameInfoComponent, CreateGameInfoJob>
    {
        protected override int GetPrefabIndex()
        {
            return SentryGhostSerializerCollection.FindGhostType<GameInfoSnapshotData>();
        }
    }

    public abstract class CreateGameSystem<TCreateGameRequest, TGameInfoComponent, TJobPart> : SystemBase
        where TCreateGameRequest : struct, IRpcCommand
        where TGameInfoComponent : struct, IComponentData
        where TJobPart : struct, ICreateGameInfoJob<TCreateGameRequest, TGameInfoComponent>
    {
        private Entity prefab;

        private EntityQuery required;
        private EntityQuery gameIdsQuery;
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

            Dependency = new GetNewGameIdsJob
            {
                Ids = newIds,
                GameIds = gameIdsQuery.ToComponentDataArrayAsync<GameIdComponent>(Allocator.TempJob, out JobHandle jh1)
            }.Schedule(JobHandle.CombineDependencies(Dependency, jh1));

            Dependency = new CreateGameJob<TCreateGameRequest, TGameInfoComponent, TJobPart>
            {
                Prefab = prefab,
                NewGameIds = newIds,
                CreateGameInfoJob = default,
                CreateGameRpcRequests =
                    required.ToComponentDataArrayAsync<TCreateGameRequest>(Allocator.TempJob, out JobHandle jh2),
                CommandBuffer = commandBuffer
            }.Schedule(required.CalculateEntityCount(), 8, JobHandle.CombineDependencies(Dependency, jh2));

            bufferSystem.CreateCommandBuffer().DestroyEntity(required);

            bufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}