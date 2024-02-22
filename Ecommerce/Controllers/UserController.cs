using Ecommerce.Const;
using Ecommerce.Extensions;
using Ecommerce.Models;
using Ecommerce.Repositories;
using Ecommerce.ViewModel.Cart;
using Ecommerce.ViewModel.User;
using Microsoft.AspNetCore.Mvc;
using static Ecommerce.Const.EnumClass;

namespace Ecommerce.Controllers
{
    public class UserController : Controller
    {
        private readonly EcommerceDbContext _context;
        private readonly UserRepositories _userRepo;
        private readonly CartRepositories _cartRepo;
        public UserController(EcommerceDbContext context, UserRepositories userRepo, CartRepositories cartRepo)
        {
            _context = context;
            _userRepo = userRepo;
            _cartRepo = cartRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserCrudModel model)
        {
            try
            {
                var userExisting = _userRepo.FirstOrDefault(model);
                if (userExisting != null)
                {
                    return View(userExisting);
                }
                if (ModelState.IsValid)
                {
                    var cartUser = new CartCrudModel
                    {
                        TotalPrice = 0,
                    };
                    var newUser = new UserCrudModel
                    {
                        Email = model.Email,
                        Password = model.Password.Hash(),
                        Name = model.Name,
                        Gender = model.Gender,
                        Role = (int)EnumClass.Role.User,
                        CartId = cartUser.CartId,
                    };
                    _cartRepo.Add(cartUser);
                    _userRepo.Add(newUser);
                    await _userRepo.CommitAsync();
                    return RedirectToAction("Login");
                }
                else
                {
                    model.ErrorMessage = "Có lỗi khi tạo tài khoản";
                    return View(model);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}
