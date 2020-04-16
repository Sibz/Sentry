using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;

namespace Sibz.Sentry
{
    public class WorldZonesCreator : IZoneWorldCreator
    {
        public List<World> CreateZoneWorlds(World defaultWorld, string zonePrefix)
        {
            var worlds = new List<World>();
            for (int i = 0; i < 16; i++)
            {
                worlds.Add(ClientServerBootstrap.CreateServerWorld(defaultWorld, $"{zonePrefix}_{i}"));
            }

            return worlds;
        }
    }
}