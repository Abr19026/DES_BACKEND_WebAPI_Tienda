using System.ComponentModel.DataAnnotations;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.Utilidades
{
    public class EstadoEntregaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var val_string = (string)value;
            EstadoEntrega x ;
            if (value != null && Enum.TryParse<EstadoEntrega>(val_string, true,out x))
            {
                return ValidationResult.Success;
            } else
            {
                return new ValidationResult("El Estado de entrega no es un valor válido, debe ser Despachando,Enviando,Recibido,Cancelado");
            }
        }
    }
}
