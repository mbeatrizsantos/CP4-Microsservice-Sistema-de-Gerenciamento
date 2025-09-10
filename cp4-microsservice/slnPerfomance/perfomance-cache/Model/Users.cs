namespace perfomance_cache.Model
{
    public class Users
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime UltimoAcesso { get; set; }
    }
}

