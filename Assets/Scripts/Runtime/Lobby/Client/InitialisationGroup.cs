using System;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sibz.Sentry.Lobby.Client
{
    [DisableAutoCreation]
    public class InitialisationGroup : ComponentSystemGroup
    {
        private bool created;
        protected override void OnCreate()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type system in types.Where(x=>x.GetCustomAttributes<UpdateInGroupAttribute>().Any(y=>y.GroupType==typeof(InitialisationGroup))))
            {
                AddSystemToUpdateList(World.CreateSystem(system));
            }
            SortSystemUpdateList();

            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (!created)
            {
                Debug.Log("Converting");
                World.ImportGhostCollection(Resources.Load<GameObject>("Collection"));
                //ImportGhostCollection(Resources.Load<GameObject>("Collection"));
                //Object.Instantiate(GameObject.FindWithTag("ClientPrefab").GetComponent<ClientPrefab>().Prefab);
                /*GameObjectConversionUtility.ConvertGameObjectHierarchy(
                    GameObject.FindWithTag("ClientPrefab").GetComponent<ClientPrefab>().Prefab,
                    new GameObjectConversionSettings(World, GameObjectConversionUtility.ConversionFlags.AssignName, new BlobAssetStore()));*/

                    /*var convertToEntitySystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>();
                    var prefab = Resources.Load<GameObject>("ClientSharedData");
                    var conversionSettings = new GameObjectConversionSettings(
                        World,
                        GameObjectConversionUtility.ConversionFlags.AssignName,
                        convertToEntitySystem.BlobAssetStore);
                    //convertToEntitySystem.AddToBeConverted(World, prefab.GetComponent<ConvertToClientServerEntity>() );

                    Entity entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                        prefab.GetComponentInChildren<GhostCollectionAuthoringComponent>().gameObject,
                        conversionSettings);*/

                    /*
                    World.EntityManager.Instantiate(entityPrefab);
                    World.EntityManager.DestroyEntity(entityPrefab);*/

                //GameObjectConversionUtility.
                created = true;
            }

            base.OnUpdate();
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
            PostUpdateCommands.RemoveComponent<Prefab>(entity);
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
                PostUpdateCommands.RemoveComponent<Prefab>(entity);
            }
        }
    }
}