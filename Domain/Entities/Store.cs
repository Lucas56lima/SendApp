﻿namespace Domain.Entities
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cnpj { get; set; }        
        public string Email { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public bool Active = true;
    }
}
