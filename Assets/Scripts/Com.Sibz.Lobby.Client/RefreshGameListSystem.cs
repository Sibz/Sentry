using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Sibz.Lobby.Client
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public abstract class RefreshGameListSystem <TGameInfoComponent> : SystemBase
        where TGameInfoComponent: struct, IComponentData
    {
        public NativeArray<TGameInfoComponent> GameInfoComponents;
        public Action GameListUpdated;
        private EntityQuery gameInfoQuery;

        protected override void OnCreate()
        {
            GameInfoComponents = new NativeArray<TGameInfoComponent>(0, Allocator.Persistent);
            gameInfoQuery = GetEntityQuery(ComponentType.ReadOnly<TGameInfoComponent>());
        }

        protected override void OnDestroy()
        {
            DisposeGameInfoComponents();
        }

        protected override void OnStopRunning()
        {
            DisposeGameInfoComponents();
            GameInfoComponents = new NativeArray<TGameInfoComponent>(0,Allocator.Persistent);
            GameListUpdated?.Invoke();
        }

        protected override void OnUpdate()
        {
            if (UnityEngine.Time.frameCount % 30 != 0)
            {
                return;
            }

            DisposeGameInfoComponents();
            GameInfoComponents = gameInfoQuery.ToComponentDataArrayAsync<TGameInfoComponent>(Allocator.Persistent, out JobHandle jh);
            jh.Complete();
            GameListUpdated?.Invoke();
        }

        private void DisposeGameInfoComponents()
        {
            if (GameInfoComponents.IsCreated)
            {
                GameInfoComponents.Dispose();
            }
        }
    }
}