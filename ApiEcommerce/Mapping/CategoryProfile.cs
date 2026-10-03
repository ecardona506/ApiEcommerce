using Mapster;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Mapping;

public class CategoryProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDto>().TwoWays();
        config.NewConfig<Category, CreateCategoryDto>().TwoWays();
    }
}
