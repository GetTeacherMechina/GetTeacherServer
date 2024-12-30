using System.Collections;

namespace GetTeacher.Server.DataStructures.Concurrent;

public class ConcurrentList<T>(int capacity = 1) : IEnumerable<T>
{
	private readonly List<T> list = new(capacity);
	private readonly object listLock = new();

	public void Add(T item)
	{
		lock (listLock)
		{
			list.Add(item);
		}
	}

	public bool Remove(T item)
	{
		lock (listLock)
		{
			return list.Remove(item);
		}
	}

	public bool Contains(T item)
	{
		lock (listLock)
		{
			return list.Contains(item);
		}
	}

	public int Count
	{
		get
		{
			lock (listLock)
			{
				return list.Count;
			}
		}
	}

	public void Clear()
	{
		lock (listLock)
		{
			list.Clear();
		}
	}

	public List<T> ToList()
	{
		lock (listLock)
		{
			return new List<T>(list);
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		List<T> snapshot;
		lock (listLock)
		{
			snapshot = new List<T>(list);
		}
		return snapshot.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public ConcurrentList<TResult> ConvertAll<TResult>(Converter<T, TResult> converter)
	{
		ArgumentNullException.ThrowIfNull(converter);

		lock (listLock)
		{
			var convertedList = list.ConvertAll(converter);
			var result = new ConcurrentList<TResult>(list.Count);
			foreach (var item in convertedList)
			{
				result.Add(item);
			}
			return result;
		}
	}
}