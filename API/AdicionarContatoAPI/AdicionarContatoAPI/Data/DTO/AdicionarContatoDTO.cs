namespace AdicionarContatoAPI.Data.DTO
{
    public class AdicionarContatoDTO
    {
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public string NumeroTelefone { get; set; }
        public int DDDId { get; set; }
    }
}
