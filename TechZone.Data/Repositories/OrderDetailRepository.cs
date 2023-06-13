using System.Collections.Generic;
using TechZone.Data.Infrastructure;
using TechZone.Model.Models;
using System.Linq;

namespace TechZone.Data.Repositories
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
    }

    public class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}