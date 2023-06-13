using System.Collections.Generic;
using TechZone.Common.ViewModels;
using TechZone.Data.Repositories;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace TechZone.Service
{
    public interface IStatisticService
    {
        IEnumerable<RevenueStatisticByDateViewModel> GetRevenueStatisticByDate(string fromDate, string toDate);
        IEnumerable<RevenueStatisticByMonthViewModel> GetRevenueStatisticByMonth(string fromMonth, string toMonth);
    }

    public class StatisticService : IStatisticService
    {
        private IOrderRepository _orderRepository;

        public StatisticService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IEnumerable<RevenueStatisticByDateViewModel> GetRevenueStatisticByDate(string fromDate, string toDate)
        {
            return _orderRepository.GetRevenueStatisticByDate(fromDate, toDate);
        }

        public IEnumerable<RevenueStatisticByMonthViewModel> GetRevenueStatisticByMonth(string fromMonth, string toMonth)
        {
            return _orderRepository.GetRevenueStatisticByMonth(fromMonth, toMonth);
        }
    }
}