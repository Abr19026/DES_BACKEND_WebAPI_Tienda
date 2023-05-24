using AutoMapper;
using WebAPI_Tienda.DTOs;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            //Productos
            CreateMap<Producto, GetProductoDTO>();
            CreateMap<PostProductoDTO, Producto>().ForMember(
                dest => dest.Categorias,
                opt => opt.MapFrom(mf => new List<Categoria>() ));
            CreateMap<IFormFile, byte[]>().ConvertUsing<IFormFileTypeConverter>();
            //Categorías
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
