using GetTeacher.Server.Services.Database.Models;
using GetTeacher.Server.Services.Managers.Interfaces;
using GetTeacher.Server.Services.Managers.Interfaces.UserState;

namespace GetTeacher.Server.Services.Managers.Implementations.UserStateTracker;

public class RedisUserStateStatus(IRedisCache redisCache) : IUserStateStatus
{
	private const string RedisHashKey = "onlineUsers";

	private readonly IRedisCache redisCache = redisCache;

	public async Task<ICollection<DbUser>> GetOnlineUsers()
	{
		return (await redisCache.GetAllKeysFromHashAsync(RedisHashKey))
			.ToList()
			.ConvertAll(id => new DbUser { Id = id });
	}

	public async Task<bool> IsUserOnline(DbUser user)
	{
		return (await redisCache.GetFromHashAsync(RedisHashKey, user.Id)) is not null;
	}

	public async Task SetOffline(DbUser user)
	{
		await redisCache.RemoveFromHashAsync(RedisHashKey, user.Id);
	}

	public async Task SetOnline(DbUser user)
	{
		await redisCache.AddToHashAsync(RedisHashKey, user.Id, "online");
	}
}