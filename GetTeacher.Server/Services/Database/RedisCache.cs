using GetTeacher.Server.Services.Managers.Interfaces;
using StackExchange.Redis;

namespace GetTeacher.Server.Services.Database;

public class RedisCache(IConnectionMultiplexer connectionMultiplexer) : IRedisCache
{
	private readonly IConnectionMultiplexer connectionMultiplexer = connectionMultiplexer;

	public async Task AddToHashAsync(string hashKey, int id, string value)
	{
		var db = connectionMultiplexer.GetDatabase();
		await db.HashSetAsync(hashKey, id.ToString(), value);
	}

	public async Task<string?> GetFromHashAsync(string hashKey, int id)
	{
		var db = connectionMultiplexer.GetDatabase();
		RedisValue value = await db.HashGetAsync(hashKey, id.ToString());

		if (value.HasValue)
			return value.ToString();
		return null;
	}

	public async Task RemoveFromHashAsync(string hashKey, int id)
	{
		var db = connectionMultiplexer.GetDatabase();
		await db.HashDeleteAsync(hashKey, id.ToString());
	}

	public async Task<Dictionary<int, string>> GetAllEntriesFromHashAsync(string hashKey)
	{
		var db = connectionMultiplexer.GetDatabase();
		var hashEntries = await db.HashGetAllAsync(hashKey);

		return hashEntries.ToDictionary(
			entry => int.Parse(entry.Name!),
			entry => entry.Value.ToString()!
		);
	}

	public async Task<ICollection<int>> GetAllKeysFromHashAsync(string hashKey)
		=> (await GetAllEntriesFromHashAsync(hashKey)).Keys;

	public async Task<ICollection<string>> GetAllValuesFromHashAsync(string hashKey)
		=> (await GetAllEntriesFromHashAsync(hashKey)).Values;
}
