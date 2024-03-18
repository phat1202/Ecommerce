using AutoMapper;
using Ecommerce.Models;
using Ecommerce.ViewModel.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ecommerce.Repositories
{
    public class UserRepository
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        public UserRepository(EcommerceDbContext context, IMapper mapper)
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
        public async Task<UserCrudModel> CheckUserExist(UserCrudModel model)
        {
            var data = await _context.Set<User>().FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == model.Email.Trim().ToLower());
            if(data == null)
            {
                return null;
            }
            else
            {
                model.ErrorMessage = "This Email have been registered.";
                return model;
            }

        }
        public User FirstOrDefault(Expression<Func<User, bool>> model)
        {
            IQueryable<User> data = _context.Set<User>();
            return data.FirstOrDefault(model);
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
