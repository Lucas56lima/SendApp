namespace Domain.Entities
{
    public class Store
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Cnpj { get; set; }        
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Path { get; set; }
        public bool Active = true;
    }
}
