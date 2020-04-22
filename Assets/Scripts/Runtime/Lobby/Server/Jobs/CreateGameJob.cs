using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Server.Jobs
{
    [BurstCompile]
    public struct CreateGameJob<TCreateGameRequest, TJobPart> : IJobParallelFor
        where TCreateGameRequest : struct, IRpcCommand
        where TJobPart : struct, ICreateGameInfoJob<TCreateGameRequest>
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<TCreateGameRequest> CreateGameRpcRequests;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<int> NewGameIds;
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public TJobPart CreateGameInfoJob;
        public Entity Prefab;

        public void Execute(int index)
        {
            int gameId = NewGameIds[index];
            GameIdComponent gameIdComponent = new GameIdComponent { Id = gameId };

            Entity newGameEntity = CommandBuffer.Instantiate(index, Prefab);
            CreateGameInfoJob.SetGameInfoComponent(CommandBuffer, index, newGameEntity, CreateGameRpcRequests[index], gameId);
            CommandBuffer.SetComponent(index, newGameEntity, gameIdComponent);
        }
    }
}