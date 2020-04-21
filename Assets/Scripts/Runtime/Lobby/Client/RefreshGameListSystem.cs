using System;
using Sibz.NetCode;
using Sibz.Sentry.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry.Client
{
    [ClientSystem]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class RefreshGameListSystem : SystemBase
    {
        public NativeList<GameInfoComponent> GameInfoComponents;
        public Action GameListUpdated;

        protected override void OnCreate()
        {
            GameInfoComponents = new NativeList<GameInfoComponent>(Allocator.Persistent);
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            GameInfoComponents.Dispose();
        }

        protected override void OnStopRunning()
        {
            GameInfoComponents.Clear();
            GameListUpdated?.Invoke();
        }

        protected override void OnUpdate()
        {
            if (UnityEngine.Time.frameCount % 30 != 0)
            {
                return;
            }
            GameInfoComponents.Clear();
            NativeList<GameInfoComponent> gameInfoComponents = GameInfoComponents;
            Entities.ForEach((ref GameInfoComponent gameInfoComponent) =>
            {
                gameInfoComponents.Add(gameInfoComponent);
            }).Schedule(Dependency).Complete();
            GameListUpdated?.Invoke();
        }
    }
}