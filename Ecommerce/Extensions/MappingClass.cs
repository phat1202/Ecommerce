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
            CreateMap<Category, CategoryViewModel>();

            //Product
            CreateMap<ProductCrudModel, Product>();
            CreateMap<Product, ProductCrudModel>();
            CreateMap<Product, ProductViewModel>();
            CreateMap<ProductCrudModel, ProductViewModel>();
            //Image
            CreateMap<ProductImageCrudModel, Image>();
        }
    }
}
