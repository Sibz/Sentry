﻿using Sibz.NetCode;
 using Sibz.Sentry.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.Sentry.Lobby.Server
{
    //[ServerSystem]
    public class CreateGameSystem : JobComponentSystem
    {
        private Entity prefab;

        private EntityQuery required;
        private EntityQuery gamesQuery;
        private EndSimulationEntityCommandBufferSystem bufferSystem;

        public struct MyJoby
        {
            public EntityCommandBuffer.Concurrent Buffer;
            public Entity Prefab;
            public NativeArray<int> GameIds;

            public static void Execute(MyJoby joby, Entity entity, int index, ref CreateGameRequest rpc,
            ref ReceiveRpcCommandRequestComponent req)
            {
                var e = joby.Buffer.Instantiate(index, joby.Prefab);
                joby.Buffer.SetComponent(index, e, new GameInfoComponent { Id = joby.GameIds[index], Name = rpc.Name, SizeX = rpc.Size.x, SizeY = rpc.Size.y});
                joby.Buffer.DestroyEntity(index, entity);
            }
        }

        protected override void OnCreate()
        {
            bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            required = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<CreateGameRequest>(),
                    ComponentType.ReadOnly<ReceiveRpcCommandRequestComponent>()
                }
            });
            RequireForUpdate(required);

            gamesQuery = GetEntityQuery(typeof(GameInfoComponent));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!HasSingleton<GhostPrefabCollectionComponent>())
            {
                return inputDeps;
            }
            if (prefab == Entity.Null)
            {
                var prefabs = GetSingleton<GhostPrefabCollectionComponent>();
                var serverPrefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(prefabs.serverPrefabs);
                prefab = serverPrefabs[SentryGhostSerializerCollection.FindGhostType<GameInfoSnapshotData>()].Value;
            }

            var commandBuffer = bufferSystem.CreateCommandBuffer()
                .ToConcurrent();

            var newIds = new NativeArray<int>(required.CalculateEntityCount(), Allocator.TempJob);

            inputDeps = new GetNewGameIds
            {
                Ids = newIds,
                Infos = gamesQuery.ToComponentDataArrayAsync<GameInfoComponent>(Allocator.TempJob, out JobHandle jh1)
            }.Schedule(JobHandle.CombineDependencies(inputDeps, jh1));

            MyJoby jobInfo = new MyJoby
            {
                Buffer = commandBuffer,
                Prefab = prefab,
                GameIds = newIds
            };

            Debug.Log($"Server: Creating new game...");

            inputDeps = Entities.ForEach((Entity entity, int entityInQueryIndex, ref CreateGameRequest rpc,
                ref ReceiveRpcCommandRequestComponent req) =>
            {
                MyJoby.Execute(jobInfo, entity, entityInQueryIndex, ref rpc, ref req);
            }).Schedule(inputDeps);

            inputDeps = new Dealloc {Ids = newIds}.Schedule(inputDeps);

            bufferSystem.AddJobHandleForProducer(inputDeps);

            return inputDeps;
        }

        public struct GetNewGameIds : IJob
        {
            public NativeArray<int> Ids;
            [DeallocateOnJobCompletion]
            public NativeArray<GameInfoComponent> Infos;
            public void Execute()
            {
                for (int i = 0; i < Ids.Length ; i++)
                {
                    for (int newId = 0; newId < int.MaxValue; newId++)
                    {
                        bool inUse = false;
                        for (int j = 0; j < Infos.Length; j++)
                        {
                            if (Infos[j].Id != newId)
                            {
                                continue;
                            }

                            inUse = true;
                            break;
                        }

                        if (inUse)
                        {
                            continue;
                        }

                        Ids[i] = newId;
                        break;
                    }
                }
            }
        }

        public struct Dealloc : IJob
        {
             [DeallocateOnJobCompletion]
             public NativeArray<int> Ids;

             public void Execute()
             {

             }
        }
    }
}