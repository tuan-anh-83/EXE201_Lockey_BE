using EXE201_Lockey.Dto;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;

namespace EXE201_Lockey.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO> GetOrderByIdAsync(int id);
        Task<string> AddOrderAsync(OrderDTO orderDTO);
        Task<string> UpdateOrderAsync(int id, OrderDTO orderDTO);
        Task<string> DeleteOrderAsync(int id);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<string> AddOrderAsync(OrderDTO orderDTO)
        {
            try
            {
                var order = new Order
                {
                    ProductID = orderDTO.ProductID,
                    OrderDate = orderDTO.OrderDate,
                    Amount = orderDTO.Amount,
                    Status = orderDTO.Status,
                    TotalPrice = orderDTO.TotalPrice
                };

                await _orderRepository.AddOrderAsync(order);
                return "Order created successfully.";
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes (you can use a logger)
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> UpdateOrderAsync(int id, OrderDTO orderDTO)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return "Order not found.";
                }

                order.ProductID = orderDTO.ProductID;
                order.OrderDate = orderDTO.OrderDate;
                order.Amount = orderDTO.Amount;
                order.Status = orderDTO.Status;
                order.TotalPrice = orderDTO.TotalPrice;

                await _orderRepository.UpdateOrderAsync(order);
                return "Order updated successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> DeleteOrderAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return "Order not found.";
                }

                await _orderRepository.DeleteOrderAsync(id);
                return "Order deleted successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return null;

            return new OrderDTO
            {
                OrderID = order.OrderID,
                ProductID = order.ProductID,
                OrderDate = order.OrderDate,
                Amount = order.Amount,
                Status = order.Status,
                TotalPrice = order.TotalPrice
            };
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Select(order => new OrderDTO
            {
                OrderID = order.OrderID,
                ProductID = order.ProductID,
                OrderDate = order.OrderDate,
                Amount = order.Amount,
                Status = order.Status,
                TotalPrice = order.TotalPrice
            }).ToList();
        }
    }



}
