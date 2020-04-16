using Unity.Networking.Transport;

namespace Sibz.Sentry
{
    public interface IZoneOptions
    {
        int ZonePortBase { get; }
        NetworkEndPoint EndPoint { get; }
    }
}