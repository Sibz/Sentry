using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry
{
    public abstract class ZonedGameBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
            GenerateSystemLists(systems);

            World world = new World(defaultWorldName);
            World.DefaultGameObjectInjectionWorld = world;

            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, ExplicitDefaultWorldSystems);
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            /*
            World lobby = CreateServerWorld(world, "LobbyWorld");
            lobby.GetExistingSystem<ServerInitializationSystemGroup>().AddSystemToUpdateList( lobby.GetOrCreateSystem<LobbyServerWorldInitGroup>());
            lobby.GetExistingSystem<ServerSimulationSystemGroup>().AddSystemToUpdateList( lobby.GetOrCreateSystem<LobbyServerWorldSimGroup>());
            lobby.GetExistingSystem<RpcCommandRequestSystemGroup>().AddSystemToUpdateList( lobby.GetOrCreateSystem<LobbyRpcRequestGroup>());
            ConvertToRpcRequestSystem.CreateInWorld(lobby);*/
            return true;
        }
    }
}