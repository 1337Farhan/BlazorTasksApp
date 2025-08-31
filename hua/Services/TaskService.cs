using hua.Data;
using hua.Entities;
using Microsoft.EntityFrameworkCore;

namespace hua.Services;

public class TaskService(IDbContextFactory<AppDbContext> contextFactory)
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory = contextFactory;

    public async Task<List<Entities.Task>> GetTasksListAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Tasks
        .Include(t => t.AssignedToUser)
        .ToListAsync();
    }

    public async Task<Entities.Task?> GetTaskByIdAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Tasks.FindAsync(id);
    }

    public async Task<Entities.Task> CreateTaskAsync(Entities.Task task)
    {
        using var context = _contextFactory.CreateDbContext();

        if (task.AssignedToUser != null && task.AssignedToUser.Id > 0)
        {
            context.Users.Attach(task.AssignedToUser);
        }
        else if (task.AssignedToUser != null && task.AssignedToUser.Id == 0)
        {
            // If user has ID 0, it might be a new user - set to null or handle appropriately
            task.AssignedToUser = null;
        }

        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        return task;
    }

    public async Task<Entities.Task?> UpdateTaskAsync(int id, Entities.Task task)
    {
        using var context = _contextFactory.CreateDbContext();
        var existingTask = await context.Tasks
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t => t.Id == id);
            
        if (existingTask == null) return null;

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.Status = task.Status;
        existingTask.CompletedDate = task.CompletedDate;

        // Handle user assignment
        if (task.AssignedToUser != null && task.AssignedToUser.Id > 0)
        {
            var user = await context.Users.FindAsync(task.AssignedToUser.Id);
            existingTask.AssignedToUser = user;
        }
        else
        {
            existingTask.AssignedToUser = null;
        }

        await context.SaveChangesAsync();
        return existingTask;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var task = await context.Tasks.FindAsync(id);
        if (task == null) return false;

        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<List<Entities.Task>> GetTasksListByUidAsync(User user)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Tasks
        .Include(t => t.AssignedToUser)
        .Where(t => t.AssignedToUser.Id == user.Id)
        .ToListAsync();
    }
}