using AutoMapper;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Category;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Ecommerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly EcommerceDbContext _context;
        private readonly CategoryRepository _categoryRepo;
        private readonly IMapper _mapper;
        public CategoryController(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _categoryRepo = new CategoryRepository(_context, _mapper);
  
        }

        public IActionResult Index()
        {
            var result = new List<CategoryViewModel>();
            var listCategory = _categoryRepo.GetAll();
            foreach(var item in listCategory)
            {
                var data = _mapper.Map<CategoryViewModel>(item);
                if (data.IsActive)
                {
                    
                }
                result.Add(data);
            }

            return View(result);
        }
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCategory(CategoryCrudModel category)
        {
            var newCategory = new CategoryCrudModel
            {
                Name = category.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = true,
                IsDelete = false,
            };
            _categoryRepo.Add(newCategory);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
