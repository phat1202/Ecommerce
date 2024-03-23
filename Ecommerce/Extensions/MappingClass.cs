using Ecommerce.Models;
using Ecommerce.ViewModel.Cart;
using AutoMapper;
using Ecommerce.ViewModel.User;
using Ecommerce.ViewModel.Category;
using Ecommerce.ViewModel.Product;
using Ecommerce.ViewModel.Image;
using Ecommerce.ViewModel.Order;
namespace Ecommerce.Extensions
{
    public class MappingClass : Profile
    {
        public MappingClass()
        {        
            //User
            CreateMap<UserCrudModel, User>().ReverseMap();
            CreateMap<UserViewModel, User>().ReverseMap();
            //Category
            //CreateMap<CategoryCrudModel, Category>();
            CreateMap<Category, CategoryCrudModel>().ReverseMap();
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            //Product
            CreateMap<ProductCrudModel, Product>().ReverseMap(); ;
            CreateMap<Product, ProductViewModel>().ReverseMap(); ;
            CreateMap<ProductCrudModel, ProductViewModel>().ReverseMap();
            //ProductImage
            CreateMap<ProductImageCrudModel, ProductImage>().ReverseMap();
            CreateMap<ProductImage, ProductImageViewModel>().ReverseMap();
            //ImageRepo
            CreateMap<ImageCrudModel, Image>().ReverseMap();
            //Cart
            CreateMap<CartCrudModel, Cart>().ReverseMap();
            CreateMap<CartViewModel, Cart>().ReverseMap();
            //CartItem
            CreateMap<CartItem, CartItemCrudModel>().ReverseMap();
            CreateMap<CartItemViewModel, CartItem>().ReverseMap();
            //Order
            CreateMap<OrderCrudModel, Order>().ReverseMap();
            CreateMap<OrderViewModel, Order>().ReverseMap();
            CreateMap<OrderCrudModel, OrderViewModel>().ReverseMap();
            //OrderItem
            CreateMap<OrderItemCrudModel, OrderItem>().ReverseMap();
            CreateMap<OrderItemViewModel, OrderItem>().ReverseMap();
        }
    }
}
