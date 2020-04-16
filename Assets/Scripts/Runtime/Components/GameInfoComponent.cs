using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Sibz.Sentry.Components
{
    [GenerateAuthoringComponent]
    public struct GameInfoComponent : IComponentData
    {
        public int Id;
        public NativeString64 Name;
        public int SizeX;
        public int SizeY;
    }
}