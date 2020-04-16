using System;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(RpcCommandRequestSystemGroup))]
    public class LobbyRpcRequestGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type system in types.Where(x=>x.GetCustomAttributes<UpdateInGroupAttribute>().Any(y=>y.GroupType==typeof(LobbyRpcRequestGroup))))
            {
                AddSystemToUpdateList(World.CreateSystem(system));
            }
            SortSystemUpdateList();
            base.OnCreate();
        }
    }
}