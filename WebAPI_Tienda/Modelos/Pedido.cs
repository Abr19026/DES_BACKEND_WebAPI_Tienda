namespace WebAPI_Tienda.Modelos
{
    public class Pedido
    {
        public int ID { get; set; }
        //public int UserID
        //public int PagoID
        public DateTime FechaPedido { get; set; }

        public ICollection<ConceptoPedido> ConceptosPedido { get; set; }
    }
}
