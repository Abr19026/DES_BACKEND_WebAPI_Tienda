using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.DTOs
{
    public class GetCarritoResumenDTO
    {
        public int ID { get; set; }
        public float total { get;set; }
        public ICollection<GetConceptoCarritoDTO> ConceptosPedido { get; set; }
    }
}
