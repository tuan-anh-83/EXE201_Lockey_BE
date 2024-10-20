using EXE201_Lockey.Dto;
using EXE201_Lockey.Interfaces;
using EXE201_Lockey.Models;
using QRCoder;

namespace EXE201_Lockey.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
        Task<PaymentDTO> GetPaymentByIdAsync(int id);
        Task<string> CreatePaymentAsync(PaymentDTO paymentDTO);
        Task<string> UpdatePaymentStatusAsync(int id, string status);
        Task<string> DeletePaymentAsync(int id);
        Task<string> GenerateVietQR(int orderId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
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

        public async Task<string> GenerateVietQR(int orderId)
        {
            // Lấy thông tin đơn hàng
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return "Order not found.";
            }

            // Thông tin cần cho mã QR
            string bankCode = "BIDV"; // Mã ngân hàng, ví dụ là Vietcombank
            string accountNumber = "7211217233"; // Số tài khoản ngân hàng nhận thanh toán
            string accountName = "PHAM TUAN ANH"; // Tên chủ tài khoản
            string amount = order.TotalPrice.ToString("F2"); // Số tiền cần thanh toán
            string description = $"Payment for order {order.OrderID}";

            // Nội dung QR theo chuẩn VietQR
            // string qrContent = $"https://img.vietqr.io/image/?amount={amount}&bankCode={bankCode}&orderId={orderId}&description={description}";
            string qrUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact2.png?amount={amount}&addInfo={description}&accountName={accountName}";
            // Tạo mã QR
            /*using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                // Tạo hình ảnh QR dưới dạng byte[]
                byte[] qrCodeImage = qrCode.GetGraphic(10);

                // Chuyển byte[] thành Base64 để có thể trả về như một chuỗi
                return Convert.ToBase64String(qrCodeImage);*/



            return qrUrl;

            
        }
    }

}
