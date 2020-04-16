using Unity.Entities;
using Unity.Jobs;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Server
{
    [UpdateInGroup(typeof(LobbyServerWorldSimGroup))]
    public class DisconnectSystem : JobComponentSystem
    {
        private BeginSimulationEntityCommandBufferSystem bufferSystem;

        protected override void OnCreate()
        {
            bufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var cmdBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent();
            inputDeps = Entities.WithAll<NetworkStreamDisconnected>().ForEach((Entity e, int entityInQueryIndex, ref CommandTargetComponent state) =>
            {
                if (state.targetEntity != Entity.Null)
                {
                    cmdBuffer.DestroyEntity(entityInQueryIndex, state.targetEntity);
                    state.targetEntity = Entity.Null;
                }
            }).Schedule(inputDeps);

            return inputDeps;
        }
    }
}