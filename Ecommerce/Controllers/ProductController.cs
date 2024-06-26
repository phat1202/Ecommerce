﻿using AutoMapper;
using Ecommerce.Extensions;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Category;
using Ecommerce.ViewModel.Image;
using Ecommerce.ViewModel.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using NuGet.DependencyResolver;

namespace Ecommerce.Controllers
{
    [Authorize(Policy = "Manager")]
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepo;
        private readonly CategoryRepository _categoryRepo;
        private readonly ProductImageRepository _productimageRepo;
        private readonly ImageRepository _imageRepo;
        private readonly EcommerceDbContext _context;
        private readonly ImageUpLoading _uploadFile;
        private readonly IMapper _mapper;
        public ProductController(EcommerceDbContext context, IMapper mapper, ImageUpLoading uploadFile)
        {
            _context = context;
            _mapper = mapper;
            _productRepo = new ProductRepository(_context, _mapper);
            _categoryRepo = new CategoryRepository(_context, _mapper);
            _productimageRepo = new ProductImageRepository(_context, _mapper);
            _imageRepo = new ImageRepository(_context, _mapper);
            _uploadFile = uploadFile;
        }
        public IActionResult Index()
        {
            var result = new List<ProductViewModel>();
            var productList = _productRepo.GetItem().Include(p => p.Category).ToList();
            foreach (var item in productList)
            {
                var data = _mapper.Map<ProductViewModel>(item);
                data.CategoryNameDisplay = item.Category.Name;
                result.Add(data);
            }
            //var result = _mapper.Map<List<ProductViewModel>>(productList);
            result.OrderBy(p => p.ProductName);
            return View(result);
        }
        public IActionResult CreateProduct()
        {
            var result = new List<CategoryViewModel>();
            var categories = _categoryRepo.GetAll();
            foreach (var item in categories)
            {
                var data = _mapper.Map<CategoryViewModel>(item);
                result.Add(data);
            }
            result.Insert(0, new CategoryViewModel()
            {
                Name = "Lựa chọn danh mục",
                CategoryId = "0",
            });
            ViewData["CategoryList"] = new SelectList(result, "CategoryId", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCrudModel productNew)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = new ProductCrudModel
                    {
                        ProductName = productNew.ProductName,
                        Price = productNew.Price,
                        ProductDescription = productNew.ProductDescription,
                        Quantity = productNew.Quantity,
                        IsActive = true,
                        IsDeleted = false,
                        CategoryId = productNew.CategoryId,
                    };
                    var imageUrl = _uploadFile.UploadImage(productNew.FileImage);
                    var image = new ImageCrudModel
                    {
                        ImageUrl = imageUrl,
                    };
                    var productImage = new ProductImageCrudModel
                    {
                        ImageId = image.ImageId,
                        ProductId = product.ProductId,
                    };
                    _imageRepo.Add(image);
                    _productRepo.Add(product);
                    _productimageRepo.Add(productImage);
                    await _productRepo.CommitAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return View(productNew);
                }
            }
            return View(productNew);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                var product = _productRepo.FirstOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                product.IsDeleted = true;
                product.IsActive = false;
                await _productRepo.CommitAsync();
                //var result = _mapper.Map<ProductViewModel>(product);
                //result.CategoryNameDisplay = categoryName;
                return Json(new { success = true, message = "Xóa sản phẩm thành công" });
            }
            catch (Exception)
            {

                return Json(new { success = true, message = "Không thể xóa sản phẩm" });
            }
        }
        
        public IActionResult EditProduct(string id)
        {
            var product = _productRepo.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            var result = _mapper.Map<ProductCrudModel>(product);
            var categories = _categoryRepo.GetAll();

            var categorylist = _mapper.Map<List<CategoryViewModel>>(categories);
            ViewData["CategoryList"] = new SelectList(categorylist, "CategoryId", "Name");
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductCrudModel model)
        {
            try
            {
                _mapper.Map<Product>(model);
                var product = _productRepo.FirstOrDefault(p => p.ProductId == model.ProductId);
                if (product == null)
                {
                    return View();
                }
                product.ProductName = model.ProductName;
                product.ProductDescription = model.ProductDescription;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.UpdatedAt = DateTime.Now;
                product.Quantity = model.Quantity;
                product.IsActive = model.IsActive;
                product.IsDeleted = model.IsDeleted;
                await _productRepo.CommitAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }
        public IActionResult ProductImageIndex(string Id)
        {
            var images_result = new List<ImageViewModel>();
            var product = _productRepo.GetItem().First(p => p.ProductId == Id);
            var product_Images = _productimageRepo.GetItem().Where(x => x.ProductId == Id).Include(i => i.image).ToList();
            foreach (var item in product_Images)
            {
                if (item.ProductId == Id && item.IsActive == true)
                {
                    var image = new ImageViewModel
                    {
                        ImageId = item.ImageId,
                        ImageUrl = item.image.ImageUrl,
                        ProductId = product.ProductId,
                    };
                    images_result.Add(image);
                }
            }
            images_result.OrderBy(c => c.CreatedAt);
            return View(images_result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewProductImageAsync(IFormFile imageFile, string productId)
        {
            try
            {
                var product = _productRepo.GetItem().First(p => p.ProductId == productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Có lỗi đã xảy ra" });
                }
                var imageUrl = _uploadFile.UploadImage(imageFile);
                var image = new ImageCrudModel
                {
                    ImageUrl = imageUrl,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };
                var productImage = new ProductImageCrudModel
                {
                    ImageId = image.ImageId,
                    ProductId = product.ProductId,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };
                _imageRepo.Add(image);
                _productimageRepo.Add(productImage);
                await _productRepo.CommitAsync();
                return Json(new { success = true, message = "Thêm hình ảnh thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi đã xảy ra: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(string id)
        {
            try
            {   
                var image = _imageRepo.FirstOrDefault(i => i.ImageId == id);
                var productImage = _productimageRepo.FirstOrDefault(p => p.ImageId == image.ImageId);
                if (image == null)
                {
                    return RedirectToAction("Index");
                }
                //_uploadFile.RemoveImageByUrl(image.ImageUrl);              
                _productimageRepo.Delete(productImage);
                _imageRepo.Delete(image);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa hình ảnh thành công" });
            }
            catch (Exception)
            {

                return Json(new { success = true, message = "Không thể xóa hình ảnh" });
            }
        }

    }
}
