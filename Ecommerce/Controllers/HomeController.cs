using AutoMapper;
using Ecommerce.Extensions;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Image;
using Ecommerce.ViewModel.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductRepository _productRepo;
        private readonly CategoryRepository _categoryRepo;
        private readonly ImageRepository _imageRepo;
        private readonly ProductImageRepository _productImageRepo;
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public HomeController(ILogger<HomeController> logger, EcommerceDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _productRepo = new ProductRepository(_context, _mapper);
            _categoryRepo = new CategoryRepository(_context, _mapper);
            _imageRepo = new ImageRepository(_context, _mapper);
            _productImageRepo = new ProductImageRepository(_context, _mapper);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ShopViewing()
        {
            var products = _productRepo.GetItem().Include(c => c.Category).Where(p => p.IsActive == true).ToList();
            var result = _mapper.Map<List<ProductViewModel>>(products);
            foreach (var item in result)
            {
                var imageProduct = _productImageRepo.GetItem().First(i => i.ProductId == item.ProductId).ImageId;
                var imagerul = _imageRepo.GetItem().First(i => i.ImageId == imageProduct).ImageUrl;
                item.ProductImageUrl = imagerul;
            }
            return View(result);
        }
        public IActionResult ProductDetail(string productId)
        {
            var list_Image = new List<ImageViewModel>();
            var product = _productRepo.GetItem().First(p => p.ProductId == productId);
            var list_ProductImage = _productImageRepo.GetItem().Where(x => x.ProductId == productId).Include(i => i.image).ToList();
            foreach(var item in list_ProductImage)
            {
                if(item.ProductId == productId)
                {
                    var image = new ImageViewModel
                    {
                        ImageId = item.ImageId,
                        ImageUrl = item.image.ImageUrl,
                    };
                    list_Image.Add(image);
                }
            }
            var data = _mapper.Map<ProductViewModel>(product);
            data.ListProductImage = list_Image;
            return View(data);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
