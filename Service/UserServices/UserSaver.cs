using Core;
using DataLayer;
using Microsoft.EntityFrameworkCore;

namespace Service.UserServices;

public interface IUserServices
{
    Task<int> SaveUserAsync(AppUser user);
    Task<int> UpdateUserAsync(int userId, AppUser user);
    Task<AppUser?> GetUserAsync(int userId);
    Task<int> DeleteUserAsync(int userId);
    Task<IEnumerable<AppUser>?> GetAllUsersAsync();
}

public class UserServices : IUserServices
{
    private readonly MasterDbContext _masterContext;
    private readonly LogDbContext _logContext;

    public UserServices(MasterDbContext masterContext, LogDbContext logContext)
    {
        _masterContext = masterContext;
        _logContext = logContext;
    }

    private async Task SaveLog(AppUser appUser, bool isDeleted = false, bool isUpdated = false, bool isInserted = false)
    {
        var logKeyWord = isDeleted ? "deleted" : isUpdated ? "updated" : "created";
        var log = new SystemLog
        {
            AppUserId = appUser.AppUserId,
            LogDateTime = DateTime.Now,
            LogSerial = Guid.NewGuid().ToString().Substring(0, 5),
            Description = $"User {appUser.Username} {logKeyWord}"
        };
        await _logContext.AddAsync(log);
    }

    public async Task<int> DeleteUserAsync(int userId)
    {
        var user = await GetUserAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        user.Deleted = true;
        _masterContext.AppUser.Update(user);

        await SaveLog(user, isDeleted: true);

        await _masterContext.SaveChangesAsync();
        await _logContext.SaveChangesAsync();

        return user.AppUserId;
    }

    public async Task<IEnumerable<AppUser>?> GetAllUsersAsync()
    {
        return await _masterContext.AppUser
            .ToListAsync();
    }

    public async Task<AppUser?> GetUserAsync(int userId)
    {
        return await _masterContext.AppUser
            .FirstOrDefaultAsync(x => x.AppUserId == userId);
    }

    public async Task<int> SaveUserAsync(AppUser user)
    {
        await _masterContext.AddAsync(user);
        await _masterContext.SaveChangesAsync();

        await SaveLog(user, isInserted: true);
        await _logContext.SaveChangesAsync();

        return user.AppUserId;
    }

    public async Task<int> UpdateUserAsync(int userId, AppUser user)
    {
        var dbUser = await GetUserAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        dbUser.Username = user.Username;
        dbUser.EmailAddress = user.EmailAddress;
        dbUser.Password = user.Password;

        _masterContext.AppUser.Update(dbUser);

        await SaveLog(dbUser, isUpdated: true);

        await _masterContext.SaveChangesAsync();
        await _logContext.SaveChangesAsync();

        return dbUser.AppUserId;
    }
}