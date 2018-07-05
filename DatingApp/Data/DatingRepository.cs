using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    /// <summary>
    /// Must have to inject to the Startup.cs and make this class available for the Controller to be injected.
    /// services.AddScoped<IDatingRepository, DatingRepository>();
    /// </summary>
    public class DatingRepository : IDatingRepository
    {
#region Fields
        private readonly DataContext _context;
#endregion
#region Ctor
        public DatingRepository(DataContext context)
        {
            this._context = context;
        }
#endregion
#region Methods
        /// <summary>
        /// Implementing the IDatingRepository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(p => p.Photos).ToListAsync();

            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
#endregion
    }
}