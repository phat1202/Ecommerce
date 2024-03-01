using AutoMapper;
using Ecommerce.Extensions;
using Ecommerce.Models;
using Ecommerce.Repositories;
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
        private readonly ProductImageRepository _imageRepo;
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public HomeController(ILogger<HomeController> logger, EcommerceDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _productRepo = new ProductRepository(_context, _mapper);
            _categoryRepo = new CategoryRepository(_context, _mapper);
            _imageRepo = new ProductImageRepository(_context, _mapper);
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
            return View(result);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
