using System.ComponentModel.DataAnnotations;
using WebAPI_Tienda.Modelos;
using WebAPI_Tienda.Utilidades;

namespace WebAPI_Tienda.DTOs
{
    public class CambiarEstadoConceptoDTO
    {
        [Required] public int pedidoid { get; set; }
        [Required] public int productoid { get; set; }
        [Required][EstadoEntrega]public string nuevo_estado { get; set; }
    }
}
