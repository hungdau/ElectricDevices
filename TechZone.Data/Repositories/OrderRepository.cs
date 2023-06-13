using System.Collections.Generic;
using System.Data.SqlClient;
using TechZone.Common.ViewModels;
using TechZone.Data.Infrastructure;
using TechZone.Model.Models;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace TechZone.Data.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<RevenueStatisticByDateViewModel> GetRevenueStatisticByDate(string fromDate, string toDate);
        IEnumerable<RevenueStatisticByMonthViewModel> GetRevenueStatisticByMonth(string fromMonth, string toMonth);
    }

    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<RevenueStatisticByDateViewModel> GetRevenueStatisticByDate(string fromDate, string toDate)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@fromDate",fromDate),
                new SqlParameter("@toDate",toDate)
            };
            return DbContext.Database.SqlQuery<RevenueStatisticByDateViewModel>("GetRevenueStatisticByDate @fromDate,@toDate", parameters);
        }

        public IEnumerable<RevenueStatisticByMonthViewModel> GetRevenueStatisticByMonth(string fromMonth, string toMonth)
        {
            var parameters = new SqlParameter[]{
                new SqlParameter("@fromDate",fromMonth),
                new SqlParameter("@toDate",toMonth)
            };
            return DbContext.Database.SqlQuery<RevenueStatisticByMonthViewModel>("GetRevenueStatisticByMonth @fromDate,@toDate", parameters);
        }
    }
}