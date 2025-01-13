namespace Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
