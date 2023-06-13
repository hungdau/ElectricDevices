using TechZone.Data.Infrastructure;

namespace TechZone.Data.Infracstructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private TechZoneDbContext dbContext;

        public TechZoneDbContext Init()
        {
            return dbContext ?? (dbContext = new TechZoneDbContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}