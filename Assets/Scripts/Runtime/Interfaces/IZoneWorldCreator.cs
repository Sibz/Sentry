using System.Collections.Generic;
using Unity.Entities;

namespace Sibz.Sentry
{
    public interface IZoneWorldCreator
    {
        List<World> CreateZoneWorlds(World defaultWorld, string zonePrefix);
    }
}