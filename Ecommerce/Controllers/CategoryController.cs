using AutoMapper;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace Ecommerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly EcommerceDbContext _context;
        private readonly CategoryRepository _categoryRepo;
        private readonly ProductRepository _productRepo;
        private readonly IMapper _mapper;
        public CategoryController(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _categoryRepo = new CategoryRepository(_context, _mapper);
            _productRepo = new ProductRepository(_context, _mapper);
        }

        public IActionResult Index()
        {
            var listCategory = _categoryRepo.GetAll();
            var result = _mapper.Map<List<CategoryViewModel>>(listCategory);
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
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                var category = _categoryRepo.FirstOrDefault(p => p.CategoryId == id);
                if (category == null)
                {
                    return RedirectToAction("Index");
                }
                var product_in_category = await _productRepo.GetItem()
                                    .Where(p => p.CategoryId == category.CategoryId)
                                    .ToListAsync();
                foreach(var item in product_in_category)
                {
                    item.IsActive = false;
                }
                category.IsDelete = true;
                category.IsActive = false;
                await _categoryRepo.CommitAsync();
                return Json(new { success = true, message = "Xóa danh mục thành công" });
            }
            catch (Exception)
            {

                return Json(new { success = true, message = "Không thể xóa danh mục này" });
            }
        }
        public IActionResult EditCategory(string id)
        {
            var category = _categoryRepo.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var result = _mapper.Map<CategoryCrudModel>(category);
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryCrudModel model)
        {

            try
            {
                _mapper.Map<Category>(model);
                var product = _categoryRepo.FirstOrDefault(p => p.CategoryId == model.CategoryId);
                if (product == null)
                {
                    return View();
                }
                product.Name = model.Name;
                product.UpdatedAt = DateTime.Now;
                product.IsActive = model.IsActive;
                product.IsDelete = model.IsDelete;
                await _categoryRepo.CommitAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}
