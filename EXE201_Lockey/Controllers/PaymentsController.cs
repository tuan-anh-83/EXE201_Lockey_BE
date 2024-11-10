using EXE201_Lockey.Dto;
using EXE201_Lockey.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace EXE201_Lockey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;


        public PaymentsController(IPaymentService paymentService, IConfiguration configuration) // Thêm IConfiguration vào constructor
        {
            _paymentService = paymentService;

        }

        // POST: api/Payments
        [HttpPost]
        public async Task<ActionResult> PostPayment(PaymentDTO paymentDTO)
        {
            var message = await _paymentService.CreatePaymentAsync(paymentDTO);
            if (message.Contains("Error"))
            {
                return BadRequest(message);
            }

            return Ok(message);
        }

        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDTO>> GetPayment(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            return Ok(payment);
        }

        // PUT: api/Payments/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] string status)
        {
            var message = await _paymentService.UpdatePaymentStatusAsync(id, status);
            if (message == "Payment not found.")
            {
                return NotFound(message);
            }

            return Ok(message);
        }

        [HttpGet("generate-payos/{orderId}")]
        public async Task<ActionResult<string>> GeneratePayOsPayment(int orderId)
        {
            var paymentUrl = await _paymentService.GeneratePayOsPayment(orderId);
            if (paymentUrl == "Order not found.")
            {
                return NotFound(paymentUrl);
            }

            return Ok(new { PaymentUrl = paymentUrl });
        }

        // Xử lý Webhook từ PayOs
        [HttpPost("webhook")]
        public async Task<IActionResult> PayOsWebhook([FromBody] PayOsWebhookPayload payload)
        {
            // Kiểm tra trạng thái thanh toán trong webhook
            if (payload.Status == "success")
            {
                // Thanh toán thành công - cập nhật trạng thái đơn hàng
                await _paymentService.UpdatePaymentStatusAsync(int.Parse(payload.TransactionId), "Paid");
                return Ok("Payment processed successfully.");
            }
            else
            {
                // Thanh toán thất bại - cập nhật trạng thái đơn hàng
                await _paymentService.UpdatePaymentStatusAsync(int.Parse(payload.TransactionId), "Failed");
                return Ok("Payment failed.");
            }
        }
        [HttpGet("cancel-payment-callback")]
        public async Task<ActionResult> CallBack(
            [FromQuery] int orderId,
            [FromQuery] string paymentMethod,
              [FromQuery] string status)
        {
            if (status == "CANCELLED")
            {
                var result = await _paymentService.UpdateOrderAndCreatePayment(orderId, paymentMethod, "CANCELLED");
                if (result.Contains("Error")) return BadRequest(result);
                return Redirect("http://localhost:3000/payment-cancelled");
            }
            else
            {
                var result = await _paymentService.UpdateOrderAndCreatePayment(orderId, paymentMethod, "PAID");
                if (result.Contains("Error")) return BadRequest(result);
                return Redirect("http://localhost:3000/payment-success");
            }

            return BadRequest("Payment not completed.");
        }




    }
}
