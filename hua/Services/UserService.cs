using hua.Data;
using hua.Entities;
using Microsoft.EntityFrameworkCore;

namespace hua.Services;

public class UserService(IDbContextFactory<AppDbContext> contextFactory)
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory = contextFactory;

    public async Task<User> GetUserById(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Users.FindAsync(id);
    }

    public async Task<User> GetUserByEmail(string email)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetAllUsersList()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Users.ToListAsync();
    }

    public async Task<List<User>> GetAdminsList()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Users.Where(u => u.Role == "admin").ToListAsync();
    }

    public async Task<List<User>> GetUsersList()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Users.Where(u => u.Role == "user").ToListAsync();
    }
}