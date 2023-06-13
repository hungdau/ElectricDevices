using System;

namespace TechZone.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        TechZoneDbContext Init();
    }
}