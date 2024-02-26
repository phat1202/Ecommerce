using AutoMapper;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Product;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepo;
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public ProductController(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _productRepo = new ProductRepository(_context, _mapper);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCrudModel productNew)
        {
            var product = new ProductCrudModel
            {
                ProductName = productNew.ProductName,
                Price = productNew.Price,
                ProductDescription = productNew.ProductDescription,
                Quantity = productNew.Quantity,
            };
            _productRepo.Add(product);
            await _productRepo.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
