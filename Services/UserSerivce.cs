using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserInfo.Web.Data;
using UserInfo.Web.Models.Entities;
using UserInfo.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace UserInfo.Web.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<(List<User> users, int totalUsers)> GetPageUsersAsync(int pageNumber, int pageSize)
        {
            var usersExist = _dbContext.Users
                            .Where(user => user.DeleletedFlg == false)
                            .OrderBy(user => user.Name);
            var totalUsers = await usersExist.CountAsync();

            var users = await usersExist
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return (users, totalUsers);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task AddUserAsync(AddUserViewModel viewModel)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = new User
                    {
                        Name = viewModel.Name,
                        Email = viewModel.Email,
                        Gender = viewModel.Gender,
                        DoB = viewModel.DoB,
                        DeleletedFlg = false
                    };
                    await _dbContext.Users.AddAsync(user);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task EditUserByIdAsync(User viewModel)
        {
            var user = await _dbContext.Users.FindAsync(viewModel.Id);
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    user.Name = viewModel.Name;
                    user.Email = viewModel.Email;
                    user.Gender = viewModel.Gender;
                    user.DoB = viewModel.DoB;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        public async Task DeleteUserByIdAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user is not null)
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        user.DeleletedFlg = true;
                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
    }
}