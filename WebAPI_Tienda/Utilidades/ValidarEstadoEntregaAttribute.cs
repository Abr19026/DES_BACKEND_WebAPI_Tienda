using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.Utilidades
{
    public class ValidarEstadoEntregaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }
    }
}
