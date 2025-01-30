namespace Domain.Entities
{
    public class Scheduling
    {
        public int Id { get; set; }
        public string Store {  get; set; }
        public string Status {  get; set; }
        public DateOnly TransitionDate { get; set; }
        public bool Active = true;
    }
}
