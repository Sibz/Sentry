using System;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

namespace Sibz.Sentry.Lobby.Client
{
    [DisableAutoCreation]
    public class SimulationGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type system in types.Where(x=>x.GetCustomAttributes<UpdateInGroupAttribute>().Any(y=>y.GroupType==typeof(SimulationGroup))))
            {
                AddSystemToUpdateList(World.CreateSystem(system));
            }
            SortSystemUpdateList();
            base.OnCreate();
        }
    }
}