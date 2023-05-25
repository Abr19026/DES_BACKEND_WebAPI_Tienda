using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WebAPI_Tienda.DTOs;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            // DTOs carrito
            CreateMap<Pedido, GetCarritoResumenDTO>();
            CreateMap<ConceptoPedido, GetConceptoCarritoDTO>();
            CreateMap<Producto, GetResumenProdutcoDTO>();
            // DTO Pago y Envío
            CreateMap<Pago, GetPagoDTO>();
            CreateMap<DatosEnvio, GetDatosEnvioDTO>();
            CreateMap<PostPagoDTO, Pago>();
            CreateMap<DatosEnvioDTO, DatosEnvio>();
            // DTO Pedido
            CreateMap<Pedido, GetPedidoDTO>().ForMember(
                dest => dest.Estado,
                opt => opt.MapFrom(mf => mf.Estado.ToString())
                );
            CreateMap<ConceptoPedido, GetConceptodPedidoDTO>();
            CreateMap<Producto, GetProductoPedidoDTO>();
            CreateMap<ConceptoPedido, ConceptoPedidoEstadoDTO>().ForMember(
                    dest => dest.EstadoEnvio,
                    opt => opt.MapFrom(mf => mf.EstadoEntrega.ToString())
                ).ForMember(
                    dest => dest.NombreProducto,
                    opt => opt.MapFrom(mf=>mf.Producto.Nombre)
                );
            // Usuarios
            CreateMap<IdentityUser, GetUserDTO>();
            // Productos
            CreateMap<Producto, GetProductoDTO>();
            CreateMap<PostProductoDTO, Producto>().ForMember(
                dest => dest.Categorias,
                opt => opt.MapFrom(mf => new List<Categoria>()));
            CreateMap<IFormFile, byte[]>().ConvertUsing<IFormFileTypeConverter>();
            // Categorías
            CreateMap<Categoria, GetCategoriaDTO>();
            CreateMap<PostCategoriaDTO, Categoria>();
        }
        /*CreateMap<Product, GetProductsQueryResponse>().ForMember(
                dest => dest.ListDescription,
                opt => opt.MapFrom(mf => $"{mf.Description} - {mf.Price:c}"));*/
    }

    public class IFormFileTypeConverter :ITypeConverter<IFormFile, byte[]>
    {
        public byte[] Convert(IFormFile source, byte[] destination, ResolutionContext ctx)
        {
            if (source != null)
            {
                MemoryStream target = new MemoryStream();
                source.CopyTo(target);
                return target.ToArray();
            }
            else
            {
                return new byte[0];
            }
            
        }
    }
}
