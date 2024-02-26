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
            CreateMap<CategoryCrudModel, Category>();
            CreateMap<Category, CategoryViewModel>();
            CreateMap<ProductCrudModel, Product>();
        }
    }
}
