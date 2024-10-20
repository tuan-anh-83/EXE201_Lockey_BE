using EXE201_Lockey.Dto;
using EXE201_Lockey.Services;
using Microsoft.AspNetCore.Mvc;

namespace EXE201_Lockey.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
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

        // GET: api/Payments/generate-vietqr/5
        [HttpGet("generate-vietqr/{orderId}")]
        public async Task<ActionResult<string>> GenerateVietQR(int orderId)
        {
            var qrCodeBase64 = await _paymentService.GenerateVietQR(orderId);
            if (qrCodeBase64 == "Order not found.")
            {
                return NotFound(qrCodeBase64);
            }

            return Ok(new { QRCode = qrCodeBase64 });
        }
    }

}
