using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Sibz.Sentry.Lobby.Server
{
    [UpdateInGroup(typeof(ServerInitializationSystemGroup))]
    public class ServerInitSystem : ComponentSystem
    {
        protected override void OnCreate() => Enabled = true;

        protected override void OnUpdate()
        {
            ImportGhostCollection(Resources.Load<GameObject>("Collection"));
            Enabled = false;
        }

        private void ImportGhostCollection(GameObject prefab)
        {
            if (prefab.GetComponentInChildren<GhostCollectionAuthoringComponent>() is null)
            {
                throw new ArgumentException(
                    $"{nameof(ImportGhostCollection)}: Prefab should have {nameof(GhostCollectionAuthoringComponent)} attached.",
                    nameof(prefab));
            }

            var convertToEntitySystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>();
            var conversionSettings = new GameObjectConversionSettings(
                World,
                GameObjectConversionUtility.ConversionFlags.AssignName,
                convertToEntitySystem.BlobAssetStore);

            Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                prefab, conversionSettings);

            RemovePrefabComponentFromSelfAndChildren(entity);
        }

        private void RemovePrefabComponentFromSelfAndChildren(Entity entity)
        {
            EntityManager.RemoveComponent<Prefab>(entity);
            DynamicBuffer<LinkedEntityGroup> buff = EntityManager.GetBuffer<LinkedEntityGroup>(entity);
            foreach (LinkedEntityGroup item in buff)
            {
                RemoveIfHasPrefab(item.Value);
            }
        }

        private void RemoveIfHasPrefab(Entity entity)
        {
            if (EntityManager.HasComponent<Prefab>(entity))
            {
                EntityManager.RemoveComponent<Prefab>(entity);
            }
        }
    }
}