using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class UserRepositories
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public UserRepositories(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public UserCrudModel GetById(string id)
        {
            var data = _context.Set<User>().Find(id);
            var user = _mapper.Map<UserCrudModel>(data);
            return user;
        }
        public List<User> GetAll()
        {
            return _context.Set<User>().ToList();
        }
        public void Add(UserCrudModel user)
        {
            var data = _mapper.Map<User>(user);
            _context.Set<User>().Add(data);
        }
        public void Update(UserCrudModel user)
        {
            var data = _mapper.Map<User>(user);
            _context.Set<User>().Update(data);
        }
        public void Delete(UserCrudModel user) 
        {
            var data = _mapper.Map<User>(user);
            _context.Set<User>().Remove(data);
        }
        public async Task<UserCrudModel> FirstOrDefault(UserCrudModel model)
        {
            var data = await _context.Set<User>().FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == model.Email.Trim().ToLower());
            if(data == null)
            {
                return null;
            }
            else
            {
                model.ErrorMessage = "Tài khoản đã tồn tại";
                return model;
            }

        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
