namespace WebAPI_Tienda.DTOs
{
    public class GetPagoDTO
    {
        public int ID { get; set; }
        public string NumeroTarjeta { get; set; }
        public int YYExpiracion { get; set; }// Año de expiración
        public int MMExpiracion { get; set; }// Mes de expiración
        public DateTime FechaPago { get; set; }

    }
}
