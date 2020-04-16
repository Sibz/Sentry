using System.Collections.Generic;
using Unity.Entities;

namespace Sibz.Sentry
{
    public interface IClientWorldCreator
    {
        List<World> CreateClientWorld(World defaultWorld, string worldName);
    }
}