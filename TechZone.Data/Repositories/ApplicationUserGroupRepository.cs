using TechZone.Data.Infrastructure;
using TechZone.Model.Models;
using TechZone.Data.Infrastructure;
using TechZone.Model.Models;

namespace TechZone.Data.Repositories
{
    public interface IApplicationUserGroupRepository : IRepository<ApplicationUserGroup>
    {
    }

    public class ApplicationUserGroupRepository : RepositoryBase<ApplicationUserGroup>, IApplicationUserGroupRepository
    {
        public ApplicationUserGroupRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}