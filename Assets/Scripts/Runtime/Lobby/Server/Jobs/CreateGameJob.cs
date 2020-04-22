using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    [BurstCompile]
    public struct CreateGameJob<TCreateGameRequest, TGameInfoComponent, TJobPart> : IJobParallelFor
        where TCreateGameRequest : struct, IRpcCommand
        where TGameInfoComponent : struct, IComponentData
        where TJobPart : struct, ICreateGameInfoJob<TCreateGameRequest, TGameInfoComponent>
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<TCreateGameRequest> CreateGameRpcRequests;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<int> NewGameIds;
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public TJobPart CreateGameInfoJob;
        public Entity Prefab;

        public void Execute(int index)
        {
            int gameId = NewGameIds[index];
            TGameInfoComponent gameInfoComponent =
                CreateGameInfoJob.ConvertRequestToComponent(CreateGameRpcRequests[index], gameId);
            GameIdComponent gameIdComponent = new GameIdComponent { Id = gameId };

            Entity newGameEntity = CommandBuffer.Instantiate(index, Prefab);
            CommandBuffer.SetComponent(index, newGameEntity, gameInfoComponent);
            CommandBuffer.SetComponent(index, newGameEntity, gameIdComponent);
        }
    }
}