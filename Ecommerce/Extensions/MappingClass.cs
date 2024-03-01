using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using AutoMapper;
using Ecommerce.ViewModel.User;
using Ecommerce.ViewModel.Category;
using Ecommerce.ViewModel.Product;
namespace Ecommerce.Extensions
{
    public class MappingClass : Profile
    {
        public MappingClass()
        {
            CreateMap<CartCrudModel, Cart>();
            CreateMap<UserCrudModel, User>();
            //Category
            //CreateMap<CategoryCrudModel, Category>();
            CreateMap<Category, CategoryCrudModel>().ReverseMap();
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            //Product
            CreateMap<ProductCrudModel, Product>().ReverseMap(); ;
            CreateMap<Product, ProductViewModel>().ReverseMap(); ;
            CreateMap<ProductCrudModel, ProductViewModel>().ReverseMap(); ;
            //Image
            CreateMap<ProductImageCrudModel, Image>();
        }
    }
}
