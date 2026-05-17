namespace NexusPay.Api.Models
{
    public class ErrorResponse
    {
        public int Status { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public string Path { get; set; }
    }
}
