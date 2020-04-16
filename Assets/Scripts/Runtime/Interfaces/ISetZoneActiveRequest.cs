using Unity.Entities;

namespace Sibz.Sentry
{
    public interface ISetZoneActiveRequest
    {
        void SetZoneActive(EntityCommandBuffer buffer, Entity targetConnection);
    }
}