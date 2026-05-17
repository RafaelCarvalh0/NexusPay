namespace NexusPay.Shared.Models.Error
{
    public class ErrorResponse
    {
        public int Status { get; set; }
        public string Error { get; set; }
        public string[] Messages { get; set; }
        public string Path { get; set; }
    }
}
