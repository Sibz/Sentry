﻿using Sibz.Lobby.Requests;
 using Sibz.NetCode;
 using Sibz.Sentry.Components;
using Sibz.Sentry.Lobby.Server.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Server
{
    //    [ServerSystem]
    public class DestroyGameSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem bufferSystem;

        private EntityQuery requiredToRunQuery;
        private EntityQuery gameInfosQuery;

        protected override void OnCreate()
        {
            bufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            requiredToRunQuery = GetEntityQuery(typeof(DestroyGameRequest), typeof(ReceiveRpcCommandRequestComponent));
            RequireForUpdate(requiredToRunQuery);
            gameInfosQuery = GetEntityQuery(typeof(GameInfoComponent));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            DestroyGameJob jobData = new DestroyGameJob
            {
                CmdBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent(),
                GameEntities = gameInfosQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle jh1),
                GameInfos = gameInfosQuery.ToComponentDataArrayAsync<GameInfoComponent>(Allocator.TempJob,
                    out JobHandle jh2)
            };

            inputDeps = JobHandle.CombineDependencies(inputDeps, jh1, jh2);

            inputDeps = Entities
                .WithAll<ReceiveRpcCommandRequestComponent>()
                .ForEach((Entity entity, int entityInQueryIndex, ref DestroyGameRequest rpc) =>
                {
                    jobData.Execute(entity, entityInQueryIndex, ref rpc);
                }).Schedule(inputDeps);

            bufferSystem.AddJobHandleForProducer(inputDeps);

            inputDeps = new Dealloc() {GameEntities = jobData.GameEntities, GameInfos = jobData.GameInfos}.Schedule(inputDeps);

            return inputDeps;
        }

        public struct Dealloc : IJob
        {
            [DeallocateOnJobCompletion] public NativeArray<Entity> GameEntities;
            [DeallocateOnJobCompletion] public NativeArray<GameInfoComponent> GameInfos;
            public void Execute()
            {

            }
        }
    }
}