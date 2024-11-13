using EXE201_Lockey.Dto;
using EXE201_Lockey.Dto.PayOsDtos;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using Net.payOS;
using Net.payOS.Types;
using QRCoder;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace EXE201_Lockey.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
        Task<PaymentDTO> GetPaymentByIdAsync(int id);
        Task<string> CreatePaymentAsync(PaymentDTO paymentDTO);
        Task<string> UpdatePaymentStatusAsync(int id, string status);
        Task<string> DeletePaymentAsync(int id);
        Task<string> GeneratePayOsPayment(int orderId); // Tạo Payment Link
        Task<string> HandlePayOsWebhook(PayOsWebhookPayload payload); // Xử lý Webhook

        Task<string> UpdateOrderAndCreatePayment(int orderId, string paymentMethod, string orderStatus);
    }


    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        private readonly PayOS _payOS; // Thêm PayOs

        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IConfiguration configuration, PayOS payOS)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;

            _payOS = payOS; // Gán PayOs
        }

        public async Task<string> CreatePaymentAsync(PaymentDTO paymentDTO)
        {
            try
            {
                var payment = new Payment
                {
                    OrderID = paymentDTO.OrderID,
                    PaymentMethod = paymentDTO.PaymentMethod,
                    Status = paymentDTO.Status,
                    PaymentDate = DateTime.UtcNow
                };

                await _paymentRepository.AddPaymentAsync(payment);
                return "Payment created successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> UpdatePaymentStatusAsync(int id, string status)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return "Payment not found.";
            }

            payment.Status = status;
            await _paymentRepository.UpdatePaymentAsync(payment);
            return "Payment status updated successfully.";
        }

        public async Task<string> DeletePaymentAsync(int id)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return "Payment not found.";
            }

            await _paymentRepository.DeletePaymentAsync(id);
            return "Payment deleted successfully.";
        }

        public async Task<PaymentDTO> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (payment == null) return null;

            return new PaymentDTO
            {
                PaymentID = payment.PaymentID,
                OrderID = payment.OrderID,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate
            };
        }
        public async Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllPaymentsAsync();
            return payments.Select(payment => new PaymentDTO
            {
                PaymentID = payment.PaymentID,
                OrderID = payment.OrderID,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate
            }).ToList();
        }

        // Tạo Payment Link cho PayOs
        public async Task<string> GeneratePayOsPayment(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return "Order not found.";
            }

            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            int totalPrice = Convert.ToInt32(order.TotalPrice);

            Net.payOS.Types.ItemData item = new Net.payOS.Types.ItemData("Product Name", 1, totalPrice);
            List<Net.payOS.Types.ItemData> items = new List<Net.payOS.Types.ItemData> { item };
            string urlbase = "https://lockeybe.azurewebsites.net/Payments/cancel-payment-callback";
            string url = $"{urlbase}?orderId={orderId}&paymentMethod=PayOs";
            // string
            Net.payOS.Types.PaymentData paymentData = new Net.payOS.Types.PaymentData(
                orderCode,
                totalPrice,
                "Thanh toán đơn hàng",
                items,
                 url, // Cancel URL
                url  // Success URL
                     // Success URL
            );

            var createPayment = await _payOS.createPaymentLink(paymentData);
            if (createPayment != null && !string.IsNullOrEmpty(createPayment.checkoutUrl))
            {
                return createPayment.checkoutUrl;
            }

            return "Failed to generate payment link";
        }




        // Xử lý Webhook từ PayOs
        public async Task<string> HandlePayOsWebhook(PayOsWebhookPayload payload)
        {
            if (payload.Status == "success")
            {
                await UpdatePaymentStatusAsync(int.Parse(payload.TransactionId), "Paid");
            }
            else
            {
                await UpdatePaymentStatusAsync(int.Parse(payload.TransactionId), "Failed");
            }
            return "Webhook processed.";
        }


        public async Task<string> UpdateOrderAndCreatePayment(int orderId, string paymentMethod, string orderStatus)
        {

            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return "Order not found.";
            }

            if (order.Status == "Pending")
            {
                return "Order is already marked as Paid.";
            }

            order.Status = orderStatus;
            await _orderRepository.UpdateOrderAsync(order);

            var payment = new Payment
            {
                OrderID = orderId,
                PaymentMethod = paymentMethod,
                Status = orderStatus,
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.AddPaymentAsync(payment);

            return "Payment successful and order updated to Paid.";
        }

    }
}



