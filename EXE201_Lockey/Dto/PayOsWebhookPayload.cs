namespace EXE201_Lockey.Dto
{
    public class PayOsWebhookPayload
    {
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string Checksum { get; set; }
    }

}
