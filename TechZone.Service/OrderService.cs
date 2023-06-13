using System;
using System.Collections.Generic;
using TechZone.Data.Infracstructure;
using TechZone.Data.Repositories;
using TechZone.Model.Models;

namespace TechZone.Service
{
    public interface IOrderService
    {
        bool Create(Order order, List<OrderDetail> orderDetails);

        OrderDetail AddOrderDetail(OrderDetail orderDetail);

        void UpdateOrderDetail(OrderDetail orderDetail);

        void UpdateOrder(Order order);

        IEnumerable<Order> GetAllOrder();

        Order GetOrderById(int id);
        IEnumerable<Order> GetListOrderByUserId(string id);

        Order GetOrderByUserId(string id);

        OrderDetail GetOrderDetailByProductId(int orderId, int productId);

        IEnumerable<Order> GetAllOrder(string keyword);

        Order DeleteOrder(int id);

        OrderDetail DeleteOrderDetail(int orderId, int productId);

        void Save();

        IEnumerable<OrderDetail> GetAllOrderDetail(int id);
    }

    public class OrderService : IOrderService
    {
        private IOrderRepository _orderRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IUnitOfWork unitOfWork)
        {
            this._orderRepository = orderRepository;
            this._orderDetailRepository = orderDetailRepository;
            this._unitOfWork = unitOfWork;
        }

        public OrderDetail AddOrderDetail(OrderDetail orderDetail)
        {
            return _orderDetailRepository.Add(orderDetail);
        }

        public bool Create(Order order, List<OrderDetail> orderDetails)
        {
            try
            {
                _orderRepository.Add(order);
                _unitOfWork.Commit();

                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.OrderID = order.ID;
                    orderDetail.IsOrder = true;
                    _orderDetailRepository.Add(orderDetail);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Order DeleteOrder(int id)
        {
            var order = this._orderRepository.GetSingleById(id);
            return _orderRepository.Delete(order);
        }

        public OrderDetail DeleteOrderDetail(int orderId, int productId)
        {
            var orderDetail = this._orderDetailRepository.GetSingleByCondition(od => od.ProductID == productId && od.OrderID == orderId);
            return _orderDetailRepository.Delete(orderDetail);
        }

        public IEnumerable<Order> GetAllOrder()
        {
            return _orderRepository.GetAll();
        }

        public IEnumerable<Order> GetAllOrder(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
                return _orderRepository.GetMulti(x => x.CustomerName.Contains(keyword));
            else
                return _orderRepository.GetAll();
        }

        public IEnumerable<OrderDetail> GetAllOrderDetail(int id)
        {
            return _orderDetailRepository.GetMulti(x => x.OrderID == id);
        }

        public Order GetOrderById(int id)
        {
            return _orderRepository.GetSingleById(id);
        }

        public IEnumerable<Order> GetListOrderByUserId(string id)
        {
            //return _orderRepository.GetSingleByCondition(o => o.CustomerID == id);
            return _orderRepository.GetMulti(o => o.CustomerID == id);
        }

        public OrderDetail GetOrderDetailByProductId(int orderId, int productId)
        {
            return _orderDetailRepository.GetSingleByCondition(od => od.ProductID == productId && od.OrderID == orderId);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateOrder(Order order)
        {
            _orderRepository.Update(order);
        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            _orderDetailRepository.Update(orderDetail);
        }
        public Order GetOrderByUserId(string id)
        {
            return _orderRepository.GetSingleByCondition(o => o.CustomerID == id && o.PaymentStatus == null);
        }
    }
}