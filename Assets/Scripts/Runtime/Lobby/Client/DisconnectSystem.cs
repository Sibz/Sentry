using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry.Lobby.Client
{
    [UpdateInGroup(typeof(InitialisationGroup))]
    [DisableAutoCreation]
    public class DisconnectSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<Disconnect>();
        }

        protected override void OnUpdate()
        {
            EntityManager.DestroyEntity(GetSingletonEntity<Disconnect>());
            if (!HasSingleton<NetworkStreamDisconnected>())
            {
                EntityManager.AddComponent<NetworkStreamDisconnected>(GetSingletonEntity<NetworkStreamInGame>());
            }
        }
    }
}