namespace Meus_Contatos.Models
{
    public class DDD
    {
        
        public int ID { get; set; }
        public int EstadoId { get; set; }
        public virtual Estado Estado { get; set; }
        public int CodigoDDD { get; set; }
    }
}
