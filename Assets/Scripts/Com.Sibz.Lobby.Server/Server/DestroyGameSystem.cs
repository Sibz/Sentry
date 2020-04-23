using Sibz.Lobby.Requests;
using Sibz.Lobby.Server.Jobs;
using Sibz.NetCode;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.Lobby.Server
{
    [ServerSystem]
    public class DestroyGameSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem bufferSystem;

        private EntityQuery requiredToRunQuery;
        private EntityQuery gameInfosQuery;

        protected override void OnCreate()
        {
            bufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            requiredToRunQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new []
                    {
                        ComponentType.ReadOnly<DestroyGameRequest>(),
                        ComponentType.ReadOnly<ReceiveRpcCommandRequestComponent>(),
                    }
                });
            RequireForUpdate(requiredToRunQuery);
            gameInfosQuery = GetEntityQuery(typeof(GameIdComponent));
        }

        protected override void OnUpdate()
        {
            DynamicBuffer<LobbyAclBufferItem> aclBuffer = GetBufferFromEntity<LobbyAclBufferItem>(true)[GetSingletonEntity<LobbyAclBufferItem>()];
            DestroyGameJob jobData = new DestroyGameJob
            {
                CmdBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent(),
                GameEntities = gameInfosQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle jh1),
                GameIds = gameInfosQuery.ToComponentDataArrayAsync<GameIdComponent>(Allocator.TempJob,
                    out JobHandle jh2),
                AclBuffer = aclBuffer
            };

            Dependency = JobHandle.CombineDependencies(Dependency, jh1, jh2);


            Dependency = Entities
                .ForEach((Entity entity, int entityInQueryIndex, ref DestroyGameRequest rpc, ref ReceiveRpcCommandRequestComponent requestComponent) =>
                {
                    jobData.Execute(entity, entityInQueryIndex, ref rpc, GetComponent<LobbyUser>(requestComponent.SourceConnection));
                }).Schedule(Dependency);

            bufferSystem.AddJobHandleForProducer(Dependency);

            Dependency =
                new Dealloc()
                    { GameEntities = jobData.GameEntities, GameInfos = jobData.GameIds }.Schedule(Dependency);
        }

        public struct Dealloc : IJob
        {
            [DeallocateOnJobCompletion] public NativeArray<Entity> GameEntities;
            [DeallocateOnJobCompletion] public NativeArray<GameIdComponent> GameInfos;

            public void Execute()
            {
            }
        }
    }
}