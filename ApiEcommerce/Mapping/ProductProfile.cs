using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

public class ProductProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.CategoryName, src => src.Category.Name);
        config.NewConfig<ProductDto, Product>();
        config.NewConfig<Product, CreateProductDto>().TwoWays();
        config.NewConfig<Product, UpdateProductDto>().TwoWays();
    }
}
