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
        public ProductController(ProductRepository productRepo, EcommerceDbContext context)
        {
            _context = context;
            _productRepo = productRepo;
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
